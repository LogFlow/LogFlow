using System;
using LogFlow.Builtins.Inputs;
using LogFlow.Builtins.Outputs;

namespace LogFlow.Examples
{
	public class FileToConsoleFlow : Flow
	{
		public FileToConsoleFlow()
		{
			Console.WriteLine("FileToConsoleFlow constructor is beeing run.");
			CreateProcess("fileToFlow", new FileInput(@"C:\Users\1323\Desktop\NLogTryout\NLogTryout\*.txt"))
				.AddProcess(new LineToConsoleOutput());
		}
	}
}
