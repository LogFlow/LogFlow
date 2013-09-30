namespace LogFlow
{
	public interface ILogProcessor
	{
		Result ExecuteProcess(FluentLogContext logContextContext, Result result);
	}
}
