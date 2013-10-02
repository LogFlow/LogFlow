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
		private readonly FileSystemWatcher _watcher;

		private readonly string _path;
		private readonly Encoding _encoding;

		public FileInput(string path) : this(path, Encoding.UTF8) { }
		
		public FileInput(string path, Encoding encoding)
		{
			_path = path;
			_encoding = encoding;

			_watcher = new FileSystemWatcher(GetPath(), GetSearchPattern());
			_watcher.Changed += (sender, args) => _changedFiles.Enqueue(args.FullPath);
			_watcher.Created += (sender, args) => _changedFiles.Enqueue(args.FullPath);
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
			Log.Trace(string.Format("Starting FileInput: '{0}'", LogContext.LogType));

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
			Log.Trace(string.Format("Started FileSystemWatcher for {0}", _path));
		}

		private void StopFileSystemWatcher()
		{
			_watcher.EnableRaisingEvents = false;
			Log.Trace(string.Format("Stopped FileSystemWatcher for {0}", _path));
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
			StopFileSystemWatcher();
		}

		private readonly IDictionary<Guid, Tuple<string, long>> _unprocessedLines = new Dictionary<Guid, Tuple<string, long>>();

		private void AddUnprocessedLine(Guid resultId, string filePath, long position)
		{
			if (!_unprocessedLines.ContainsKey(resultId))
			{
				_unprocessedLines.Add(resultId, new Tuple<string, long>(GetPositionKey(filePath), position));
			}
		}

		private void StoreProcessedLine(Guid resultId)
		{
			if (!_unprocessedLines.ContainsKey(resultId)) return;

			var positionKey = _unprocessedLines[resultId].Item1;
			var position = _unprocessedLines[resultId].Item2;

			LogContext.Storage.Insert(positionKey, position);
			_unprocessedLines.Remove(resultId);
		}

		private readonly Queue<Result> _results = new Queue<Result>();

		public override Result GetLine()
		{
			while (!_results.Any())
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
					fs.Position = LogContext.Storage.Get<long>(GetPositionKey(filePath));

					while (fs.Position < fs.Length)
					{
						var lineResult = fs.ReadLine();

						if (string.IsNullOrWhiteSpace(lineResult))
							continue;

						var result = new Result {Line = lineResult};

						_results.Enqueue(result);
						AddUnprocessedLine(result.Id, filePath, fs.Position);
					}
				}
			}

			return _results.Dequeue();
		}

		public override void LineIsProcessed(Guid resultId)
		{
			StoreProcessedLine(resultId);
		}
	}
}
