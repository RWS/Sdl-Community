using System.Collections.Generic;
using System.Globalization;
using Sdl.Community.XLIFF.Manager.FileTypeSupport.XLIFF.Model;
using Sdl.Community.XLIFF.Manager.Model;
using Sdl.FileTypeSupport.Framework.Core.Utilities.IntegrationApi;
using Sdl.FileTypeSupport.Framework.IntegrationApi;

namespace Sdl.Community.XLIFF.Manager.FileTypeSupport.SDLXLIFF
{
	public class SdlxliffReader
	{
		private readonly SegmentBuilder _segmentBuilder;
		private readonly ExportOptions _exportOptions;
		private readonly List<AnalysisBand> _analysisBands;
		private readonly IMultiFileConverter _multiFileConverter;

		public SdlxliffReader(SegmentBuilder segmentBuilder,  
			ExportOptions exportOptions, List<AnalysisBand> analysisBands)
		{
			_segmentBuilder = segmentBuilder;
			_exportOptions = exportOptions;
			_analysisBands = analysisBands;
			ConfirmationStatistics = new ConfirmationStatistics();
			TranslationOriginStatistics = new TranslationOriginStatistics();
		}

		public ConfirmationStatistics ConfirmationStatistics { get; private set; }

		public TranslationOriginStatistics TranslationOriginStatistics { get; private set; }

		public CultureInfo SourceLanguage { get; private set; }

		public CultureInfo TargetLanguage { get; private set; }

		public Xliff ReadFile(string projectId, string filePath)
		{
			var fileTypeManager = DefaultFileTypeManager.CreateInstance(true);
			var converter = fileTypeManager.GetConverterToDefaultBilingual(filePath, null, null);

			var contentReader = new ContentReader(projectId, filePath, false, _segmentBuilder, 
				_exportOptions, _analysisBands);		
			converter.AddBilingualProcessor(contentReader);
			
			SourceLanguage = contentReader.SourceLanguage;
			TargetLanguage = contentReader.TargetLanguage;			

			converter.Parse();

			ConfirmationStatistics = contentReader.ConfirmationStatistics;
			TranslationOriginStatistics = contentReader.TranslationOriginStatistics;

			return contentReader.Xliff;
		}
	}
}
