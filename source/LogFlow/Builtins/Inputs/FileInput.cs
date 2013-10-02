using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NLog;

namespace LogFlow.Builtins.Inputs
{
	public class FileInput : ILogInput
	{
		private static readonly Logger Log = LogManager.GetCurrentClassLogger();

		private readonly CancellationTokenSource _tokenSource = new CancellationTokenSource();
		private readonly ConcurrentQueue<string> _changedFiles = new ConcurrentQueue<string>();
		private Dictionary<string, long> _currentFilePositions;
		private FileSystemWatcher _watcher;

		private readonly string _path;
		private readonly Encoding _encoding;

		public FileInput(string path) : this(path, Encoding.UTF8) { }
		
		public FileInput(string path, Encoding encoding)
		{
			_path = path;
			_encoding = encoding;
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

		public void Start(FluentLogContext logContext, Result result)
		{
			Log.Trace(string.Format("Starting FileInput: '{0}'", logContext.LogType));

			AddCurrentFilesToQueue();
			StartFileSystemWatcher();

			Task.Factory.StartNew(() =>
				{
					Log.Trace(string.Format("Started FileInput: '{0}'", logContext.LogType));

					_currentFilePositions = GetCurrentFilePositionsFromStorage(logContext);

					while (!_tokenSource.Token.IsCancellationRequested)
					{
						string filePath;
						if (!_changedFiles.TryDequeue(out filePath))
						{
							Thread.Sleep(TimeSpan.FromSeconds(1));
							continue;
						}

						Log.Trace(string.Format("Starting to read lines in '{0}'", filePath));

						using (var fs = new TextFileLineReader(filePath, _encoding))
						{
							fs.Position = GetLastReadFilePostion(filePath);

							while (fs.Position < fs.Length)
							{
								var lineResult = fs.ReadLine();

								if (string.IsNullOrWhiteSpace(lineResult))
									continue;

								result.Line = lineResult;
								logContext.TryRunProcesses(result);

								StoreLastReadFilePosition(logContext, filePath, fs.Position);
							}
						}
					}

					if (_tokenSource.Token.IsCancellationRequested)
					{
						Log.Trace(string.Format("Cancelled FileInput: '{0}'", logContext.LogType));
					}

				}, _tokenSource.Token, TaskCreationOptions.LongRunning, TaskScheduler.Default);
		}

		private long GetLastReadFilePostion(string filePath)
		{
			return _currentFilePositions.ContainsKey(filePath)
				       ? _currentFilePositions[filePath]
				       : 0;
		}

		private Dictionary<string, long> GetCurrentFilePositionsFromStorage(FluentLogContext logContext)
		{
			return StateStorage.Get<Dictionary<string, long>>(logContext.LogType) ?? new Dictionary<string, long>();
		}

		private void StoreLastReadFilePosition(FluentLogContext logContext, string filePath, long position)
		{
			if (_currentFilePositions.ContainsKey(filePath))
			{
				_currentFilePositions[filePath] = position;
			}
			else
			{
				_currentFilePositions.Add(filePath, position);
			}

			Log.Trace(string.Format("New position: {0}", position));

			StateStorage.Insert(logContext.LogType, _currentFilePositions);
		}

		private void StartFileSystemWatcher()
		{
			Log.Trace(string.Format("Starting FileSystemWatcher for {0}", _path));

			_watcher = new FileSystemWatcher(GetPath(), GetSearchPattern());
			_watcher.Changed += (sender, args) => _changedFiles.Enqueue(args.FullPath);
			_watcher.Created += (sender, args) => _changedFiles.Enqueue(args.FullPath);
			_watcher.EnableRaisingEvents = true;

			Log.Trace(string.Format("Started FileSystemWatcher for {0}", _path));
		}

		private void StopFileSystemWatcher()
		{
			if (_watcher == null)
			{
				return;
			}

			Log.Trace("Stopping FileSystemWatcher");

			_watcher.EnableRaisingEvents = false;
			_watcher.Dispose();

			Log.Trace("Stopped FileSystemWatcher");
		}

		private void AddCurrentFilesToQueue()
		{
			Log.Trace("Adding all current files as changed.");

			foreach (var file in GetCurrentFiles())
			{
				_changedFiles.Enqueue(file);
			}
		}

		private IEnumerable<string> GetCurrentFiles()
		{
			return Directory.GetFiles(GetPath(), GetSearchPattern()).OrderBy(f => new FileInfo(f).LastWriteTime);
		}

		public void Stop()
		{
			_tokenSource.Cancel();
			StopFileSystemWatcher();
		}
	}
}
