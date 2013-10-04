using LogFlow.Specifications.Helpers;

namespace LogFlow.Specifications.Flows
{
	public class FlowWithoutProcess : Flow
	{
		public FlowWithoutProcess()
		{
			CreateProcess("TestProcessor").FromInput(new EmptyInput());
		}
	}
}
