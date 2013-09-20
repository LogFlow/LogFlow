using System.Collections.Generic;

namespace LogFlow
{
	public abstract class Flow
	{
		private ILogInput Inputs;
		private List<ILogProcess> Processes = new List<ILogProcess>();


	}
}
