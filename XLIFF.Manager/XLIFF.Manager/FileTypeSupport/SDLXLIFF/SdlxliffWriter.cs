using System.Collections.Generic;
using Sdl.Community.XLIFF.Manager.FileTypeSupport.XLIFF.Model;
using Sdl.Community.XLIFF.Manager.Model;
using Sdl.FileTypeSupport.Framework.IntegrationApi;

namespace Sdl.Community.XLIFF.Manager.FileTypeSupport.SDLXLIFF
{
	public class SdlxliffWriter
	{
		private readonly SegmentBuilder _segmentBuilder;
		private readonly IFileTypeManager _fileTypeManager;
		private readonly List<string> _excludeFilterItems;
		private readonly ImportOptions _importOptions;
		private readonly List<AnalysisBand> _analysisBands;

		public SdlxliffWriter(IFileTypeManager fileTypeManager, SegmentBuilder segmentBuilder,
			List<string> excludeFilterItems, ImportOptions importOptions, List<AnalysisBand> analysisBands)
		{
			_fileTypeManager = fileTypeManager;
			_segmentBuilder = segmentBuilder;
			_excludeFilterItems = excludeFilterItems;
			_importOptions = importOptions;
			_analysisBands = analysisBands;
		}

		public bool UpdateFile(Xliff xliff, string filePathInput, string filePathOutput)
		{
			var converter = _fileTypeManager.GetConverterToDefaultBilingual(filePathInput, filePathOutput, null);
			var contentWriter = new ContentWriter(xliff, _segmentBuilder, _excludeFilterItems, _importOptions, _analysisBands);

			converter.AddBilingualProcessor(contentWriter);
			converter.SynchronizeDocumentProperties();

			converter.Parse();

			return true;
		}
	}
}
