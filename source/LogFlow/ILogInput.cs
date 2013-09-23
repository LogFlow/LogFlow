using System;

namespace LogFlow
{
	public interface ILogInput
	{
		void StartReading(Func<Result, bool> processResult);
		void StopReading();
	}
}
