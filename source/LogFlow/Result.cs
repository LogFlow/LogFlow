using Newtonsoft.Json.Linq;

namespace LogFlow
{
	public class Result
	{
		public string Line { get; set; }
		public JObject Json { get; set; } 
	}
}
