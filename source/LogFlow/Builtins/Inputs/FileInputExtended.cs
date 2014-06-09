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
	public class FileInputExtended : LogInput, IStartable
	{
		private static readonly Logger Log = LogManager.GetCurrentClassLogger();
		
		private readonly ConcurrentDictionary<string, bool> _files = new ConcurrentDictionary<string, bool>();
		private readonly ConcurrentQueue<Result> _unprocessed = new ConcurrentQueue<Result>();
		private readonly FileSystemWatcher _watcher;
		private readonly IDictionary<string, long> _positionCache = new Dictionary<string, long>();

		private readonly string _path;
		private readonly Encoding _encoding;
		private readonly int _readBatchSize;
		private readonly int _checkIntervalSeconds;

		public FileInputExtended(string path) : this(path, Encoding.UTF8, false)
		{
		}

		public FileInputExtended(string path, Encoding encoding, bool includeSubDirectories, int readBatchSize = 100, int checkIntervalSeconds = 30)
		{
			_path = path;
			_encoding = encoding;
			_readBatchSize = readBatchSize;
			_checkIntervalSeconds = checkIntervalSeconds;

			_watcher = new FileSystemWatcher(GetPath(), GetSearchPattern()) {IncludeSubdirectories = includeSubDirectories};
			_watcher.Changed += (sender, args) => AddToQueueWithDuplicationCheck(args.FullPath);
			_watcher.Created += (sender, args) => AddToQueueWithDuplicationCheck(args.FullPath);
		}

		private void AddToQueueWithDuplicationCheck(string fullPath)
		{
			_files.AddOrUpdate(fullPath, true, (key, value) => value);
			Log.Trace(string.Format("{0}: Enqueuing file '{1}' for processing.", LogContext.LogType, fullPath));
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
			var searchOption = _watcher.IncludeSubdirectories ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
			return Directory.GetFiles(GetPath(), GetSearchPattern(), searchOption).OrderBy(f => new FileInfo(f).LastWriteTime);
		}

		public void Stop()
		{
			StopFileSystemWatcher();
			
			foreach (var position in _positionCache)
			{
				LogContext.Storage.Insert(position.Key, position.Value);
			}
		}

		public override Result GetLine()
		{
			while (true)
			{
				Result result;
				if (_unprocessed.TryDequeue(out result))
				{
					return result;
				}

				while (_files.Count == 0)
				{
					Thread.Sleep(TimeSpan.FromSeconds(1));
				}

				var files = _files.ToList();
				for (int i = 0; i < files.Count; i++)
				{
					var limit = (i + 1) * (_readBatchSize / files.Count);
					ReadLinesFromFile(files[i].Key, limit);
				}
				
				// if nothing new in files sleep for 30 seconds
				if (_unprocessed.Count == 0)
				{
					Thread.Sleep(30000);
				}
			}
		}

		private long GetPosition(string filePath)
		{
			var key = "position_" + filePath;

			long position;
			if (!_positionCache.TryGetValue(key, out position))
			{
				return LogContext.Storage.Get<long>(key);	
			}
			return position;
		}

		public void SavePosition(string filePath, long pos, bool persist)
		{
			var key = "position_" + filePath;
			_positionCache[key] = pos;

			if (persist)
			{
				LogContext.Storage.Insert(key, pos);	
			}
		}

		private void ReadLinesFromFile(string filePath, int limit)
		{
			try
			{
				using (var fs = new TextFileLineReader(filePath, _encoding))
				{
					var originalPosition = GetPosition(filePath);
					fs.Position = originalPosition;

					while (fs.Position < fs.Length)
					{
						var lineResult = fs.ReadLine();

						if (string.IsNullOrWhiteSpace(lineResult))
							continue;

						var result = new Result {Line = lineResult};
						result.MetaData[MetaDataKeys.FilePath] = filePath;
						result.Position = fs.Position;

						Log.Trace(string.Format("{0}: ({1}) from '{2}' at byte position {3}.", LogContext.LogType, result.Id, filePath, originalPosition));
						Log.Trace(string.Format("{0}: ({1}) line '{2}' read.", LogContext.LogType, result.Id, lineResult));

						_unprocessed.Enqueue(result);

						if (_unprocessed.Count >= limit)
						{
							break;
						}
					}
				}
			}
			catch (FileNotFoundException ex)
			{
				bool tmp;
				_files.TryRemove(filePath, out tmp);
			}
		}

		public override void LineIsProcessed(Result result)
		{
			SavePosition(result.MetaData[MetaDataKeys.FilePath], result.Position, persist: false);
		}
	}
}