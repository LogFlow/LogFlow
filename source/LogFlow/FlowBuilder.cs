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

		public List<FluentProcess> Flows = new List<FluentProcess>();

		public void BuildAndRegisterFlow(Flow flow)
		{
			if(flow.FluentProcess == null)
			{
				logger.Error("No flow has been registered for " + flow.GetType().FullName);
				return;
			}

			var flowToRegister = flow.FluentProcess;

			if(string.IsNullOrWhiteSpace(flowToRegister.Name))
			{
				logger.Error("No name for flow has been registered for " + flow.GetType().FullName + ". A name must be entered for each process.");
				return;
			}

			if(Flows.Any(f => f.Name.Equals(flowToRegister.Name, StringComparison.InvariantCultureIgnoreCase)))
			{
				logger.Error("There is already a flow registered with the name " + flowToRegister.Name + ". Flow names must be uniqe.");
				return;
			}

			if (flowToRegister.Input == null)
			{
				logger.Error("Flow " + flowToRegister.Name + " doesn't have an input.");
				return;
			}

			if (flowToRegister.Processes == null || flowToRegister.Processes.Count == 0)
			{
				logger.Error("Flow " + flowToRegister.Name + " doesn't have any type of processing.");
				return;
			}

			Flows.Add(flowToRegister);            
		}

		public void StartFlow(Flow flow)
		{
			var result = new Result();
			var json = new JObject(
				new JProperty("hostname", Environment.MachineName),
				new JProperty("loggerName", flow.GetType().FullName),
				new JProperty("loggerVersion", flow.GetType().Assembly.GetName().Version.ToString())
			);



			flow.FluentProcess.Input.Start(flow.FluentProcess, result);
		}
	}
}
