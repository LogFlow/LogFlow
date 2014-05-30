using LogFlow.Builtins.Outputs;
using LogFlow.Builtins.Processors;
using Nest;

namespace LogFlow.Examples
{
	public class RandomDataFlow : Flow
	{
		public RandomDataFlow()
		{
			var elasticConfiguration = new ElasticSearchConfiguration();
			elasticConfiguration.Host = "localhost";
			elasticConfiguration.Port = 9200;
			elasticConfiguration.IndexNameFormat = @"\b\a\c\o\n\l\o\g\-yyyyMMdd";
			elasticConfiguration.Mappings = mappings =>
			{
				mappings.String(m => m.Name("Message").Index(FieldIndexOption.not_analyzed));
			};

			CreateProcess().FromInput(new RandomBaconInput())
				.ToOutput(new ElasticSearchOutput(elasticConfiguration));
		}
	}
}
