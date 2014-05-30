using System;
using System.Collections.Generic;

namespace LogFlow.Builtins.Processors.IISLog
{
	public class LogLine
	{
		public DateTime TimeStamp { get; set; }
		public Dictionary<string, string> Fields { get; set; }
	}
}