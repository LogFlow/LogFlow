using System;
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
		}

		public string Host { get; set; }
		public int Port { get; set; }
		public string Ttl { get; set; }
		public int ConnectionLimit { get; set; }
		public string IndexNameFormat { get; set; }
		public Action<PropertiesDescriptor<dynamic>> Mappings { get; set; }

		public ConnectionSettings CreateConnectionFromSettings()
		{
			var clientSettings = new ConnectionSettings(new Uri(string.Format("http://{0}:{1}", Host, Port)));
			clientSettings.SetDefaultPropertyNameInferrer(name => name);
			return clientSettings;
		}
	}
}
