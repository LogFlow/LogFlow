using System.IO;

namespace LogFlow.EncodingDetection
{
	/// <summary>
	/// Detects the encoding in a stream.
	/// </summary>
	public interface IEncodingDetector
	{
		/// <summary>
		/// Detects the encoding in a stream. The stream must be seekable as we must be able to rewind the position
		/// </summary>
		/// <param name="seekableStream">The stream.</param>
		/// <returns></returns>
		DetectedEncoding DetectEncoding(Stream seekableStream);
	}
}