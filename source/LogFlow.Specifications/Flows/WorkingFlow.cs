using LogFlow.Specifications.Helpers;

namespace LogFlow.Specifications.Flows
{
    public class WorkingFlow : Flow
    {
        public WorkingFlow()
        {
            CreateProcess("TestProcess", new TestInput())
                .AddProcess(new TestProcess());
        }
    }
}