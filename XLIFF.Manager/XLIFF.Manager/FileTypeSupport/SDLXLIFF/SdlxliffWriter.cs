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
		private readonly ImportOptions _importOptions;
		private readonly List<AnalysisBand> _analysisBands;

		public SdlxliffWriter(IFileTypeManager fileTypeManager, SegmentBuilder segmentBuilder,
			ImportOptions importOptions, List<AnalysisBand> analysisBands)
		{
			_fileTypeManager = fileTypeManager;
			_segmentBuilder = segmentBuilder;
			_importOptions = importOptions;
			_analysisBands = analysisBands;
			ConfirmationStatistics = new ConfirmationStatistics();
			TranslationOriginStatistics = new TranslationOriginStatistics();
		}

		public ConfirmationStatistics ConfirmationStatistics { get; private set; }

		public TranslationOriginStatistics TranslationOriginStatistics { get; private set; }

		public bool UpdateFile(Xliff xliff, string filePathInput, string filePathOutput)
		{
			var converter = _fileTypeManager.GetConverterToDefaultBilingual(filePathInput, filePathOutput, null);
			var contentWriter = new ContentWriter(xliff, _segmentBuilder, _importOptions, _analysisBands);

			converter.AddBilingualProcessor(contentWriter);
			converter.SynchronizeDocumentProperties();

			converter.Parse();

			ConfirmationStatistics = contentWriter.ConfirmationStatistics;
			TranslationOriginStatistics = contentWriter.TranslationOriginStatistics;

			return true;
		}
	}
}
