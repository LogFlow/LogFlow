using LogFlow.Specifications.Helpers;

namespace LogFlow.Specifications.Flows
{
	public class SimpleLogFlow : LogFlow
	{
		public SimpleLogFlow()
		{
			CreateProcess("SimpleLogFlow", new EmptyInput())
				.AddProcess(new SimpleTagProcessor("Duck"))
				.AddProcess(new SimpleTagProcessor("Cow"))
				.AddProcess(new ReportToCurrentResultProcessor());
		}
	}
}
