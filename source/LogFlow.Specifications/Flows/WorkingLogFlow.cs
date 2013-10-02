using LogFlow.Specifications.Helpers;

namespace LogFlow.Specifications.Flows
{
	public class WorkingLogFlow : LogFlow
	{
		public WorkingLogFlow()
		{
			CreateProcess("TestProcessor")
				.FromInput(new EmptyInput())
				.Then(new TestProcessor())
				.ToOutput(new ReportToCurrentResultOutput());
		}
	}
}