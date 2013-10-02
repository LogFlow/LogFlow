using System;
using LogFlow.Builtins.Outputs;
using Newtonsoft.Json.Linq;

namespace LogFlow.Builtins.Processors
{
	public class ElasticSearchTimestampToday : LogProcessor
	{
		public override Result Process(Result result)
		{
			result.Json.Add(ElasticSearchFields.Timestamp, new JValue(DateTime.Now));
			return result;
		}
	}
}