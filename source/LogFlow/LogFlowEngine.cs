using System;
using System.Linq;
using NLog;

namespace LogFlow
{
	public class LogFlowEngine
	{
		private static Logger logger = LogManager.GetCurrentClassLogger();
		public static FlowBuilder FlowBuilder = new FlowBuilder();

		public bool Start()
		{
			var flowTypes = AppDomain.CurrentDomain.GetAssemblies()
					   .SelectMany(assembly => assembly.GetTypes())
					   .Where(type => type.IsSubclassOf(typeof(Flow)));

			foreach(var flowType in flowTypes)
			{
				try
				{
					var flow = (Flow)Activator.CreateInstance(flowType);
					FlowBuilder.BuildAndRegisterFlow(flow);
					FlowBuilder.StartFlow(flow);
				}
				catch(Exception exception)
				{
					logger.Error(exception.Data);
				}
			}

			//Log all running flows.

			return true;
		}

		public bool Stop()
		{
			//Kill all the things
			return true;
		}
	}
}
