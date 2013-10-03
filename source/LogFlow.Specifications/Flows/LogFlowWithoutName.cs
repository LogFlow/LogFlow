using LogFlow.Specifications.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogFlow.Specifications.Flows
{
	public class LogFlowWithoutName : LogFlow
	{
		public LogFlowWithoutName()
		{
			CreateProcess().FromInput(new EmptyInput())
				.Then(new TestProcessor())
				.ToOutput(new ReportToCurrentResultOutput());
		}
	}
}
