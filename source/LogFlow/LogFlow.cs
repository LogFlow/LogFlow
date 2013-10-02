namespace LogFlow
{
	public abstract class LogFlow
	{
		public FluentLogContext FluentLogContext;

		protected FluentLogContext CreateProcess(string logType, ILogInput input)
		{
			FluentLogContext = new FluentLogContext(logType) { Input = input };
			return FluentLogContext;
		}
	}

	public static class FluentProcessExtensions
	{
		public static FluentLogContext AddProcess(this FluentLogContext fluentLogContext, ILogProcessor processor)
		{
			fluentLogContext.Processes.Add(processor);
			return fluentLogContext;
		}
	}
}
