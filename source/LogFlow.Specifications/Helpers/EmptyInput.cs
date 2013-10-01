namespace LogFlow.Specifications.Helpers
{
	public class EmptyInput : ILogInput
	{
		public void Start(FluentLogContext logContext, Result result)
		{
			logContext.TryRunProcesses(result);
		}

		public void Stop()
		{
		}
	}
}
