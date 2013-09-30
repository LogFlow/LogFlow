using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace LogFlow.Specifications.Flows
{
	public class SimpleTagProcessor : ILogProcessor
	{
		private readonly string _tagName;

		public SimpleTagProcessor(string tagName)
		{
			_tagName = tagName;
		}

		public Result ExecuteProcess(Result result)
		{
			var tagArray = result.Json["tags"] as JArray ?? new JArray();
			tagArray.Add(new JValue(_tagName));
			result.Json["tags"] = tagArray;
			return result;
		}
	}
}
