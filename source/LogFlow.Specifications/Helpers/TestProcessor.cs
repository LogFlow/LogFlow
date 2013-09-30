namespace LogFlow.Specifications.Helpers
{
	public class TestProcessor : ILogProcessor
	{
		public Result ExecuteProcess(Result result)
		{
			return new Result();
		}
	}
}
