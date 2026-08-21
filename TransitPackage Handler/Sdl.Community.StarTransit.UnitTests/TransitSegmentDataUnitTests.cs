using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using Sdl.Community.StarTransit.Shared.Import;
using Xunit;

namespace Sdl.Community.StarTransit.UnitTests
{
	public class TransitSegmentDataUnitTests
	{
		//Data attributes taken as they are from the sample package attached to
		//https://community.rws.com/product-groups/trados-portfolio/rws-appstore/f/rws-appstore/61608
		private const string MachineTranslatedSegment =
			"\uE908\uEA25\uEC01\uED10\uEE06\uF502\uEF02C:\\Users\\Public\\Documents\\Transit NXT\\projects\\Nxt_Word\\_AEXTR_MT_DLP_DEU" +
			"\uEF030\uEF0420260731\uEF05152925\uEF10\u4000\uEFB0\u401D\u401D\u401D\u401D\u401D\u4000\u4000\uEFD33\uEFD6\u4004\uEFDC\u4004";

		private const string ReferenceMatchSegment =
			"\uE90A\uEA0D\uEB64\uED10\uEE05\uF50A\uEF02C:\\Users\\Public\\Documents\\Transit NXT\\projects\\Nxt_Word\\REF\\Word_ref1" +
			"\uEF030\uEF0420100114\uEF05122909\uEF06Support\uEF0720100114\uEF08121827\uEF09Support" +
			"\uEF10\u4000\uEFB0\u401D\u401D\u401D\u401D\u401D\u4000\u4000\uEFD31\uEFD6\u4004";

		//A Transit file carrying the Data attributes of the sample package as they were written by Transit,
		//with the translated text replaced by placeholders
		private readonly string _machineTranslatedFilePath =
			Path.Combine($"{AppDomain.CurrentDomain.BaseDirectory}", "TestingFiles", "MachineTranslatedSegments.DEU");

		[Fact]
		public void IsMachineTranslated_RealMachineTranslatedSegment_ReturnsTrue()
		{
			var segmentData = new TransitSegmentData(MachineTranslatedSegment);
			Assert.True(segmentData.IsMachineTranslated);
		}

		[Fact]
		public void IsMachineTranslated_RealReferenceMatchSegment_ReturnsFalse()
		{
			var segmentData = new TransitSegmentData(ReferenceMatchSegment);
			Assert.False(segmentData.IsMachineTranslated);
		}

		[Theory]
		[InlineData(0x25)]
		[InlineData(0x20)]
		[InlineData(0x2D)]
		public void IsMachineTranslated_MachineTranslationFlagSet_ReturnsTrue(int segmentFlags)
		{
			var segmentData = new TransitSegmentData(CreateData(segmentFlags, null, "REF\\Word_ref1"));
			Assert.True(segmentData.IsMachineTranslated);
		}

		[Theory]
		[InlineData(0x0D)]
		[InlineData(0x09)]
		[InlineData(0x00)]
		public void IsMachineTranslated_MachineTranslationFlagNotSet_ReturnsFalse(int segmentFlags)
		{
			var segmentData = new TransitSegmentData(CreateData(segmentFlags, 100, "REF\\Word_ref1"));
			Assert.False(segmentData.IsMachineTranslated);
		}

		[Fact]
		public void IsMachineTranslated_NoFlagButMachineTranslationExtract_ReturnsTrue()
		{
			//Transit versions which do not set the flag are recognised by the file the pretranslation came from
			var segmentData = new TransitSegmentData(CreateData(null, null, "projects\\Nxt_Word\\_AEXTR_MT_DLP_DEU"));
			Assert.True(segmentData.IsMachineTranslated);
		}

		[Theory]
		[InlineData("REF\\Word_ref1")]
		[InlineData("REF\\MT_Handbuch")]
		[InlineData("REF\\Text_AEXTR_MT_notes")]
		[InlineData("_AEXTR_1")]
		[InlineData("")]
		public void IsMachineTranslated_NoFlagAndNoMachineTranslationExtract_ReturnsFalse(string referenceFile)
		{
			var segmentData = new TransitSegmentData(CreateData(null, 100, referenceFile));
			Assert.False(segmentData.IsMachineTranslated);
		}

		[Theory]
		[InlineData(100, 100)]
		[InlineData(65, 65)]
		[InlineData(0, 0)]
		[InlineData(101, 100)] //Transit reports context matches above 100, Studio does not allow it
		public void MatchRate_MatchRateRecorded_ReturnsRecordedRate(int matchRate, int expected)
		{
			var segmentData = new TransitSegmentData(CreateData(0x0D, matchRate, "REF\\Word_ref1"));
			Assert.Equal(expected, segmentData.MatchRate);
		}

		[Fact]
		public void MatchRate_NoMatchRateButPretranslated_ReturnsFullMatch()
		{
			//Transit versions which do not record a match rate only ever pretranslated with full matches
			var segmentData = new TransitSegmentData(CreateData(0x0D, null, "REF\\Word_ref1"));
			Assert.Equal(100, segmentData.MatchRate);
		}

		[Fact]
		public void MatchRate_NotPretranslated_ReturnsNoMatch()
		{
			var segmentData = new TransitSegmentData(CreateData(0x0D, null, null));
			Assert.Equal(0, segmentData.MatchRate);
		}

		[Theory]
		[InlineData("C:\\projects\\Nxt_Word\\REF\\Word_ref1", "Word_ref1")]
		[InlineData("Word_ref1", "Word_ref1")]
		public void ReferenceFileName_ReturnsFileNameWithoutFolder(string referenceFile, string expected)
		{
			var segmentData = new TransitSegmentData(CreateData(0x0D, 100, referenceFile));
			Assert.Equal(expected, segmentData.ReferenceFileName);
		}

		[Theory]
		[InlineData(null)]
		[InlineData("")]
		public void EmptyData_IsNotPretranslated(string data)
		{
			var segmentData = new TransitSegmentData(data);

			Assert.False(segmentData.IsMachineTranslated);
			Assert.Equal(0, segmentData.MatchRate);
			Assert.Empty(segmentData.ReferenceFileName);
		}

		[Fact]
		public void MachineTranslatedFile_SeparatesMachineTranslationFromReferenceMatches()
		{
			var segments = ReadSegmentData(_machineTranslatedFilePath).ToList();
			Assert.Equal(33, segments.Count);

			//The package was pretranslated from both a machine translation extract and reference material,
			//none of the machine translated segments may be reported to the translator as a memory match
			Assert.Equal(24, segments.Count(segment => segment.IsMachineTranslated));
			Assert.Equal(9, segments.Count(segment => !segment.IsMachineTranslated && segment.MatchRate == 100));
			Assert.All(segments.Where(segment => segment.IsMachineTranslated),
				segment => Assert.StartsWith("_AEXTR_MT_", segment.ReferenceFileName));
		}

		/// <summary>
		/// Builds a Data attribute holding the fields in the order Transit writes them.
		/// </summary>
		private static string CreateData(int? segmentFlags, int? matchRate, string referenceFile)
		{
			var data = new StringBuilder("\uE908");
			if (segmentFlags != null) data.Append((char)(0xEA00 | segmentFlags.Value));
			if (matchRate != null) data.Append((char)(0xEB00 | matchRate.Value));
			if (referenceFile != null) data.Append('\uEF02').Append(referenceFile);

			//the fields Transit writes after the reference file, which must not be read as part of it
			data.Append("\uEF030\uEF0420260731\uEF05152925");
			return data.ToString();
		}

		private static IEnumerable<TransitSegmentData> ReadSegmentData(string transitFilePath)
		{
			var transitFile = new XmlDocument { PreserveWhitespace = true };
			transitFile.Load(transitFilePath);

			return transitFile.SelectNodes("//Seg")
				.Cast<XmlNode>()
				.Select(segment => new TransitSegmentData(segment.SelectSingleNode("./@Data")?.InnerText));
		}
	}
}
