using System;
using System.Collections.Concurrent;
using System.Text;

namespace LogFlow.Builtins.Inputs
{
	public class FileInput : ILogInput
	{
		private readonly string _path;
		private readonly Encoding _encoding;
		private readonly ConcurrentQueue<string> FileChangeQue = new ConcurrentQueue<string>();

		public FileInput(string path) : this(path, Encoding.UTF8) {}
		
		public FileInput(string path, Encoding encoding)
		{
			_path = path;
			_encoding = encoding;
		}

		public void StartReading(Func<Result, bool> processResult)
		{
			AddCurrentFilesToQue();
			StartFolderWatcher();
			StartFileQueReader();
		}

		public void StopReading()
		{
			
		}
	}
}
