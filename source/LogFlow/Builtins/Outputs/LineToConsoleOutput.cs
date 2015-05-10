using System;

namespace LogFlow.Builtins.Outputs
{
	public class LineToConsoleOutput : LogProcessor
	{
		public override Result Process(Result result)
		{
			Console.WriteLine(result.Line);
			return result;
		}
	}
}
