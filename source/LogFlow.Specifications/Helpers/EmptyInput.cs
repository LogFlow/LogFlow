using System;

namespace LogFlow.Specifications.Helpers
{
	public class EmptyInput : LogInput
	{
		public override Result GetLine()
		{
			return new Result();
		}

		public override void LineIsProcessed(Guid resultId)
		{
		}
	}
}
