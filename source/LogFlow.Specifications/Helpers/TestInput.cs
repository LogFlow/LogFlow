using System;

namespace LogFlow.Specifications.Helpers
{
	public class TestInput : ILogInput
	{

		public void StartReading(Func<Result, bool> processResult)
		{
		}

		public void StopReading()
		{
		}
	}
}
