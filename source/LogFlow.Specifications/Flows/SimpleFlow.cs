using LogFlow.Specifications.Helpers;

namespace LogFlow.Specifications.Flows
{
	public class SimpleFlow : Flow
	{
		public SimpleFlow()
		{
			CreateProcess("SimpleFlow", new EmptyInput())
				.AddProcess(new SimpleTagProcess("Duck"))
				.AddProcess(new SimpleTagProcess("Cow"))
				.AddProcess(new ReportToCurrentResultProcess());
		}
	}
}
