using Sdl.Community.XLIFF.Manager.Common;
using Sdl.Community.XLIFF.Manager.FileTypeSupport.SDLXLIFF;
using Sdl.Community.XLIFF.Manager.FileTypeSupport.XLIFF.Model;
using Sdl.Community.XLIFF.Manager.FileTypeSupport.XLIFF.Readers;
using XLIFF.Manager.UnitTests.Common;
using Xunit;

namespace XLIFF.Manager.UnitTests.FileTypeSupport
{
	public class XliffReaderTests
	{
		private readonly TestFilesUtil _testFilesUtil;

		public XliffReaderTests()
		{
			_testFilesUtil = new TestFilesUtil();
		}

		[Theory(Skip = "TODO: Update Unit-tests to reflect latest changes")]
		[InlineData(Enumerators.XLIFFSupport.xliff12polyglot)]
		[InlineData(Enumerators.XLIFFSupport.xliff12sdl)]
		public void XliffReader_XLIFFSupportSniffer_ReturnsEqual(Enumerators.XLIFFSupport support)
		{
			// arrange
			var sniffer = new XliffSniffer();
			var segmentBuilder = new SegmentBuilder();
			var xliffReader = new XliffReder(sniffer, segmentBuilder);
			var testFile = support == Enumerators.XLIFFSupport.xliff12polyglot
				? _testFilesUtil.GetSampleFilePath("Xliff12", "Polyglot", "DefaultSample.sdlxliff.xliff")
				: _testFilesUtil.GetSampleFilePath("Xliff12", "xsi", "DefaultSample.sdlxliff.xliff");

			// act
			var reader = xliffReader.ReadXliff(testFile);

			// assert
			Assert.Equal(support, reader.Support);
		}


		[Theory(Skip = "TODO: Update Unit-tests to reflect latest changes")]
		[InlineData(Enumerators.XLIFFSupport.xliff12polyglot)]
		[InlineData(Enumerators.XLIFFSupport.xliff12sdl)]
		public void XliffReader_ReadLockedContent_ReturnsEqual(Enumerators.XLIFFSupport support)
		{
			// arrange
			var sniffer = new XliffSniffer();
			var segmentBuilder = new SegmentBuilder();
			var xliffReader = new XliffReder(sniffer, segmentBuilder);
			var testFile = support == Enumerators.XLIFFSupport.xliff12polyglot
				? _testFilesUtil.GetSampleFilePath("Xliff12", "Polyglot", "LockedContentSample.sdlxliff.xliff")
				: _testFilesUtil.GetSampleFilePath("Xliff12", "xsi", "LockedContentSample.sdlxliff.xliff");

			// act
			var reader = xliffReader.ReadXliff(testFile);
			var segmentPair = reader.Files[0].Body.TransUnits[0].SegmentPairs[0];
			var openElementLocked = segmentPair.Target.Elements[1];
			var closeElementLocked = segmentPair.Target.Elements[9];

			// assert
			Assert.Equal(11, segmentPair.Target.Elements.Count);
			Assert.True(openElementLocked is ElementLocked openElement && openElement.Type == Element.TagType.OpeningTag);
			Assert.True(closeElementLocked is ElementLocked closeElement && closeElement.Type == Element.TagType.ClosingTag);
		}
	}
}
