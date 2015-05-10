using System;

namespace LogFlow
{
	[Obsolete("Use LogProcessor")]
	public abstract class LogOutput : ILogOutput
	{
		protected LogContext LogContext { get; private set; }

		public void SetContext(LogContext logContext)
		{
			LogContext = logContext;
		}

		public abstract void Process(Result result);
	}
}