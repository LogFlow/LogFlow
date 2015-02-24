using System;
using NLog;

namespace LogFlow.Builtins.Processors.IISLog
{
	public class IISLogProcessor : LogProcessor
	{
		private static readonly Logger Log = LogManager.GetCurrentClassLogger();

		public override Result Process(Result result)
		{
			if(result.Line.StartsWith("#"))
			{

				if(result.Line.StartsWith(Constants.HeaderField))
				{
					LogContext.Storage.Insert(GetStorageKey(result), new FieldParser().ParseFields(result.Line));
				}

				result.Canceled = true;
				return result;
			}

			try
			{
				var parsedLine = ParseLine(result);
				result.EventTimeStamp = parsedLine.TimeStamp;

				foreach(var field in parsedLine.Fields)
				{
				    var token = FieldToJToken.Parse(field);
					result.Json.Add(field.Key, token);
				}

				return result;
			}
			catch(Exception ex)
			{
				Log.ErrorException("Line: " + result.Line + " could not be parsed.", ex);
				result.Canceled = true;
				return result;
			}
			
		}

		private LogLine ParseLine(Result result)
		{
			var fields = LogContext.Storage.Get<string[]>(GetStorageKey(result));
			var parser = new LineParser(fields);
			return parser.ParseLogLine(result.Line);
		}

		private static string GetStorageKey(Result result)
		{
			return Constants.FieldStorageKey + "_" + GetFilePathFromMetaData(result);
		}

		private static string GetFilePathFromMetaData(Result result)
		{
			string filePath;
			return result.MetaData.TryGetValue(MetaDataKeys.FilePath, out filePath) ? filePath : string.Empty;
		}
	}
}
