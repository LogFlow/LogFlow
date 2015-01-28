using System;
using System.Collections.Generic;
using Nest;

namespace LogFlow.Builtins.Outputs
{
	public class ElasticSearchConfiguration
	{
		public ElasticSearchConfiguration()
		{
			Host = "localhost";
			Port = 9200;
			IndexNameFormat = @"\l\o\g\f\l\o\w\-yyyyMM";
			ConnectionLimit = 5;
		    MappingProperties = new Dictionary<PropertyNameMarker, IElasticType>
		    {
		        {
		            ElasticSearchFields.Source, new StringMapping() {Index = FieldIndexOption.NotAnalyzed}
		        },
		        {
		            ElasticSearchFields.Timestamp, new DateMapping() {Format = "date_time", Index = NonStringIndexOption.NotAnalyzed}
		        }
		    };
		}

		public string Host { get; set; }
		public int Port { get; set; }
		public string Ttl { get; set; }
		public int ConnectionLimit { get; set; }
		public string IndexNameFormat { get; set; }
		public Dictionary<PropertyNameMarker, IElasticType> MappingProperties { get; set; }

		public ConnectionSettings CreateConnectionFromSettings()
		{
			var clientSettings = new ConnectionSettings(new Uri(string.Format("http://{0}:{1}", Host, Port)));
			clientSettings.SetDefaultPropertyNameInferrer(name => name);
			return clientSettings;
		}
	}
}
