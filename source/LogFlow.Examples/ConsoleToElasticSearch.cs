using System;
using LogFlow.Builtins.Inputs;
using LogFlow.Builtins.Outputs;
using LogFlow.Builtins.Processors;

namespace LogFlow.Examples
{
	public class ConsoleToElasticSearch : Flow
	{
		public ConsoleToElasticSearch()
		{
			CreateProcess()
				.FromInput(new ConsoleInput())
				.Then(new SetEventTimeStampToNow())
				.Then(new ElasticSearchOutput(new ElasticSearchConfiguration()));
		}
	}
}