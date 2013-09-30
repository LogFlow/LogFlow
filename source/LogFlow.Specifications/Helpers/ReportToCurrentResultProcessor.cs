
namespace LogFlow.Specifications.Helpers
{
	public class ReportToCurrentResultProcessor : ILogProcessor
	{
		public static Result CurrentResult;
		public Result ExecuteProcess(Result result)
		{
			CurrentResult = result;
			return result;
		}
	}
}
