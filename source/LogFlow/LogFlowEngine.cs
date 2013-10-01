using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using NLog;
using Nancy.Hosting.Self;

namespace LogFlow
{
	public class LogFlowEngine
	{
		private static Logger logger = LogManager.GetCurrentClassLogger();
		public static FlowBuilder FlowBuilder = new FlowBuilder();
		public static NancyHost nancyHost;
		public bool Start()
		{
			Console.WriteLine("Starting");

			var path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

			Console.WriteLine("Assembly Path:" + path);

			var allAssemblies = Directory.GetFiles(path, "*.dll").Select(dll => Assembly.LoadFile(dll)).ToList();

			var flowTypes = allAssemblies
					   .SelectMany(assembly => assembly.GetTypes())
					   .Where(type => type.IsSubclassOf(typeof(LogFlow)));

			logger.Trace("Number of flows found: " + flowTypes.Count());

			foreach(var flowType in flowTypes)
			{
				
				try
				{
					var flow = (LogFlow)Activator.CreateInstance(flowType);
					logger.Trace("Starting flow: " + flow.FluentLogContext.LogType);
					Console.WriteLine("Starting flow: " + flow.FluentLogContext.LogType);
					FlowBuilder.BuildAndRegisterFlow(flow);
					FlowBuilder.StartFlow(flow);
					logger.Trace("Started flow: " + flow.FluentLogContext.LogType);
					Console.WriteLine("Started flow: " + flow.FluentLogContext.LogType);
				}
				catch(Exception exception)
				{
					logger.Error(exception);
					Console.WriteLine(exception);
				}
			}

			nancyHost = new NancyHost(new Uri("http://localhost:1234"));
			nancyHost.Start();
			//Log all running flows.

			return true;
		}

		public bool Stop()
		{
			//Kill all the things
			if(nancyHost != null)
			{
				nancyHost.Stop();
				nancyHost.Dispose();
			}
				
			return true;
		}
	}
}
