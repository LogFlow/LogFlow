using System;
using System.Collections.Generic;
using System.Linq;
using NLog;

namespace LogFlow
{
	public class FluentLogContext
	{
		private static Logger logger = LogManager.GetCurrentClassLogger();
		public string LogType { get; set; }
		public ILogInput Input { get; set; }
		public List<ILogProcessor> Processes = new List<ILogProcessor>();
		public bool TryRunProcesses(Result result)
		{
			try
			{
				Processes.Aggregate(result, (current, logProcess) => logProcess.ExecuteProcess(this, current));
				return true;
			}
			catch(Exception exception)
			{
				logger.Error(exception.Data);
				return false;
			}
		}
	}
}