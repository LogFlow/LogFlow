using System;

namespace LogFlow.Specifications.Helpers
{
	public class EmptyInput : LogInput
	{
		public static bool IsFinished;

		public override Result GetLine()
		{
			return new Result();
		}

		public override void LineIsProcessed(Guid resultId)
		{
			IsFinished = true;
		}
	}
}
