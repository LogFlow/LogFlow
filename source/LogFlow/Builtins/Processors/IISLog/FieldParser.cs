using System;

namespace LogFlow.Builtins.Processors.IISLog
{
	public class FieldParser
	{
		public string[] ParseFields(string fields)
		{
			return fields.Replace(Constants.HeaderField, "").Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries);
		}
	}
}