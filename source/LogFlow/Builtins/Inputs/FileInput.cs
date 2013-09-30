using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogFlow.Builtins.Inputs
{
	public class FileInput : ILogInput
	{
		private readonly string _directory;
		private bool runQueueProcessing = true;
		private readonly string _filter;
		private readonly Encoding _encoding;
		private Task _queueReaderTask;
		private readonly ConcurrentQueue<string> _fileChangeQue = new ConcurrentQueue<string>();
		private FileSystemWatcher _watcher;

		public FileInput(string path) : this(path, Encoding.UTF8) { }
		
		public FileInput(string path, Encoding encoding)
		{
			_directory = Path.GetDirectoryName(path);
			_filter = Path.GetFileName(path);
			_filter = string.IsNullOrWhiteSpace(_filter) ? "*" : _filter.Trim();
			
			_encoding = encoding;
		}

		public void Start(FluentLogContext logContextContext, Result result)
		{
			Console.WriteLine("Starting FileInput: " + logContextContext.LogType);
			AddCurrentFilesToQue();
			StartFolderWatcher();
			StartFileQueReader(logContextContext, result);
			Console.WriteLine("Started FileInput: " + logContextContext.LogType);
		}

		private void StartFileQueReader(FluentLogContext logContextContext, Result result)
		{
			_queueReaderTask = new Task(() =>
			{
				try
				{

				
				while(runQueueProcessing)
				{
					string dequedResult;
					if(!_fileChangeQue.TryDequeue(out dequedResult)) continue;
					
					var lastPostion = 0L;
					var filePositions = StateStorage.Get<Dictionary<string, long>>(logContextContext.LogType) ?? new Dictionary<string, long>();

					if(filePositions.ContainsKey(dequedResult))
						lastPostion = filePositions[dequedResult];

					Console.WriteLine("Starting to read lines in " + dequedResult);
					using(var fs = new TextFileLineReader(dequedResult, _encoding))
					{
						fs.Position = lastPostion;

						while(fs.Position < fs.Length)
						{
							var lineResult = fs.ReadLine();
							if(string.IsNullOrWhiteSpace(lineResult))
								continue;

							result.Line = lineResult;
							logContextContext.TryRunProcesses(result);
							
							if(filePositions.ContainsKey(dequedResult))
								filePositions[dequedResult] = fs.Position;
							else
								filePositions.Add(dequedResult, fs.Position);

							StateStorage.Insert(logContextContext.LogType, filePositions);
						}
					}
				}

				}
				catch(Exception ex)
				{
					Console.WriteLine(ex);
					
				}
				
			}, TaskCreationOptions.LongRunning);

			_queueReaderTask.Start();

		}

		private void StartFolderWatcher()
		{
			_watcher = new FileSystemWatcher(_directory, _filter);
			_watcher.Changed += (sender, args) => _fileChangeQue.Enqueue(args.FullPath);
			_watcher.Created += (sender, args) => _fileChangeQue.Enqueue(args.FullPath);
			_watcher.EnableRaisingEvents = true;
			
		}

		private void AddCurrentFilesToQue()
		{

			var filesToAdd = Directory.GetFiles(_directory, _filter).OrderBy(f => new FileInfo(f).LastWriteTime);
			filesToAdd.ToList().ForEach(file => _fileChangeQue.Enqueue(file));

		}

		public void Stop()
		{
			runQueueProcessing = false;
			_queueReaderTask.Dispose();
			_watcher.Dispose();
		}
	}
}
