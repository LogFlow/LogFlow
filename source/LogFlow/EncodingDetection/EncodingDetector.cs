using System;
using System.IO;
using System.Text;

namespace LogFlow.EncodingDetection
{
	/// <summary>
	/// Capable of detecting the following byte order marks from a stream.
	/// <code>
	///   "EF BB BF"       UTF-8
	///   "FE FF"          UTF-16 Big Endian
	///   "FF FE"          UTF-16 Little Endian
	///   "00 00 FE FF"    UTF-32 Big Endian
	///   "FF FE 00 00"    UTF-32 Little Endian
	/// </code>
	/// </summary>
	public class EncodingDetector : IEncodingDetector
	{
		//As EncodingDetector has no state, we create an instance that can be reused.
		//However in order for users to create their own implementations based on this 
		//class the constructor is not made private
		private static readonly EncodingDetector _instance=new EncodingDetector();

		internal static EncodingDetector Instance { get { return _instance; } }


		public virtual DetectedEncoding DetectEncoding(Stream seekableStream)
		{
			return DetectEncoding(seekableStream, 4);
		}

		protected virtual DetectedEncoding DetectEncoding(Stream seekableStream, int maxNumberOfBytesInPreamble)
		{
			//Store the current position in the stream. This should normally be 0.
			var startPosition = seekableStream.Position;

			//Read as many bytes as we need for the preamble from the stream
			var preamble = new byte[maxNumberOfBytesInPreamble];
			var bytesRead = seekableStream.Read(preamble, 0, maxNumberOfBytesInPreamble);

			//Detect encoding
			int preambleLength;
			var result = DetectEncoding(preamble, bytesRead, out preambleLength);

			//Rewind the stream to the position after the preamble (which, if no preamble was detected, will be the same as startPosition)
			seekableStream.Position = startPosition + preambleLength;
			return result;
		}

		protected virtual DetectedEncoding DetectEncoding(byte[] bytes, int availableBytes, out int preambleLength)
		{
			//See http://en.wikipedia.org/wiki/Byte_order_mark for a list of BOMs
			//Currently this function detects:
			//   "EF BB BF"       UTF-8
			//   "FE FF"          UTF-16 Big Endian
			//   "FF FE"          UTF-16 Little Endian
			//   "00 00 FE FF"    UTF-32 Big Endian
			//   "FF FE 00 00"    UTF-32 Little Endian

			preambleLength = 0;

			if(availableBytes < 2)
			{
				return DetectedEncoding.ToFewBytesToDetermine;
			}
			bool preambleNeedsToBeChecked;
			Encoding detectedEncoding = null;

			if(bytes[0] == 0xFE && bytes[1] == 0xFF)
			{
				detectedEncoding = Encoding.BigEndianUnicode; // "FE FF": UTF-16 Big Endian
				preambleLength = 2;
				preambleNeedsToBeChecked = false;
			}
			else if(bytes[0] == 0xFF && bytes[1] == 0xFE) // "FF FE"
			{
				//We need at least three bytes to be able to distinguish between "FF FE" and "FF FE 00 00"
				if(availableBytes >= 3)
				{
					if(bytes[2] == 0x00) // "FF FE 00"
					{
						//We need the fourth byte to be certain we have "FF FE 00 00". If we cannot see what the
						//fourth byte is, we must wait and redetect once we have more bytes.
						if(availableBytes >= 4)
						{
							if(bytes[3] == 0x00) // "FF FE 00 00"
							{
								detectedEncoding = new UTF32Encoding(false, true); // "FF FE 00 00": UTF-32 Little Endian
								preambleLength = 4;
								preambleNeedsToBeChecked = false;
							}
							else
							{
								//We have "FF FE 00 xx" where xx!="00"
								detectedEncoding = Encoding.Unicode; // "FF FE": UTF-16 Little Endian
								preambleLength = 2;
								preambleNeedsToBeChecked = false;
							}
						}
						else
						{
							//We have "FF FE 00" but as we don't know what the fourth character is we cannot know for certain which encoding is used
							preambleNeedsToBeChecked = true;
						}
					}
					else
					{
						//We have "FF FE xx" where xx != "00"
						detectedEncoding = Encoding.Unicode; // "FF FE": UTF-16 Little Endian
						preambleLength = 2;
						preambleNeedsToBeChecked = false;
					}
				}
				else
				{
					//We have "FF FE" but have to no more bytes than that. It's to few bytes to distinguish between "FF FE" or "FF FE 00 00"
					preambleNeedsToBeChecked = true;
				}
			}
			else if(bytes[0] == 0xEF && bytes[1] == 0xBB)
			{
				if(availableBytes >= 3)
				{
					if(bytes[2] == 0xBF)
					{
						detectedEncoding = Encoding.UTF8; // "EF BB BF": UTF-8
						preambleLength = 3;
						preambleNeedsToBeChecked = false;
					}
					else
						preambleNeedsToBeChecked = false; //Third byte is not BF so no encoding used	
				}
				else
					preambleNeedsToBeChecked = true; //We only have two bytes. The third byte could be BF so we need to recheck
			}
			else if(bytes[0] == 0x00 && bytes[1] == 0x00)
			{
				if(availableBytes >= 3)
				{
					if(bytes[2] == 0xFE)
					{
						if(availableBytes >= 4)
						{
							if(bytes[3] == 0xFF)
							{
								detectedEncoding = new UTF32Encoding(true, true); // "00 00 FE FF": UTF-32 Big Endian
								preambleLength = 4;
								preambleNeedsToBeChecked = false;
							}
							else
								preambleNeedsToBeChecked = false; //Fourth byte is not FF so no encoding used	
						}
						else
							preambleNeedsToBeChecked = true; //We only have three bytes. The fourth byte could be FF so we need to recheck
					}
					else
						preambleNeedsToBeChecked = false; //Third byte is not FE so no encoding used	
				}
				else
					preambleNeedsToBeChecked = true; //We only have two bytes. The third byte could be BF so we need to recheck
			}
			else
				preambleNeedsToBeChecked = false;

			if(preambleNeedsToBeChecked)
				return DetectedEncoding.ToFewBytesToDetermine;


			if(detectedEncoding == null)
				return DetectedEncoding.NoEncoding;

			var detectedPreamble = new Byte[preambleLength];
			Array.Copy(bytes, detectedPreamble, preambleLength);
			return new DetectedEncoding(detectedEncoding);
		}
	}
}