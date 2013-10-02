using System;
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
		}

		public Guid Id { get; private set; }
		public string Line { get; set; }
		public JObject Json { get; set; }
		public bool Canceled { get; set; }
	}
}
