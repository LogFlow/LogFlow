using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using NLog;
using Nest;
using Newtonsoft.Json.Linq;

namespace LogFlow.Builtins.Outputs
{
	public class ElasticSearchOutput : ILogProcessor
	{
		private static Logger logger = LogManager.GetCurrentClassLogger();
		private readonly ElasticSearchConfiguration _configuration;
		private readonly HashSet<string> indexNames = new HashSet<string>();
		private readonly ElasticClient _client;
		private readonly RawElasticClient _rawClient;

		public ElasticSearchOutput(ElasticSearchConfiguration configuration)
		{
			_configuration = configuration;
			var clientSettings = new ConnectionSettings(new Uri(string.Format("http://{0}:{1}", configuration.Host, configuration.Port)));
			_rawClient = new RawElasticClient(clientSettings);
			_client = new ElasticClient(clientSettings);
		}

		public Result ExecuteProcess(FluentLogContext logContext, Result result)
		{
			//Ensure requered properties
			var timestampProperty = result.Json[ElasticSearchFields.Timestamp] as JValue;
			if(timestampProperty == null)
			{
				//Log and freak out
				return null;
			}

			DateTime timestamp;
			if(!DateTime.TryParse(timestampProperty.Value.ToString(), out timestamp))
			{
				//Log and freak out
				return null;
			}

			timestampProperty.Value = timestamp.ToString(CultureInfo.InvariantCulture.DateTimeFormat.SortableDateTimePattern);
			result.Json[ElasticSearchFields.Timestamp] = timestampProperty;
			
			var messageProperty = result.Json[ElasticSearchFields.Message] as JValue;
			if(messageProperty == null || string.IsNullOrWhiteSpace(messageProperty.Value.ToString()))
			{
				messageProperty = new JValue(result.Line);
				result.Json[ElasticSearchFields.Message] = messageProperty;
			}

			var lineId = Guid.NewGuid().ToString();
			result.Json[ElasticSearchFields.Id] = new JValue(lineId);
			result.Json[ElasticSearchFields.Type] = new JValue(logContext.LogType);
			result.Json[ElasticSearchFields.Source] = new JValue(Environment.MachineName);

			if(!string.IsNullOrWhiteSpace(_configuration.Ttl))
			{
				result.Json[ElasticSearchFields.TTL] = new JValue(_configuration.Ttl);
			}

			IndexLog(result.Json.ToString(Newtonsoft.Json.Formatting.None), timestamp, logContext.LogType, lineId);
			
			return result;
		}

		private void IndexLog(string jsonBody, DateTime timestamp, string logType, string lineId)
		{
			var indexName = BuildIndexName(timestamp);
			EnsureIndexExists(indexName);

			var indexResult = _rawClient.IndexPut(indexName, logType, lineId, jsonBody);

			if(!indexResult.Success)
			{
				logger.Error(string.Format("Failed to index: '{0}'. Result: '{1}'. Retrying...", jsonBody, indexResult.Result));
				Thread.Sleep(10000);
			}
		}

		private string BuildIndexName(DateTime timestamp)
		{
			return timestamp.ToString(_configuration.IndexNameFormat);
		}


		private void EnsureIndexExists(string indexName)
		{
			if(indexNames.Contains(indexName))
				return;

			if(CreateIndex(indexName))
			{
				indexNames.Add(indexName);
				return;
			}

			logger.Error("ElasticSearch Index could not be created");
			Thread.Sleep(10000);
		}


		private bool CreateIndex(string indexName)
		{
			if(_client.IndexExists(indexName).Exists)
				return true;

			var indexSettings = new IndexSettings();
			indexSettings.Add("index.store.compress.stored", true);
			indexSettings.Add("index.store.compress.tv", true);
			indexSettings.Add("index.query.default_field", ElasticSearchFields.Message);
			IIndicesOperationResponse result = _client.CreateIndex(indexName, indexSettings);

			CreateMappings(indexName);

			if(!result.OK)
			{
				logger.Error(string.Format("Failed to create index: '{0}'. Result: '{1}' Retrying...", indexName, result.ConnectionStatus.Result));
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
				.SourceField(s => s.SetCompression(true))
				.Properties(descriptor => descriptor
					.String(m => m.Name(ElasticSearchFields.Source).Index(FieldIndexOption.not_analyzed))
					.Date(m => m.Name(ElasticSearchFields.Timestamp).Index(NonStringIndexOption.not_analyzed))
					.String(m => m.Name(ElasticSearchFields.Type).Index(FieldIndexOption.not_analyzed))
					.String(m => m.Name(ElasticSearchFields.Message).IndexAnalyzer("whitespace"))
				)
			);
		}
	}
}
