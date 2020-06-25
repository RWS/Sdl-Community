using System.Collections.Generic;
using Sdl.Community.XLIFF.Manager.Common;
using Sdl.Community.XLIFF.Manager.FileTypeSupport.SDLXLIFF;
using Sdl.Community.XLIFF.Manager.FileTypeSupport.XLIFF.Readers;
using Sdl.Community.XLIFF.Manager.Model;
using Sdl.FileTypeSupport.Framework.Core.Utilities.IntegrationApi;
using XLIFF.Manager.UnitTests.Common;
using Xunit;

namespace XLIFF.Manager.UnitTests.FileTypeSupport
{
	public class XliffWriterTests
	{
		private readonly TestFilesUtil _testFilesUtil;

		public XliffWriterTests()
		{
			_testFilesUtil = new TestFilesUtil();
		}

		[Theory]
		//[InlineData(Enumerators.XLIFFSupport.xliff12polyglot)]
		[InlineData(Enumerators.XLIFFSupport.xliff12sdl)]
		public void XliffReader_XLIFFSupportSniffer_ReturnsEqual(Enumerators.XLIFFSupport support)
		{
			// arrange
			var sniffer = new XliffSniffer();
			var segmentBuilder = new SegmentBuilder();
			var xliffReader = new XliffReder(sniffer, segmentBuilder);
			//var pocoFilterManager = new PocoFilterManager(false);
			var fileTypeManager = DefaultFileTypeManager.CreateInstance(true);
			var filterItems = new List<FilterItem>();
			var importOptions = new ImportOptions();
			var sdlxliffWriter = new SdlxliffWriter(fileTypeManager, segmentBuilder, filterItems, importOptions);

			var testFile = support == Enumerators.XLIFFSupport.xliff12polyglot
				? _testFilesUtil.GetSampleFilePath("Xliff12", "Polyglot", "QuotesSample.docx.sdlxliff.xliff")
				: _testFilesUtil.GetSampleFilePath("Xliff12", "Sdl", "QuotesSample.docx.sdlxliff.xliff");
			var sdlxliffFile = _testFilesUtil.GetSampleFilePath("Sdlxliff", "QuotesSample.docx.sdlxliff");			

			// act
			var xliff = xliffReader.ReadXliff(testFile);
			var outputFile = sdlxliffFile + ".out.sdlxliff";
			var success = sdlxliffWriter.UpdateFile(xliff, sdlxliffFile, outputFile);
			
			// assert
			Assert.Equal(support, xliff.Support);
		}	
	}
}
