namespace LogFlow
{
	public interface ILogProcessor
	{
		Result ExecuteProcess(FluentLogContext logContext, Result result);
	}
}
