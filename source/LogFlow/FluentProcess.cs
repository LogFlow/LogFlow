using System;
using System.Collections.Generic;
using System.Linq;
using NLog;

namespace LogFlow
{
	public class FluentProcess
	{
		private static Logger logger = LogManager.GetCurrentClassLogger();
		public string Name { get; set; }
		public ILogInput Input { get; set; }
		public List<ILogProcess> Processes = new List<ILogProcess>();
		public bool TryRunProcesses(Result result)
		{
			try
			{
				Processes.Aggregate(result, (current, logProcess) => logProcess.ExecuteProcess(current));
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