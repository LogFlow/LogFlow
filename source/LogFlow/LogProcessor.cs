namespace LogFlow
{
	public abstract class LogProcessor : ILogProcessor
	{
		protected LogContext LogContext { get; private set; }
		public void SetContext(LogContext logContext)
		{
			LogContext = logContext;
		}

		public abstract Result Process(Result result);
	}
}