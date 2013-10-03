using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using NLog;

namespace LogFlow.Builtins.Inputs
{
	public class FileInput : LogInput, IStartable
	{
		private static readonly Logger Log = LogManager.GetCurrentClassLogger();

		private readonly ConcurrentQueue<string> _changedFiles = new ConcurrentQueue<string>();
		private LineInProcess _lineInProcess;
		private readonly FileSystemWatcher _watcher;

		private readonly string _path;
		private readonly Encoding _encoding;

		public FileInput(string path) : this(path, Encoding.UTF8) { }

		public FileInput(string path, Encoding encoding)
		{
			_path = path;
			_encoding = encoding;

			_watcher = new FileSystemWatcher(GetPath(), GetSearchPattern());
			_watcher.Changed += (sender, args) => AddToQueueWithDuplicationCheck(args.FullPath);
			_watcher.Created += (sender, args) => AddToQueueWithDuplicationCheck(args.FullPath);
		}

		private void AddToQueueWithDuplicationCheck(string fullPath)
		{
			if (!_changedFiles.Contains(fullPath))
			{
				Log.Trace(string.Format("{0}: Enqueuing file '{1}' for processing.", LogContext.LogType, fullPath));
				_changedFiles.Enqueue(fullPath);
			}
		}


		private string GetPath()
		{
			return Path.GetDirectoryName(_path);
		}

		private string GetSearchPattern()
		{
			var fileName = Path.GetFileName(_path);
			return string.IsNullOrWhiteSpace(fileName) ? "*" : fileName.Trim();
		}

		public void Start()
		{
			Log.Info(string.Format("{0}: Starting FileInput.", LogContext.LogType));

			AddCurrentFilesToQueue();
			StartFileSystemWatcher();
		}

		private static string GetPositionKey(string filePath)
		{
			return "position_" + filePath;
		}

		private void StartFileSystemWatcher()
		{
			_watcher.EnableRaisingEvents = true;
			Log.Info(string.Format("{0}: Started FileSystemWatcher for path {1}", LogContext.LogType, _path));
		}

		private void StopFileSystemWatcher()
		{
			_watcher.EnableRaisingEvents = false;
			Log.Info(string.Format("{0}: Stopped FileSystemWatcher for path {1}", LogContext.LogType, _path));
		}

		private void AddCurrentFilesToQueue()
		{
			Log.Info(string.Format("{0}: Adding all current files as changed.", LogContext.LogType));

			foreach (var file in GetCurrentFiles())
			{
				AddToQueueWithDuplicationCheck(file);
			}
		}

		private IEnumerable<string> GetCurrentFiles()
		{
			return Directory.GetFiles(GetPath(), GetSearchPattern()).OrderBy(f => new FileInfo(f).LastWriteTime);
		}

		public void Stop()
		{
			StopFileSystemWatcher();
		}

		private void SetLineInProcess(Guid resultId, string filePath, long position)
		{
			_lineInProcess = new LineInProcess
				{
					ResultId = resultId,
					FilePath = filePath,
					Position = position
				};
		}

		public override Result GetLine()
		{
			while (true)
			{
				string filePath;
				while (!_changedFiles.TryPeek(out filePath))
				{
					Thread.Sleep(TimeSpan.FromSeconds(1));
				}

				using (var fs = new TextFileLineReader(filePath, _encoding))
				{
					var originalPosition = LogContext.Storage.Get<long>(GetPositionKey(filePath));
					fs.Position = originalPosition;

					while (fs.Position < fs.Length)
					{
						var lineResult = fs.ReadLine();

						if (string.IsNullOrWhiteSpace(lineResult))
							continue;
						
						var result = new Result { Line = lineResult };

						Log.Trace(string.Format("{0}: ({1}) from '{2}' at byte position {3}.", LogContext.LogType, result.Id, filePath, originalPosition));
						Log.Trace(string.Format("{0}: ({1}) line '{2}' read.", LogContext.LogType, result.Id, lineResult));

						SetLineInProcess(result.Id, filePath, fs.Position);
						return result;
					}

					_changedFiles.TryDequeue(out filePath);
				}
			}
		}

		public override void LineIsProcessed(Guid resultId)
		{
			Log.Trace(string.Format("{0}: ({1}) is processed.", LogContext.LogType, resultId));

			if (_lineInProcess.ResultId != resultId)
			{
				throw new InvalidOperationException("Result ids does not match.");
			}

			string filePath;
			_changedFiles.TryPeek(out filePath);

			if (_lineInProcess.FilePath != filePath)
			{
				throw new InvalidOperationException("File paths does not match.");
			}

			var positionKey = GetPositionKey(filePath);

			using (var fs = new TextFileLineReader(filePath, _encoding))
			{
				fs.Position = LogContext.Storage.Get<long>(positionKey);

				if (_lineInProcess.Position >= fs.Length)
				{
					_changedFiles.TryDequeue(out filePath);
				}
			}

			LogContext.Storage.Insert(positionKey, _lineInProcess.Position);
			_lineInProcess = null;
		}

		private class LineInProcess
		{
			public Guid ResultId;
			public string FilePath;
			public long Position;
		}
	}
}
