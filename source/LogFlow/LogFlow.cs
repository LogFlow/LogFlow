using System;
using System.Threading;
using System.Threading.Tasks;
using NLog;

namespace LogFlow
{
	public abstract class LogFlow
	{
		private const int TimesToRetry = 3;
		private static readonly Logger Log = LogManager.GetCurrentClassLogger();
		private readonly LogFlowStructure _flowStructure = new LogFlowStructure();
		private CancellationTokenSource _tokenSource;
		private LogFlowStatus _currentStatus = LogFlowStatus.Stopped;
		
		public LogFlowStatus CurrentStatus
		{
			get { return _currentStatus; }
		}

		protected LogStructureWithoutInput CreateProcess()
		{
			return CreateProcess(GetType().Name);
		}

		protected LogStructureWithoutInput CreateProcess(string logType)
		{
			_flowStructure.Context = new LogContext(logType);
			return new LogStructureWithoutInput(_flowStructure);
		}

		internal LogFlowStructure FlowStructure
		{
			get { return _flowStructure; }
		}

		public void Start()
		{
			if (_currentStatus == LogFlowStatus.Running || _currentStatus == LogFlowStatus.Retrying) return;

			Log.Trace(string.Format("Starting flow '{0}'.", _flowStructure.Context.LogType));
			_flowStructure.StartAll(); 
			_tokenSource = new CancellationTokenSource();
			Task.Factory.StartNew(ExecuteProcess, _tokenSource.Token, TaskCreationOptions.LongRunning, TaskScheduler.Default);
		}

		public void Stop()
		{
			if (_tokenSource == null) return;

			Log.Trace(string.Format("Stopping flow '{0}'.", _flowStructure.Context.LogType));
			_tokenSource.Cancel();
		}

		private void ExecuteProcess()
		{
			var retriedTimes = 0;

			_currentStatus = LogFlowStatus.Running;
			Log.Trace(string.Format("Started flow '{0}'.", _flowStructure.Context.LogType));

			while (true)
			{
				try
				{
					ExecuteStructure();
					_tokenSource.Token.ThrowIfCancellationRequested();
				}
				catch (OperationCanceledException)
				{
					_flowStructure.StopAll();
					_currentStatus = LogFlowStatus.Stopped;
					Log.Trace(string.Format("Stopped flow '{0}'.", _flowStructure.Context.LogType));
					break;
				}
				catch (Exception ex)
				{
					if (retriedTimes < TimesToRetry)
					{
						retriedTimes++;
						_currentStatus = LogFlowStatus.Retrying;

						Log.WarnException(string.Format("Retrying flow '{0}' {1} times.", _flowStructure.Context.LogType, retriedTimes), ex);
						Thread.Sleep(TimeSpan.FromSeconds(10));
						continue;
					}

					_flowStructure.StopAll();
					_currentStatus = LogFlowStatus.Broken;

					Log.ErrorException(string.Format("Shut down broken flow '{0}'.", _flowStructure.Context.LogType), ex);
					break;
				}

				if (_currentStatus == LogFlowStatus.Retrying)
				{
					_currentStatus = LogFlowStatus.Running;

					Log.Info(string.Format("Resuming flow '{0}' after {1} times.", _flowStructure.Context.LogType, retriedTimes));
					retriedTimes = 0;
				}
			}
		}

		private void ExecuteStructure()
		{
			var result = GetResultFromInput();

			foreach (var processor in _flowStructure.Processors)
			{
				if (result.Canceled)
				{
					break;
				}

				result = processor.Process(result);
			}

			if (!result.Canceled)
			{
				_flowStructure.Output.Process(result);
			}

			_flowStructure.Input.LineIsProcessed(result.Id);
		}

		private Result GetResultFromInput()
		{
			var inputTask = Task.Run(() => _flowStructure.Input.GetLine());
			inputTask.Wait(_tokenSource.Token);

			return inputTask.Result;
		}
	}
}
