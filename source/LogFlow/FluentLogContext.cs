using System;
using System.Collections.Generic;
using System.Linq;
using NLog;

namespace LogFlow
{
	public class FluentLogContext
	{
		private static Logger logger = LogManager.GetCurrentClassLogger();
		public string LogType { get; private set; }
		public ILogInput Input { get; set; }
		public List<ILogProcessor> Processes = new List<ILogProcessor>();

		public DateTime? BrokenStart { get; private set; }
		public bool IsBroken { get { return BrokenStart != null; } }
		public StateStorage Storage { get; private set; }
		public int NumberOfBrokenRetries { get; private set; }

		public FluentLogContext(string logType)
		{
			if (string.IsNullOrWhiteSpace(logType))
			{
				throw new ArgumentNullException("logType");
			}

			LogType = logType;
			Storage = new StateStorage(logType);
		}

		public void BreakFlow()
		{
			BrokenStart = DateTime.Now;
		}

		public void UnbreakFlow()
		{
			BrokenStart = null;
			NumberOfBrokenRetries = 0;
		}
 
		public bool TryRunProcesses(Result result)
		{
			try
			{
				Processes.Aggregate(result, (current, logProcess) => logProcess.ExecuteProcess(this, current));
				return true;
			}
			catch(Exception exception)
			{
				logger.ErrorException("RunProcesses failed!", exception);
				return false;
			}
		}
	}
}