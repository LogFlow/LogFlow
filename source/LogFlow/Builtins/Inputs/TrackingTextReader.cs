using System.IO;

namespace LogFlow.Builtins.Inputs
{
	public class TrackingTextReader : TextReader
	{
		private readonly TextReader baseReader;
		private long position;

		public TrackingTextReader(TextReader baseReader, long startPosition)
		{
			position = startPosition;
			this.baseReader = baseReader;
		}

		public long Position
		{
			get { return position; }
		}

		public override int Read()
		{
			position++;
			return baseReader.Read();
		}

		public override int Peek()
		{
			return baseReader.Peek();
		}
	}
}
