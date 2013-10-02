using System;

namespace LogFlow.Builtins.Outputs
{
	public class LineToConsoleOutput : LogOutput
	{
		public override void Process(Result result)
		{
			Console.WriteLine(result.Line);
		}
	}
}
