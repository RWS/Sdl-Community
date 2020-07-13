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

		[Theory(Skip = "TODO: Update Unit-tests to reflect latest changes")]
		[InlineData(Enumerators.XLIFFSupport.xliff12polyglot)]
		[InlineData(Enumerators.XLIFFSupport.xliff12sdl)]
		public void XliffReader_XLIFFSupportSniffer_ReturnsEqual(Enumerators.XLIFFSupport support)
		{
			// arrange
			var sniffer = new XliffSniffer();
			var segmentBuilder = new SegmentBuilder();
			var xliffReader = new XliffReder(sniffer, segmentBuilder);
			//var pocoFilterManager = new PocoFilterManager(false);
			var fileTypeManager = DefaultFileTypeManager.CreateInstance(true);			
			var importOptions = new ImportOptions();
			var analysisBands = new List<AnalysisBand>();
			var sdlxliffWriter = new SdlxliffWriter(fileTypeManager, segmentBuilder, importOptions, analysisBands);

			var testFile = support == Enumerators.XLIFFSupport.xliff12polyglot
				? _testFilesUtil.GetSampleFilePath("Xliff12", "Polyglot", "QuotesSample.docx.sdlxliff.xliff")
				: _testFilesUtil.GetSampleFilePath("Xliff12", "xsi", "QuotesSample.docx.sdlxliff.xliff");
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
