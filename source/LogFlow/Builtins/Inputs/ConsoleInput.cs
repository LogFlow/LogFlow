using System;

namespace LogFlow.Builtins.Inputs
{
	public class ConsoleInput : LogInput
	{
		public override Result GetLine()
		{
			var result = new Result();

			while (string.IsNullOrWhiteSpace(result.Line))
			{
				result.Line = Console.ReadLine();
			}
			
			return result;
		}

		public override void LineIsProcessed(Guid resultId) { }
	}
}
