namespace LogFlow
{
	public class LogContext
	{
		public LogContext(string logType)
		{
			LogType = logType;
			Storage = new StateStorage(logType);
		}

		public string LogType { get; private set; }
		public StateStorage Storage { get; private set; }
	}
}