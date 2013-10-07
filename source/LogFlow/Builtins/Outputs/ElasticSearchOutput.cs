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
				throw new ApplicationException(string.Format("Failed to index: '{0}'. Result: '{1}'.", jsonBody, indexResult.Result));
			}

			Log.Trace(string.Format("{0}: ({1}) Indexed successfully.", LogContext.LogType, lineId));
		}

		private string BuildIndexName(DateTime timestamp)
		{
			return timestamp.ToString(_configuration.IndexNameFormat);
		}

		private void EnsureIndexExists(string indexName)
		{
			if(_indexNames.Contains(indexName))
				return;

			CreateIndex(indexName);
			_indexNames.Add(indexName);
		}


		private void CreateIndex(string indexName)
		{
			if(_client.IndexExists(indexName).Exists)
				return;

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
				throw new ApplicationException(string.Format("Failed to create index: '{0}'. Result: '{1}'", indexName, result.ConnectionStatus.Result));
			}

			Log.Trace(string.Format("{0}: Index '{1}' i successfully created.", LogContext.LogType, indexName));
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
			if(result.EventTimeStamp == null)
			{
				throw new ArgumentNullException(ElasticSearchFields.Timestamp);
			}

			var timestampIsoString = result.EventTimeStamp.Value.ToString(@"yyyy-MM-ddTHH\:mm\:ss.fff", CultureInfo.InvariantCulture);
			if(result.Json[ElasticSearchFields.Timestamp] == null)
			{
				result.Json.Add(ElasticSearchFields.Timestamp, new JValue(timestampIsoString));
			}
			else
			{
				result.Json[ElasticSearchFields.Timestamp] = new JValue(timestampIsoString);
			}
			
			var messageProperty = result.Json[ElasticSearchFields.Message] as JValue;
			if (messageProperty == null || string.IsNullOrWhiteSpace(messageProperty.Value.ToString()))
			{
				messageProperty = new JValue(result.Line);
				result.Json[ElasticSearchFields.Message] = messageProperty;
			}

			var lineId = result.Id.ToString();
			result.Json[ElasticSearchFields.Id] = new JValue(lineId);
			result.Json[ElasticSearchFields.Type] = new JValue(LogContext.LogType);
			result.Json[ElasticSearchFields.Source] = new JValue(Environment.MachineName);

			if (!string.IsNullOrWhiteSpace(_configuration.Ttl))
			{
				result.Json[ElasticSearchFields.TTL] = new JValue(_configuration.Ttl);
			}

			IndexLog(result.Json.ToString(Newtonsoft.Json.Formatting.None), result.EventTimeStamp.Value, LogContext.LogType, lineId);
		}
	}
}
