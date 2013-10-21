using System;
using System.Threading;
using System.Threading.Tasks;
using NLog;

namespace LogFlow
{
	public abstract class Flow
	{
		private const int TimesToRetry = 10;
		private static readonly Logger Log = LogManager.GetCurrentClassLogger();
		private readonly LogFlowStructure _flowStructure = new LogFlowStructure();
		private CancellationTokenSource _tokenSource;
		private LogFlowStatus _currentStatus = LogFlowStatus.Stopped;
		private Task _processTask;

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

		public string LogType
		{
			get { return _flowStructure.Context.LogType; }
		}

		internal LogFlowStructure FlowStructure
		{
			get { return _flowStructure; }
		}

		public void Start()
		{
			if (_currentStatus == LogFlowStatus.Running || _currentStatus == LogFlowStatus.Retrying) return;

			Log.Info(string.Format("{0}: Starting.", _flowStructure.Context.LogType));
			_flowStructure.StartAll(); 
			_tokenSource = new CancellationTokenSource();
			_processTask = Task.Factory.StartNew(ExecuteProcess, _tokenSource.Token, TaskCreationOptions.LongRunning, TaskScheduler.Default);
		}

		public void Stop()
		{
			if (_tokenSource == null) return;

			Log.Info(string.Format("{0}: Stopping.", _flowStructure.Context.LogType));
			_tokenSource.Cancel();

			if (_processTask != null)
			{
				_processTask.Wait();
			}
		}

		private void ExecuteProcess()
		{
			var retriedTimes = 0;

			_currentStatus = LogFlowStatus.Running;
			Log.Info(string.Format("{0}: Started.", _flowStructure.Context.LogType));

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
					Log.Info(string.Format("{0}: Stopped.", _flowStructure.Context.LogType));
					break;
				}
				catch (Exception ex)
				{
					if (retriedTimes < TimesToRetry)
					{
						retriedTimes++;
						_currentStatus = LogFlowStatus.Retrying;

						Log.Warn(string.Format("{0}: {1}", _flowStructure.Context.LogType, ex));
						Log.Warn(string.Format("{0}: Retrying {1} times.", _flowStructure.Context.LogType, retriedTimes));
						Thread.Sleep(TimeSpan.FromSeconds(10));
						continue;
					}

					_flowStructure.StopAll();
					_currentStatus = LogFlowStatus.Broken;

					Log.Error(ex);
					Log.Error(string.Format("{0}: Shut down because broken!", _flowStructure.Context.LogType));
					break;
				}

				if (_currentStatus == LogFlowStatus.Retrying)
				{
					_currentStatus = LogFlowStatus.Running;

					Log.Info(string.Format("{0}: Resuming after {1} times.", _flowStructure.Context.LogType, retriedTimes));
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
