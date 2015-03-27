using System.IO;

namespace LogFlow.EncodingDetection
{
	/// <summary>
	/// Totally ignores any byte order marks in a file, and always returns <see cref="DetectedEncoding.NoEncoding"/> (no encoding detected).	
	/// </summary>
	public class NoEncodingDetector : IEncodingDetector
	{
		private static readonly NoEncodingDetector _instance = new NoEncodingDetector();

		private NoEncodingDetector() { }

		public static NoEncodingDetector Instance { get { return _instance; } }

		public DetectedEncoding DetectEncoding(Stream seekableStream)
		{
			return DetectedEncoding.NoEncoding;
		}

	}
}