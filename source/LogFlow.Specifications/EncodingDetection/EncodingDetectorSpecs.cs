using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using LogFlow.EncodingDetection;
using Xunit;
// ReSharper disable InconsistentNaming
// ReSharper disable UnusedMember.Global

namespace LogFlow.Specifications.EncodingDetection
{
	public class EncodingDetectorSpecs
	{
		[Theory, MemberData("Data_CorrectPreambles")]
		public void Given_correct_preambles_Then_it_should_detect_correctly(Data data)
		{
			PerformTest(data, DetectionResult.EncodingDetected);
		}

		[Theory, MemberData("Data_TooShortPreambles")]
		public void Given_to_short_preambles_Then_it_should_report_ToFewBytesToDetermine(Data data)
		{
			PerformTest(data, DetectionResult.ToFewBytesToDetermine);
		}

		[Theory, MemberData("Data_LooksLikePreambles")]
		public void Given_starts_that_resembles_preambles_Then_it_should_report_NoEncoding(Data data)
		{
			PerformTest(data, DetectionResult.NoEncoding);
		}

		private static void PerformTest(Data data, DetectionResult expectedResult)
		{
			var detector = new EncodingDetector();
			var memoryStream = new MemoryStream(data.Bytes);

			var result = detector.DetectEncoding(memoryStream);

			Assert.Equal(expectedResult, result.Result);
			Assert.Equal(data.Expected, result.Encoding);
			Assert.Equal(data.ExpectedPosition, memoryStream.Position);
		}

		private static readonly IEnumerable<Encoding> _supportedEncodingsExceptUTF16le = new[]
		{
			Encoding.UTF8,
			Encoding.BigEndianUnicode,     //UTF-16 Big Endian
			Encoding.UTF32,                //UTF-32 Little Endian
			new UTF32Encoding(true, true), //UTF-32 Big Endian
		};

		private static readonly IEnumerable<Encoding> _supportedEncodings = _supportedEncodingsExceptUTF16le.Concat(new[]
		{
			Encoding.Unicode,              //UTF-16 Little Endian
		});



		public static IEnumerable<object[]> Data_CorrectPreambles
		{
			get
			{
				foreach(var encoding in _supportedEncodingsExceptUTF16le)
				{
					var preamble = encoding.GetPreamble();

					//Yield the preamble
					yield return new object[] { new Data(preamble, preamble, encoding, preamble.Length) };

					//yield one with extra bytes
					yield return new object[] { new Data(Concat(preamble, (byte)'H', (byte)'i'), preamble, encoding, preamble.Length) };
				}

				//UTF-16le preamble in it self is not enough to make it distinguishable from UTF-32 Little Endian
				//   "FF FE"          UTF-16 Little Endian
				//   "FF FE 00 00"    UTF-32 Little Endian
				// So we add an extra byte != 00
				var utf16lePreamble = Encoding.Unicode.GetPreamble();
				var utf16Bytes = Concat(utf16lePreamble, (byte)'X');

				yield return new object[] { new Data(utf16Bytes, utf16lePreamble, Encoding.Unicode, utf16lePreamble.Length) };
			}
		}


		public static IEnumerable<object[]> Data_TooShortPreambles
		{
			get
			{
				//Yield all encodings but with one byte less
				foreach(var encoding in _supportedEncodings)
				{
					var preamble = encoding.GetPreamble();
					var bytes = RemoveLastByte(preamble);
					yield return new object[] { new Data(bytes, new byte[0], null, 0) };
				}

				//UTF-16le preamble in it self is not enough to make it distinguishable from UTF-32 Little Endian
				//   "FF FE"          UTF-16 Little Endian
				//   "FF FE 00 00"    UTF-32 Little Endian
				//So we yield FF FE and it should not be enough to be detected
				var utf16lePreamble = Encoding.Unicode.GetPreamble();
				yield return new object[] { new Data(utf16lePreamble, new byte[0], null, 0) };

			}
		}

		public static IEnumerable<object[]> Data_LooksLikePreambles
		{
			get
			{
				//Yield preambles that looks like the encoding's preambles but with foreign
				//character inserted at index 0 and 1
				foreach(var encoding in _supportedEncodings)
				{
					foreach(var position in Enumerable.Range(0, 1))
					{
						var preamble = encoding.GetPreamble();
						var bytes = InsertAtPosition(preamble, position, 'X');
						yield return new object[] { new Data(bytes, new byte[0], null, 0) };
					}
				}
			}
		}

		private static byte[] RemoveLastByte(byte[] preamble)
		{
			var bytes = new byte[preamble.Length - 1];
			Array.Copy(preamble, bytes, bytes.Length);
			return bytes;
		}

		public static byte[] InsertAtPosition(byte[] array, int position, char extraChar)
		{
			var length = array.Length;
			var newArray = new byte[length + 1];
			Array.Copy(array, newArray, position);
			newArray[position] = (byte)extraChar;
			Array.Copy(array, position, newArray, position + 1, array.Length - position);
			return newArray;
		}


		public static byte[] Concat(byte[] array, params byte[] items)
		{
			var length = array.Length;
			var newArray = new byte[length + items.Length];
			Array.Copy(array, newArray, length);
			Array.Copy(items, 0, newArray, length, items.Length);
			return newArray;
		}

		public class Data
		{
			public Data(byte[] bytes, byte[] preamble, Encoding expected, int expectedPosition)
			{
				Bytes = bytes;
				Preamble = preamble;
				Expected = expected;
				ExpectedPosition = expectedPosition;
			}

			public byte[] Bytes { get; private set; }
			public byte[] Preamble { get; private set; }
			public Encoding Expected { get; private set; }
			public int ExpectedPosition { get; private set; }
		}
	}
}