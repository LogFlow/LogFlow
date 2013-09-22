using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogFlow.Specifications.Helpers
{
    public class TestProcess : ILogProcess
    {
        public Result ExecuteProcess(string logLine, JObject incomingResult)
        {
            return new Result();
        }
    }
}
