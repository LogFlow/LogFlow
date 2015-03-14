using System.IO;
using System.Text;

namespace LogFlow.Builtins.Components
{
	internal class Streams 
	{
        public static FileStream OpenFile(string filePath, long position) 
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException("File (" + filePath + ") is not found.");

            var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            fileStream.Seek(position >= fileStream.Length ? fileStream.Length : position, SeekOrigin.Begin);
            return fileStream;
        }

		/// <summary>
		/// Initializes a new instance of the <see cref="Streams"/> class.
		/// </summary>
        public static StreamReader GetStreamReaderWithFallback(Stream stream, Encoding encoding)
		{
			var safeEncoding = (Encoding)encoding.Clone();
			safeEncoding.DecoderFallback = new DecoderReplacementFallback("");
            return new StreamReader(stream, safeEncoding);
		}
    }
}
