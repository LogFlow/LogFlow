
namespace LogFlow.Specifications.Helpers
{
	public class ReportToCurrentResultOutput : LogOutput
	{
		public static Result CurrentResult;

		public override void Process(Result result)
		{
			CurrentResult = result;
		}
	}
}
