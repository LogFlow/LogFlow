using System;
using System.IO;
using NLog;
using Topshelf;

namespace LogFlow
{
	class Program
	{
		private static Logger logger = LogManager.GetCurrentClassLogger();

		static void Main(string[] args)
		{
			try
			{
				logger.Info("Starting");

				Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);

				
				HostFactory.Run(x =>
				{
					x.Service<LogFlowEngine>(s =>
					{
						s.WhenStarted(tc => tc.Start());
						s.WhenStopped(tc => tc.Stop());
					});

					x.UseNLog();

					x.RunAsLocalSystem();
					x.SetDescription("Fluently processes log files");
					x.SetDisplayName("LogFlow");
					x.SetServiceName("LogFlow");
				});

				logger.Info("Started");
			}
			catch (Exception ex)
			{
				logger.Error("Failed to start", ex);
			}
			
			//Start flows, run i parralell
		}
	}
}
