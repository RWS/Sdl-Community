using Sdl.Community.XLIFF.Manager.Common;
using Sdl.Community.XLIFF.Manager.FileTypeSupport.SDLXLIFF;
using Sdl.Community.XLIFF.Manager.FileTypeSupport.XLIFF.Readers;
using Sdl.FileTypeSupport.Framework.Core.Utilities.BilingualApi;
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

		[Theory]
		[InlineData(Enumerators.XLIFFSupport.xliff12polyglot)]
		[InlineData(Enumerators.XLIFFSupport.xliff12sdl)]
		public void XliffReader_XLIFFSupportSniffer_ReturnsEqual(Enumerators.XLIFFSupport support)
		{
			// arrange
			var sniffer = new XliffSupportSniffer();
			var segmentBuilder = new SegmentBuilder();
			var xliffReader = new XliffReder(sniffer, segmentBuilder);
			var testFile = support == Enumerators.XLIFFSupport.xliff12polyglot
				? _testFilesUtil.TestFilePolyglotXliff12
				: _testFilesUtil.TestFileSdlXliff12;

			// act
			var reader = xliffReader.ReadXliff(testFile);

			// assert
			Assert.Equal(support, reader.Support);
		}


		[Theory]
		[InlineData(Enumerators.XLIFFSupport.xliff12polyglot)]
		//[InlineData(Enumerators.XLIFFSupport.xliff12sdl)]
		public void XliffReader_ReadXliffTESTTEST_TODO_ReturnsEqual(Enumerators.XLIFFSupport support)
		{
			// arrange
			var sniffer = new XliffSupportSniffer();
			var segmentBuilder = new SegmentBuilder();
			var xliffReader = new XliffReder(sniffer, segmentBuilder);
			var testFile = support == Enumerators.XLIFFSupport.xliff12polyglot
				? _testFilesUtil.TestFilePolyglotXliff12
				: _testFilesUtil.TestFileSdlXliff12;

			// act
			var reader = xliffReader.ReadXliff(testFile);

			// assert
			Assert.Equal(support, reader.Support);
		}
	}
}
