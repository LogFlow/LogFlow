using NLog;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LogFlow
{
	public class FlowBuilder
	{
		private static readonly Logger Log = LogManager.GetCurrentClassLogger();

		public List<LogFlow> Flows = new List<LogFlow>();

		public void BuildAndRegisterFlow(LogFlow logFlow)
		{
			if(logFlow.FlowStructure.Context == null)
			{
				Log.Error("No LogFlow has been registered for " + logFlow.GetType().FullName);
				return;
			}

			var flowStructure = logFlow.FlowStructure;

			if(string.IsNullOrWhiteSpace(flowStructure.Context.LogType))
			{
				Log.Error("No name for LogFlow has been registered for " + logFlow.GetType().FullName + ". A name must be entered for each LogFlow.");
				return;
			}

			if(Flows.Any(f => f.FlowStructure.Context.LogType.Equals(flowStructure.Context.LogType, StringComparison.InvariantCultureIgnoreCase)))
			{
				Log.Error("There is already a LogFlow registered with the name " + flowStructure.Context.LogType + ". LogFlow names must be unique.");
				return;
			}

			if (flowStructure.Input == null)
			{
				Log.Error("LogFlow " + flowStructure.Context.LogType + " doesn't have an input.");
				return;
			}

			if (flowStructure.Output == null)
			{
				Log.Error("LogFlow " + flowStructure.Context.LogType + " doesn't have an output.");
				return;
			}

			Flows.Add(logFlow);
		}
	}
}
