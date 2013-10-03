using System.IO;
using LogFlow.Builtins.Inputs;
using LogFlow.Builtins.Outputs;
using LogFlow.Builtins.Processors;

namespace LogFlow.Examples
{
	public class FileToElasticSearch : LogFlow
	{
		public FileToElasticSearch()
		{
			CreateProcess()
				.FromInput(new FileInput(Path.Combine(Directory.GetCurrentDirectory(), "*.txt")))
				.Then(new ElasticSearchTimestampToday())
				.ToOutput(new ElasticSearchOutput(new ElasticSearchConfiguration()));
		}
	}
}