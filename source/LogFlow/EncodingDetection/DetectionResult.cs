namespace LogFlow.EncodingDetection
{
	/// <summary>
	/// The outcome of detecting the encoding in <see cref="IEncodingDetector.DetectEncoding"/>
	/// </summary>
	public enum DetectionResult
	{
		/// <summary>
		/// An encoding was detected. <see cref="DetectedEncoding.Encoding"/> contains the detected encoding.
		/// </summary>
		EncodingDetected,

		/// <summary>
		/// The stream did not contain any known encodings. <see cref="DetectedEncoding.Encoding"/> will be <c>null</c>.
		/// </summary>
		NoEncoding,

		/// <summary>
		/// The stream did not contain enough number of bytes to determine. You need to recheck when more bytes are available.
		/// </summary>
		ToFewBytesToDetermine
	}
}