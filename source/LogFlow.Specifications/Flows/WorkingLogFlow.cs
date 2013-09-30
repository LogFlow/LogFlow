using LogFlow.Specifications.Helpers;

namespace LogFlow.Specifications.Flows
{
    public class WorkingLogFlow : LogFlow
    {
        public WorkingLogFlow()
        {
            CreateProcess("TestProcessor", new EmptyInput())
                .AddProcess(new TestProcessor());
        }
    }
}