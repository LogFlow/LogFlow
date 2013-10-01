using System;
using System.IO;
using LogFlow.Builtins.Inputs;
using LogFlow.Builtins.Outputs;

namespace LogFlow.Examples
{
	public class FileToConsoleLogFlow : LogFlow
	{
		public FileToConsoleLogFlow()
		{
			Console.WriteLine("FileToConsoleLogFlow started for: '{0}'", GetFilePath());

			CreateProcess("fileToFlow", new FileInput(GetFilePath()))
				.AddProcess(new LineToConsoleOutput());
		}

		private string GetFilePath()
		{
			return Path.Combine(Directory.GetCurrentDirectory(), "*.txt");
		}
	}
}
