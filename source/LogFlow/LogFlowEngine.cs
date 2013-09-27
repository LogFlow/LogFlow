using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using NLog;

namespace LogFlow
{
	public class LogFlowEngine
	{
		private static Logger logger = LogManager.GetCurrentClassLogger();
		public static FlowBuilder FlowBuilder = new FlowBuilder();

		public bool Start()
		{
			Console.WriteLine("Starting");

			var path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

			Console.WriteLine("Assembly Path:" + path);

			var allAssemblies = Directory.GetFiles(path, "*.dll").Select(dll => Assembly.LoadFile(dll)).ToList();

			var flowTypes = allAssemblies
					   .SelectMany(assembly => assembly.GetTypes())
					   .Where(type => type.IsSubclassOf(typeof(Flow)));

			logger.Trace("Number of flows found: " + flowTypes.Count());

			foreach(var flowType in flowTypes)
			{
				
				try
				{
					var flow = (Flow)Activator.CreateInstance(flowType);
					logger.Trace("Starting flow: " + flow.FluentProcess.Name);
					Console.WriteLine("Starting flow: " + flow.FluentProcess.Name);
					FlowBuilder.BuildAndRegisterFlow(flow);
					FlowBuilder.StartFlow(flow);
					logger.Trace("Started flow: " + flow.FluentProcess.Name);
					Console.WriteLine("Started flow: " + flow.FluentProcess.Name);
				}
				catch(Exception exception)
				{
					logger.Error(exception);
					Console.WriteLine(exception);
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
