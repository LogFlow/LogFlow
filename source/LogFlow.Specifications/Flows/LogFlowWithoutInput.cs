using LogFlow.Specifications.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogFlow.Specifications.Flows
{
	public class LogFlowWithoutInput : LogFlow
	{
		public LogFlowWithoutInput()
		{
			CreateProcess("TestProcessor").FromInput(null).Then(new TestProcessor());
		}
	}
}