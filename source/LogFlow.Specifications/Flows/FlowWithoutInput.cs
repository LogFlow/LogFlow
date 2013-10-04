namespace LogFlow.Specifications.Flows
{
	public class FlowWithoutInput : Flow
	{
		public FlowWithoutInput()
		{
			CreateProcess("TestProcessor");
		}
	}
}