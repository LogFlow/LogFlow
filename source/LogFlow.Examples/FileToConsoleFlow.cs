using System.IO;
using LogFlow.Builtins.Inputs;
using LogFlow.Builtins.Outputs;

namespace LogFlow.Examples
{
	public class FileToConsoleFlow : Flow
	{
		public FileToConsoleFlow()
		{
			CreateProcess()
				.FromInput(new FileInput(Path.Combine(Directory.GetCurrentDirectory(), "*.txt")))
				.Then(new LineToConsoleOutput());
		}
	}
}
