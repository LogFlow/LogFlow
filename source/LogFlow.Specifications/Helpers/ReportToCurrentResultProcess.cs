
namespace LogFlow.Specifications.Helpers
{
	public class ReportToCurrentResultProcess : ILogProcess
	{
		public static Result CurrentResult;
		public Result ExecuteProcess(Result result)
		{
			CurrentResult = result;
			return result;
		}
	}
}
