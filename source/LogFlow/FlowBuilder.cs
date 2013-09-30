using LogFlow.Builtins.Outputs;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace LogFlow
{
	public class FlowBuilder
	{
		private static Logger logger = LogManager.GetCurrentClassLogger();

		public List<FluentLogContext> Flows = new List<FluentLogContext>();

		public void BuildAndRegisterFlow(LogFlow logFlow)
		{
			if(logFlow.FluentLogContext == null)
			{
				logger.Error("No LogFlow has been registered for " + logFlow.GetType().FullName);
				return;
			}

			var flowToRegister = logFlow.FluentLogContext;

			if(string.IsNullOrWhiteSpace(flowToRegister.LogType))
			{
				logger.Error("No name for LogFlow has been registered for " + logFlow.GetType().FullName + ". A name must be entered for each processor.");
				return;
			}

			if(Flows.Any(f => f.LogType.Equals(flowToRegister.LogType, StringComparison.InvariantCultureIgnoreCase)))
			{
				logger.Error("There is already a LogFlow registered with the name " + flowToRegister.LogType + ". LogFlow names must be uniqe.");
				return;
			}

			if (flowToRegister.Input == null)
			{
				logger.Error("LogFlow " + flowToRegister.LogType + " doesn't have an input.");
				return;
			}

			if (flowToRegister.Processes == null || flowToRegister.Processes.Count == 0)
			{
				logger.Error("LogFlow " + flowToRegister.LogType + " doesn't have any type of processing.");
				return;
			}

			Flows.Add(flowToRegister);            
		}

		public void StartFlow(LogFlow logFlow)
		{
			var startJson = new JObject();

			var result = new Result() { Json = startJson };
			

			logFlow.FluentLogContext.Input.Start(logFlow.FluentLogContext, result);
		}
	}
}
