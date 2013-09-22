using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogFlow.Specifications.Helpers
{
    public class TestInput : ILogInput
    {
        public Result ReadLine()
        {
            return new Result();
        }
    }
}
