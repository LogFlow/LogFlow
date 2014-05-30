using System;
using System.Globalization;
using System.Linq;

namespace LogFlow.Builtins.Processors.IISLog
{
	public class LineParser
	{
		private readonly string[] _fields;

		public LineParser(string[] fields)
		{
			_fields = fields;
		}

		public LogLine ParseLogLine(string logLine)
		{
			var splitted = logLine.Split(' ');

			return new LogLine
			{
				TimeStamp = ParseIISLogDate(splitted),
				Fields = _fields.ToDictionary(s => s, s => GetFieldValue(splitted, s))
			};
		}

		private DateTime ParseIISLogDate(string[] splitted)
		{
			return DateTime.Parse(GetDateString(splitted), CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal);
		}

		private string GetDateString(string[] splitted)
		{
			return string.Format("{0} {1}", GetFieldValue(splitted, LogLineFields.Date), GetFieldValue(splitted, LogLineFields.Time));
		}

		private string GetFieldValue(string[] values, string fieldName)
		{
			for(int i = 0; i < _fields.Length; i++)
			{
				if(_fields[i] == fieldName)
				{
					return values[i];
				}
			}

			return null;
		}
	}
}