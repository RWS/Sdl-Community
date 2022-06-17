using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Sdl.FileTypeSupport.Framework.Core.Utilities.IntegrationApi;
using Trados.Transcreate.FileTypeSupport.XLIFF.Model;
using Trados.Transcreate.Model;
using File = System.IO.File;

namespace Trados.Transcreate.FileTypeSupport.SDLXLIFF
{
	public class SdlxliffReader
	{
		private readonly SegmentBuilder _segmentBuilder;
		private readonly ExportOptions _exportOptions;
		private readonly List<AnalysisBand> _analysisBands;

		public SdlxliffReader(SegmentBuilder segmentBuilder, ExportOptions exportOptions, List<AnalysisBand> analysisBands)
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

		public Xliff ReadFile(string projectId, string documentId, string filePath, string targetLanguage)
		{
			var fileTypeManager = DefaultFileTypeManager.CreateInstance(true);
			var tempFile = Path.GetTempFileName();
			var converter = fileTypeManager.GetConverterToDefaultBilingual(filePath, tempFile, null);

			var contentReader = new XliffContentReader(projectId, documentId, filePath, targetLanguage, false, _segmentBuilder,
				_exportOptions, _analysisBands);
			//converter.AddBilingualProcessor(new SegmentRenumberingBilingualProcessor());
			converter.AddBilingualProcessor(contentReader);

			converter.Parse();

			SourceLanguage = contentReader.SourceLanguage;
			TargetLanguage = contentReader.TargetLanguage;

			ConfirmationStatistics = contentReader.ConfirmationStatistics;
			TranslationOriginStatistics = contentReader.TranslationOriginStatistics;


			if (File.Exists(tempFile))
			{
				File.Delete(tempFile);
			}

			return contentReader.Xliff;
		}
	}
}
