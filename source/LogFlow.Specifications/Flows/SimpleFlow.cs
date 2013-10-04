using LogFlow.Specifications.Helpers;

namespace LogFlow.Specifications.Flows
{
	public class SimpleFlow : Flow
	{
		public SimpleFlow()
		{
			CreateProcess("SimpleFlow")
				.FromInput(new EmptyInput())
				.Then(new SimpleTagProcessor("Duck"))
				.Then(new SimpleTagProcessor("Cow"))
				.ToOutput(new ReportToCurrentResultOutput());
		}
	}
}
