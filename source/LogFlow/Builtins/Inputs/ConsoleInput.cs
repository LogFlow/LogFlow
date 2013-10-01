using System;
using System.Threading;
using System.Threading.Tasks;
using NLog;

namespace LogFlow.Builtins.Inputs
{
	public class ConsoleInput : ILogInput
	{
		private static readonly Logger Log = LogManager.GetCurrentClassLogger();
		private readonly CancellationTokenSource _tokenSource;
		private readonly CancellationToken _token;

		public ConsoleInput()
		{
			_tokenSource = new CancellationTokenSource();
			_token = _tokenSource.Token;
		}

		public void Start(FluentLogContext logContext, Result result)
		{
			Task.Factory.StartNew(() =>
				{
					Log.Trace(string.Format("Started ConsoleInput: '{0}'", logContext.LogType));

					while (true)
					{
						var lineResult = Console.ReadLine();

						if (string.IsNullOrWhiteSpace(lineResult))
						{
							continue;
						}

						result.Line = lineResult;
						logContext.TryRunProcesses(result);

						if (_token.IsCancellationRequested)
						{
							Log.Trace(string.Format("Cancelled ConsoleInput: '{0}'", logContext.LogType));
							break;
						}
					}
				}, _token, TaskCreationOptions.LongRunning, TaskScheduler.Default);
		}

		public void Stop()
		{
			_tokenSource.Cancel();
		}
	}
}
