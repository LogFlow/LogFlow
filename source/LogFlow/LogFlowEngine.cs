using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using NLog;

namespace LogFlow
{
	public class LogFlowEngine
	{
		private static readonly Logger Log = LogManager.GetCurrentClassLogger();
		public static readonly FlowBuilder FlowBuilder = new FlowBuilder();

		public bool Start()
		{
			Log.Trace("Starting");

			var path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

			Log.Trace("Assembly Path:" + path);

			try
			{
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
			}
			catch(ReflectionTypeLoadException exception)
			{
				Log.Error(exception);
				Log.Error(exception.LoaderExceptions);
			}

			Task.WaitAll(FlowBuilder.Flows.Select(x => Task.Run(() => x.Start())).ToArray());

			return true;
		}

		public bool Stop()
		{
			Task.WaitAll(FlowBuilder.Flows.Select(x => Task.Run(() => x.Stop())).ToArray());
			return true;
		}
	}
}
