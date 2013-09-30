using LogFlow.Specifications.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogFlow.Specifications.Flows
{
    public class LogFlowWithoutProcess: LogFlow
    {
        public LogFlowWithoutProcess()
        {
            CreateProcess("TestProcessor", new EmptyInput());
        }
    }
}
