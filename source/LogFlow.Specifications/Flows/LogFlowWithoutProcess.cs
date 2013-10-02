using LogFlow.Specifications.Helpers;

namespace LogFlow.Specifications.Flows
{
	public class LogFlowWithoutProcess : LogFlow
	{
		public LogFlowWithoutProcess()
		{
			CreateProcess("TestProcessor").FromInput(new EmptyInput());
		}
	}
}
