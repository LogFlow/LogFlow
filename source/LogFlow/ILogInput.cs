namespace LogFlow
{
	public interface ILogInput
	{
		void Start(FluentLogContext logContext, Result result);
		void Stop();
	}
}
