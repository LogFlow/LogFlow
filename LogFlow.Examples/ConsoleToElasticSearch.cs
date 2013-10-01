using System;
using LogFlow.Builtins.Inputs;
using LogFlow.Builtins.Outputs;
using LogFlow.Builtins.Processors;

namespace LogFlow.Examples
{
	public class ConsoleToElasticSearch : LogFlow
	{
		public ConsoleToElasticSearch()
		{
			Console.WriteLine("ConsoleToElasticSearch constructor is beeing run.");
			CreateProcess("C2ES", new ConsoleInput())
				.AddProcess(new ElasticSearchTimestampToday())
				.AddProcess(new ElasticSearchOutput(new ElasticSearchConfiguration()));
		}
	}
}