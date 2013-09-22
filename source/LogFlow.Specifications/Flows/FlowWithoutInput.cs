using LogFlow.Specifications.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogFlow.Specifications.Flows
{
    public class FlowWithoutInput : Flow
    {
        public FlowWithoutInput()
        {
            CreateProcess("TestProcess", null)
                .AddProcess(new TestProcess());
        }
    }
}