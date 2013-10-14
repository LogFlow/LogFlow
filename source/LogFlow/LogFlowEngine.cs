using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using NLog;
using Nancy.Hosting.Self;

namespace LogFlow
{
	public class LogFlowEngine
	{
		private static readonly Logger Log = LogManager.GetCurrentClassLogger();
		public static readonly FlowBuilder FlowBuilder = new FlowBuilder();
		private static NancyHost _nancyHost;

		public bool Start()
		{
			Log.Trace("Starting");

			var path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

			Log.Trace("Assembly Path:" + path);

			var allAssemblies = Directory.GetFiles(path, "*.dll").Select(Assembly.LoadFile).ToList();

			var flowTypes = allAssemblies
					   .SelectMany(assembly => assembly.GetTypes())
					   .Where(type => !type.IsAbstract)
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

			Task.WaitAll(FlowBuilder.Flows.Select(x => Task.Run(() => x.Start())).ToArray());

			if (Config.EnableNancyHealthModule)
			{
				Log.Info("Starting Nancy health module");
				_nancyHost = new NancyHost(new Uri(Config.NancyHostUrl));
				_nancyHost.Start();
				Log.Info("Started Nancy health module on " + Config.NancyHostUrl);
			}

			return true;
		}

		public bool Stop()
		{
			Task.WaitAll(FlowBuilder.Flows.Select(x => Task.Run(() => x.Stop())).ToArray());

			if(_nancyHost != null)
			{
				Log.Info("Stopping Nancy health module");
				_nancyHost.Stop();
				_nancyHost.Dispose();
				Log.Info("Stopped Nancy health module");
			}

			return true;
		}
	}
}
