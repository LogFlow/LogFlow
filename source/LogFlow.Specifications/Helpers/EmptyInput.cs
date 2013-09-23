using System;

namespace LogFlow.Specifications.Helpers
{
	public class EmptyInput : ILogInput
	{

		public void StartReading(Func<Result, bool> processResult)
		{
			processResult(new Result());
		}

		public void StopReading()
		{
		}
	}
}
