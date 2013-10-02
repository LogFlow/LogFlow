using System;
using System.IO;
using NLog;
using Topshelf;

namespace LogFlow
{
	class Program
	{
		private static readonly Logger Log = LogManager.GetCurrentClassLogger();

		static void Main(string[] args)
		{
			try
			{
				Log.Info("Starting");
				Console.WriteLine("Things are happening");

				Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);

				
				HostFactory.Run(x =>
				{
					x.Service<LogFlowEngine>(s =>
					{
						s.ConstructUsing(name => new LogFlowEngine()); 
						s.WhenStarted(tc => tc.Start());
						s.WhenStopped(tc => tc.Stop());
					});
					
					x.RunAsLocalSystem();
					x.SetDescription("Fluently processes log files");
					x.SetDisplayName("LogFlow");
					x.SetServiceName("LogFlow");
				});

				Log.Info("Started");
			}
			catch(Exception ex)
			{
				Log.Error("Failed to start", ex);
				Console.WriteLine(ex);
			}
			
			//Start flows, run i parralell
		}
	}
}
