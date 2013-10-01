using System;
using System.Threading;
using System.Threading.Tasks;

namespace LogFlow.Builtins.Inputs
{
	public class ConsoleInput : ILogInput
	{
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
							break;
						}
					}
				}, _token);
		}

		public void Stop()
		{
			_tokenSource.Cancel();
		}
	}
}
