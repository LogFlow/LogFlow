using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace LogFlow
{
	public class Result
	{
		public Result(LogContext logContext)
		{
			Id = Guid.NewGuid();
			Json = new JObject();
			Json.Add(JSonKeys.LogType, logContext.LogType);
			Json.Add(JSonKeys.MachineName, System.Environment.MachineName);

			Canceled = false;
			MetaData = new Dictionary<string, string>();

		}

		public Guid Id { get; private set; }
		public string Line { get; set; }
		public JObject Json { get; set; }
		public bool Canceled { get; set; }
		public long Position { get; set; }
		public Dictionary<string, string> MetaData { get; private set; }
		public DateTime? EventTimeStamp { get; set; }
	}
}
