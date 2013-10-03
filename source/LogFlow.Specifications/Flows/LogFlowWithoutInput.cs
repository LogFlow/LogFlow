namespace LogFlow.Specifications.Flows
{
	public class LogFlowWithoutInput : LogFlow
	{
		public LogFlowWithoutInput()
		{
			CreateProcess("TestProcessor");
		}
	}
}