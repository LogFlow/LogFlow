using LogFlow.Builtins.Outputs;

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
			

			CreateProcess().FromInput(new RandomBaconInput())
				.Then(new ElasticSearchOutput(elasticConfiguration));
		}
	}
}
