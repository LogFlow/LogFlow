namespace LogFlow.Specifications.Helpers
{
	public class TestProcessor : ILogProcessor
	{
		public Result ExecuteProcess(FluentLogContext logContext, Result result)
		{
			return new Result();
		}
	}
}
