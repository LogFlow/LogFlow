using NLog;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LogFlow
{
	public class FlowBuilder
	{
		private static readonly Logger Log = LogManager.GetCurrentClassLogger();

		public List<Flow> Flows = new List<Flow>();

		public void BuildAndRegisterFlow(Flow flow)
		{
			if(flow.FlowStructure.Context == null)
			{
				Log.Error("No Flow has been registered for " + flow.GetType().FullName);
				return;
			}

			var flowStructure = flow.FlowStructure;

			if(string.IsNullOrWhiteSpace(flowStructure.Context.LogType))
			{
				Log.Error("No name for Flow has been registered for " + flow.GetType().FullName + ". A name must be entered for each Flow.");
				return;
			}

			if(Flows.Any(f => f.FlowStructure.Context.LogType.Equals(flowStructure.Context.LogType, StringComparison.InvariantCultureIgnoreCase)))
			{
				Log.Error("There is already a Flow registered with the name " + flowStructure.Context.LogType + ". Flow names must be unique.");
				return;
			}

			if (flowStructure.Input == null)
			{
				Log.Error("Flow " + flowStructure.Context.LogType + " doesn't have an input.");
				return;
			}

			if (flowStructure.Output == null)
			{
				Log.Error("Flow " + flowStructure.Context.LogType + " doesn't have an output.");
				return;
			}

			Flows.Add(flow);
		}
	}
}
