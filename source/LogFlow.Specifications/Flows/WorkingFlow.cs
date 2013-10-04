using LogFlow.Specifications.Helpers;

namespace LogFlow.Specifications.Flows
{
	public class WorkingFlow : Flow
	{
		public WorkingFlow()
		{
			CreateProcess("TestProcessor")
				.FromInput(new EmptyInput())
				.Then(new TestProcessor())
				.ToOutput(new ReportToCurrentResultOutput());
		}
	}
}