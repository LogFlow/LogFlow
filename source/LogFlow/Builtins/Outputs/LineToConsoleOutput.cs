using System;

namespace LogFlow.Builtins.Outputs
{
	public class LineToConsoleOutput : ILogProcess
	{
		public Result ExecuteProcess(Result result)
		{
			Console.WriteLine(result.Line);
			return result;
		}
	}
}
