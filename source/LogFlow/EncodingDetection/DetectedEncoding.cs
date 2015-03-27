using System;
using System.Text;

namespace LogFlow.EncodingDetection
{
	/// <summary>
	/// The result of <see cref="IEncodingDetector.DetectEncoding"/>
	/// </summary>
	public struct DetectedEncoding
	{
		private static readonly DetectedEncoding _toFewBytesToDetermine = new DetectedEncoding(null, DetectionResult.ToFewBytesToDetermine);
		private static readonly DetectedEncoding _noEncoding = new DetectedEncoding(null, DetectionResult.NoEncoding);

		private readonly Encoding _encoding;
		private readonly DetectionResult _result;

		public DetectedEncoding(Encoding encoding)
		{
			if(encoding == null) throw new ArgumentNullException("encoding");
			_encoding = encoding;
			_result = DetectionResult.EncodingDetected;
		}

		private DetectedEncoding(Encoding encoding, DetectionResult result)
		{
			_encoding = encoding;
			_result = result;
		}

		/// <summary>
		/// Gets the detected encoding. If no encoding was detected, <c>null</c> is returned.
		/// </summary>
		public Encoding Encoding { get { return _encoding; } }

		/// <summary>
		/// Gets the outcome of the detection, i.e. if an encoding was detected or not, or if
		/// it was indeterminable as to few bytes were available.
		/// </summary>
		public DetectionResult Result { get { return _result; } }

		/// <summary>
		/// Gets the <see cref="DetectedEncoding"/> instance for when it is not possible to determine the encoding due to too few bytes available.
		/// </summary>
		public static DetectedEncoding ToFewBytesToDetermine { get { return _toFewBytesToDetermine; } }

		/// <summary>
		/// Gets the <see cref="DetectedEncoding"/> instance for when no encoding was detected.
		/// </summary>
		public static DetectedEncoding NoEncoding { get { return _noEncoding; } }
	}

}