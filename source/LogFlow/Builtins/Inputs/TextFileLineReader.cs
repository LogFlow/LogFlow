using System;
using System.IO;
using System.Text;
using LogFlow.EncodingDetection;
using NLog;

namespace LogFlow.Builtins.Inputs
{
	public sealed class TextFileLineReader : IDisposable
	{
		private readonly bool _allowChangeEncoding;
		private static readonly Logger Log = LogManager.GetCurrentClassLogger();
		private readonly EncodingDetector _encodingDetector;
		private readonly long _length;
		private FileStream _fileStream;
		private BinaryReader _binReader;
		private Encoding _encoding;

		/// <summary>
		/// Initializes a new instance of the <see cref="TextFileLineReader"/> class. The encoding will default to UTF-8 unless
		/// the file contains byte order marks for UTF-16 or UTF-32.
		/// </summary>
		/// <param name="filePath">The path to text file.</param>
		public TextFileLineReader(string filePath) : this(filePath, Encoding.UTF8) { }

		/// <summary>
		/// Initializes a new instance of the <see cref="TextFileLineReader"/> class.
		/// </summary>
		/// <param name="filePath">The path to text file.</param>
		/// <param name="encoding">The encoding of text file.</param>
		/// <param name="encodingDetector">Optional: instance capable of detecting text encodings. 
		/// If <c>null</c> <see cref="EncodingDetector"/> will be used which can detect files with byte order marks
		/// for UTF-8, UTF-16 and UTF-32.</param>
		/// <param name="allowChangeEncoding">Optional: <c>true</c> to allow the reader to change to 
		/// another encoding if one is detected by <paramref name="encodingDetector"/>. Default is <c>true</c>.</param>
		public TextFileLineReader(string filePath, Encoding encoding, EncodingDetector encodingDetector = null, bool allowChangeEncoding = true)
		{
			_allowChangeEncoding = allowChangeEncoding;
			_encodingDetector = encodingDetector ?? EncodingDetector.Instance;

			if(!File.Exists(filePath))
				throw new FileNotFoundException("File (" + filePath + ") is not found.");


			_fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
			_length = _fileStream.Length;
			CreateBinaryReader(encoding);
		}

		private void CreateBinaryReader(Encoding encoding)
		{
			var safeEncoding = (Encoding)encoding.Clone();
			safeEncoding.DecoderFallback = new DecoderReplacementFallback("");
			if(_binReader != null)
				_binReader.Dispose();

			_binReader = new BinaryReader(_fileStream, safeEncoding, leaveOpen: true);
			_encoding = safeEncoding;
		}


		/// <summary>
		/// Reads a line of characters from the current stream at the current position and returns the data as a string.
		/// </summary>
		/// <returns>The next line from the input stream, or null if the end of the input stream is reached</returns>
		public string ReadLine()
		{
			var position = _binReader.BaseStream.Position;
			if(position == _binReader.BaseStream.Length)
				return null;

			if(position == 0)
				DetectEncoding();

			string line = "";
			int nextChar = _binReader.Read();
			while(nextChar != -1)
			{
				char current = (char)nextChar;
				if(current.Equals('\n'))
					break;
				else if(current.Equals('\r'))
				{
					int pickChar = _binReader.PeekChar();
					if(pickChar != -1 && ((char)pickChar).Equals('\n'))
						nextChar = _binReader.Read();
					break;
				}
				else
					line += current;
				nextChar = _binReader.Read();
			}
			return line;
		}


		private void DetectEncoding()
		{
			var detectedEncoding = _encodingDetector.DetectEncoding(_fileStream);
			switch(detectedEncoding.Result)
			{
				case DetectionResult.EncodingDetected:
					var isSameEncoding = Equals(detectedEncoding.Encoding, _encoding);
					if(!isSameEncoding)
					{
						if(_allowChangeEncoding)
						{
							Log.Trace("User requested encoding {0} but the file {1} uses encoding {2}. Switching to {2}.", _encoding.HeaderName, _fileStream.Name, detectedEncoding.Encoding.HeaderName);
							CreateBinaryReader(detectedEncoding.Encoding);
						}
						else
						{
							Log.Trace("User requested encoding {0} but the file {1} uses encoding {2}. User has requested to not change encoding so {0} will be used.", _encoding.HeaderName, _fileStream.Name, detectedEncoding.Encoding.HeaderName);
						}
					}
					else
					{
						Log.Trace("Detected encoding {0} for file {1}", detectedEncoding.Encoding.HeaderName, _fileStream.Name);
					}
					break;
				case DetectionResult.NoEncoding:
					break;
				case DetectionResult.ToFewBytesToDetermine:
					//_needToDetectEncoding = true;
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		/// <summary>
		/// Gets the length of text file (in bytes).
		/// </summary>
		public long Length
		{
			get { return _length; }
		}

		/// <summary>
		/// Gets or sets the current reading position.
		/// </summary>
		public long Position
		{
			get
			{
				if(_binReader == null)
					return -1;
				return _binReader.BaseStream.Position;
			}
			set
			{
				if(_binReader == null)
					return;

				SetPosition(value >= this.Length ? this.Length : value);
			}
		}

		private void SetPosition(long position)
		{
			_binReader.BaseStream.Seek(position, SeekOrigin.Begin);
		}

		/// <summary>
		/// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		/// </summary>
		public void Dispose()
		{
			if(_fileStream != null)
			{
				_fileStream.Close();
				_fileStream.Dispose();
				_fileStream = null;
			}
			if(_binReader != null)
			{
				_binReader.Close();
				_binReader.Dispose();
				_binReader = null;
			}
		}

	}
}
