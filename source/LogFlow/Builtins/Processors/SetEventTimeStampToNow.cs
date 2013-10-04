using System;

namespace LogFlow.Builtins.Processors
{
	public class SetEventTimeStampToNow : LogProcessor
	{
		public override Result Process(Result result)
		{
			result.EventTimeStamp = DateTime.Now;
			return result;
		}
	}
}