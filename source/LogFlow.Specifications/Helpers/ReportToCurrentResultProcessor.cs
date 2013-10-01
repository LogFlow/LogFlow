
namespace LogFlow.Specifications.Helpers
{
	public class ReportToCurrentResultProcessor : ILogProcessor
	{
		public static Result CurrentResult;
		public Result ExecuteProcess(FluentLogContext logContext, Result result)
		{
			CurrentResult = result;
			return result;
		}
	}
}
