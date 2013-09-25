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
		private readonly string _flowName;
		private readonly string _path;
		private readonly string _directory;
		private readonly string _filter;
		private readonly Encoding _encoding;
		private Task _queueReaderTask;
		private readonly ConcurrentQueue<string> _fileChangeQue = new ConcurrentQueue<string>();
		private FileSystemWatcher _watcher;

		public FileInput(string flowName, string path) : this(flowName, path, Encoding.UTF8) { }
		
		public FileInput(string flowName,  string path, Encoding encoding)
		{
			_flowName = flowName;
			_path = path;
			_directory = Path.GetDirectoryName(path);
			_filter = Path.GetFileName(path);
			_filter = string.IsNullOrWhiteSpace(_filter) ? "*" : _filter.Trim();
			
			_encoding = encoding;
		}

		public void StartReading(Func<Result, bool> processResult)
		{
			AddCurrentFilesToQue();
			StartFolderWatcher();
			StartFileQueReader();
		}

		private void StartFileQueReader()
		{
			_queueReaderTask = new Task(() =>
			{
				while(true)
				{
					string dequedResult;
					if(_fileChangeQue.TryDequeue(out dequedResult))
					{
						//Get poistion in file.
						var lastPostion = 0l;
						var somethig = StateStorage.Get<Dictionary<string, long>>("");
						if(somethig.ContainsKey(dequedResult))
							lastPostion = somethig[dequedResult];

						using(var fileStream = File.OpenText(dequedResult))
						{
							var mjau = fileStream.ReadLine();

						}


						file.Close();

						using(var fr = new FileReader(path.FullName, lastReadPosition, configuration.Encoding))
						{
						}

						//Read next line
						//Run process thinga maggiy
						//Save new position
					}
				}

			},
			TaskCreationOptions.LongRunning);

			_queueReaderTask.Start();
		}

		private void StartFolderWatcher()
		{
			_watcher = new FileSystemWatcher(_directory, _filter);
			_watcher.Changed += (sender, args) => _fileChangeQue.Enqueue(args.FullPath);
			_watcher.Created += (sender, args) => _fileChangeQue.Enqueue(args.FullPath);
		}

		private void AddCurrentFilesToQue()
		{

			var filesToAdd = Directory.GetFiles(_directory, _filter);
			filesToAdd.ToList().ForEach(file => _fileChangeQue.Enqueue(file));

		}

		public void StopReading()
		{
			_queueReaderTask.Dispose();
			_watcher.Dispose();
		}
	}
}
