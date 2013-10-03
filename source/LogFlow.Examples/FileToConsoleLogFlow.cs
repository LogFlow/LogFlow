using System.IO;
using LogFlow.Builtins.Inputs;
using LogFlow.Builtins.Outputs;

namespace LogFlow.Examples
{
	public class FileToConsoleLogFlow : LogFlow
	{
		public FileToConsoleLogFlow()
		{
			CreateProcess()
				.FromInput(new FileInput(Path.Combine(Directory.GetCurrentDirectory(), "*.txt")))
				.ToOutput(new LineToConsoleOutput());
		}
	}
}
