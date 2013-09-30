using System;
using Newtonsoft.Json.Linq;

namespace LogFlow
{
	public class Result
	{
		public Result()
		{
			Json = new JObject();
		}
		public string Line { get; set; }
		public JObject Json { get; set; }
	}
}
