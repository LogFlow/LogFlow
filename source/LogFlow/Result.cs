using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace LogFlow
{
	public class Result
	{
		public Result()
		{
			Id = Guid.NewGuid();
			Json = new JObject();
			Canceled = false;
			MetaData = new Dictionary<string, string>();
		}

		public Guid Id { get; private set; }
		public string Line { get; set; }
		public JObject Json { get; set; }
		public bool Canceled { get; set; }
		public Dictionary<string, string> MetaData { get; private set; }
		public DateTime? EventTimeStamp { get; set; }
	}
}
