using LogFlow.Specifications.Helpers;

namespace LogFlow.Specifications.Flows
{
	public class SimpleLogFlow : LogFlow
	{
		public SimpleLogFlow()
		{
			CreateProcess("SimpleLogFlow")
				.FromInput(new EmptyInput())
				.Then(new SimpleTagProcessor("Duck"))
				.Then(new SimpleTagProcessor("Cow"))
				.ToOutput(new ReportToCurrentResultOutput());
		}
	}
}
