namespace LogFlow
{
	public interface ILogInput
	{
		void Start(FluentLogContext logContextContext, Result result);
		void Stop();
	}
}
