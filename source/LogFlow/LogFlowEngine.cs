using System;
using System.IO;
using System.Linq;
using System.Reflection;
using NLog;
using Nancy.Hosting.Self;

namespace LogFlow
{
	public class LogFlowEngine
	{
		private static readonly Logger Log = LogManager.GetCurrentClassLogger();
		public static FlowBuilder FlowBuilder = new FlowBuilder();
		public static NancyHost NancyHost;

		public bool Start()
		{
			Log.Trace("Starting");

			var path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

			Log.Trace("Assembly Path:" + path);

			var allAssemblies = Directory.GetFiles(path, "*.dll").Select(Assembly.LoadFile).ToList();

			var flowTypes = allAssemblies
					   .SelectMany(assembly => assembly.GetTypes())
					   .Where(type => type.IsSubclassOf(typeof(Flow)));

			Log.Trace("Number of flows found: " + flowTypes.Count());

			foreach(var flowType in flowTypes)
			{
				try
				{
					var flow = (Flow)Activator.CreateInstance(flowType);
					FlowBuilder.BuildAndRegisterFlow(flow);
				}
				catch(Exception exception)
				{
					Log.Error(exception);
				}
			}

			foreach (var flow in FlowBuilder.Flows)
			{
				flow.Start();
			}

			NancyHost = new NancyHost(new Uri("http://localhost:1234"));
			NancyHost.Start();
			//Log all running flows.

			return true;
		}

		public bool Stop()
		{
			//Kill all the things
			foreach (var flow in FlowBuilder.Flows)
			{
				flow.Stop();
			}

			if(NancyHost != null)
			{
				NancyHost.Stop();
				NancyHost.Dispose();
			}

			return true;
		}
	}
}
