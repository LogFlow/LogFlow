using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using NLog;
using Nest;
using Newtonsoft.Json.Linq;

namespace LogFlow.Builtins.Outputs
{
	public class ElasticSearchOutput : LogOutput
	{
		private static readonly Logger Log = LogManager.GetCurrentClassLogger();
		private readonly ElasticSearchConfiguration _configuration;
		private readonly HashSet<string> _indexNames = new HashSet<string>();
		private readonly ElasticClient _client;
		private readonly RawElasticClient _rawClient;

		public ElasticSearchOutput(ElasticSearchConfiguration configuration)
		{
			_configuration = configuration;
			var clientSettings = new ConnectionSettings(new Uri(string.Format("http://{0}:{1}", configuration.Host, configuration.Port)));
			_rawClient = new RawElasticClient(clientSettings);
			_client = new ElasticClient(clientSettings);
		}

		private void IndexLog(string jsonBody, DateTime timestamp, string logType, string lineId)
		{
			var indexName = BuildIndexName(timestamp);
			EnsureIndexExists(indexName);

			var queryString = new NameValueCollection();

			if(!string.IsNullOrWhiteSpace(_configuration.Ttl))
			{
				queryString.Add("ttl", _configuration.Ttl);
			}

			var indexResult = _rawClient.IndexPut(indexName, logType, lineId, jsonBody, queryString);

			if (!indexResult.Success)
			{
				Log.Error(string.Format("Failed to index: '{0}'. Result: '{1}'. Retrying...", jsonBody, indexResult.Result));
			}
			else
			{
				Log.Trace(string.Format("Indexed '{0}' successfully.", lineId));
			}
		}

		private string BuildIndexName(DateTime timestamp)
		{
			return timestamp.ToString(_configuration.IndexNameFormat);
		}


		private void EnsureIndexExists(string indexName)
		{
			if(_indexNames.Contains(indexName))
				return;

			if(CreateIndex(indexName))
			{
				_indexNames.Add(indexName);
				return;
			}

			Log.Error("ElasticSearch Index could not be created");
		}


		private bool CreateIndex(string indexName)
		{
			if(_client.IndexExists(indexName).Exists)
				return true;

			var indexSettings = new IndexSettings
				{
					{"index.store.compress.stored", true},
					{"index.store.compress.tv", true},
					{"index.query.default_field", ElasticSearchFields.Message}
				};
			IIndicesOperationResponse result = _client.CreateIndex(indexName, indexSettings);

			CreateMappings(indexName);

			if (!result.OK)
			{
				Log.Error(string.Format("Failed to create index: '{0}'. Result: '{1}' Retrying...", indexName,
				                           result.ConnectionStatus.Result));
			}
			else
			{
				Log.Trace(string.Format("Index '{0}' i successfully created.", indexName));
			}

			return result.OK;
		}

		private void CreateMappings(string indexName)
		{
			_client.MapFluent(map => map
				.IndexName(indexName)
				.DisableAllField()
				.TypeName("_default_")
				.TtlField(t => t.SetDisabled(false))
				.SourceField(s => s.SetCompression())
				.Properties(descriptor => descriptor
					.String(m => m.Name(ElasticSearchFields.Source).Index(FieldIndexOption.not_analyzed))
					.Date(m => m.Name(ElasticSearchFields.Timestamp).Index(NonStringIndexOption.not_analyzed))
					.String(m => m.Name(ElasticSearchFields.Type).Index(FieldIndexOption.not_analyzed))
					.String(m => m.Name(ElasticSearchFields.Message).IndexAnalyzer("whitespace"))
				)
			);
		}

		public override void Process(Result result)
		{
			var timestampProperty = result.Json[ElasticSearchFields.Timestamp] as JValue;
			if (timestampProperty == null)
			{
				Log.Error(string.Format("{0} is null", ElasticSearchFields.Timestamp));
				throw new ArgumentNullException(ElasticSearchFields.Timestamp);
			}

			DateTime timestamp;
			if (!DateTime.TryParse(timestampProperty.Value.ToString(), out timestamp))
			{
				var message = string.Format("{0} could not be parsed as a datetime.", timestampProperty.Value);
				Log.Error(message);
				throw new ArgumentException(message, ElasticSearchFields.Timestamp);
			}

			timestampProperty.Value = timestamp.ToString(CultureInfo.InvariantCulture.DateTimeFormat.SortableDateTimePattern);
			result.Json[ElasticSearchFields.Timestamp] = timestampProperty;

			var messageProperty = result.Json[ElasticSearchFields.Message] as JValue;
			if (messageProperty == null || string.IsNullOrWhiteSpace(messageProperty.Value.ToString()))
			{
				messageProperty = new JValue(result.Line);
				result.Json[ElasticSearchFields.Message] = messageProperty;
			}

			var lineId = result.Id.ToString(); //Guid.NewGuid().ToString());
			result.Json[ElasticSearchFields.Id] = new JValue(lineId);
			result.Json[ElasticSearchFields.Type] = new JValue(LogContext.LogType);
			result.Json[ElasticSearchFields.Source] = new JValue(Environment.MachineName);

			if (!string.IsNullOrWhiteSpace(_configuration.Ttl))
			{
				result.Json[ElasticSearchFields.TTL] = new JValue(_configuration.Ttl);
			}

			IndexLog(result.Json.ToString(Newtonsoft.Json.Formatting.None), timestamp, LogContext.LogType, lineId);
		}
	}
}
