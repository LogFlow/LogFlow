namespace LogFlow
{
	public interface ILogInput : INeedContext
	{
		Result GetLine();
		void LineIsProcessed(Result result);
	}

	public interface IStartable
	{
		void Start();
		void Stop();
	}

	public abstract class LogInput : ILogInput
	{
		protected LogContext LogContext { get; private set; }
		public void SetContext(LogContext logContext)
		{
			LogContext = logContext;
		}

		public abstract Result GetLine();
		public abstract void LineIsProcessed(Result result);
	}
}
