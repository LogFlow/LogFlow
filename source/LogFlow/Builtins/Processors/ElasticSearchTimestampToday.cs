using System;
using LogFlow.Builtins.Outputs;
using Newtonsoft.Json.Linq;

namespace LogFlow.Builtins.Processors
{
	public class ElasticSearchTimestampToday : ILogProcessor
	{
		public Result ExecuteProcess(FluentLogContext logContext, Result result)
		{
			result.Json.Add(ElasticSearchFields.Timestamp, new JValue(DateTime.Now));
			return result;
		}
	}
}