using System.Collections.Generic;
using System.Globalization;
using Sdl.FileTypeSupport.Framework.IntegrationApi;
using Trados.Transcreate.FileTypeSupport.MSOffice.Visitors;
using Trados.Transcreate.FileTypeSupport.MSOffice.Writers;
using Trados.Transcreate.FileTypeSupport.SDLXLIFF;
using Trados.Transcreate.Model;

namespace Trados.Transcreate.FileTypeSupport.MSOffice
{
	public class ImportProcessor
	{
		private readonly ImportOptions _settings;
		private readonly List<AnalysisBand> _analysisBands;
		private readonly TokenVisitor _tokenVisitor;
		private readonly IFileTypeManager _fileTypeManager;
		private readonly SegmentBuilder _segmentBuilder;

		public ImportProcessor(IFileTypeManager fileTypeManager, TokenVisitor tokenVisitor, 
			ImportOptions settings, List<AnalysisBand> analysisBands, SegmentBuilder segmentBuilder)
		{
			_fileTypeManager = fileTypeManager;
			_analysisBands = analysisBands;
			_settings = settings;
			_segmentBuilder = segmentBuilder;

			_tokenVisitor = tokenVisitor;
		}

		public ConfirmationStatistics ConfirmationStatistics { get; private set; }

		public TranslationOriginStatistics TranslationOriginStatistics { get; private set; }

		public CultureInfo SourceLanguage { get; private set; }

		public CultureInfo TargetLanguage { get; private set; }


		/// <summary>
		/// Imports updated DOCX file into the SDLXLIFF file
		/// </summary>
		/// <param name="filePathInput">Original SDLXLIFF file</param>
		/// <param name="filePathOutput">Output SDLXLIFF file</param>
		/// <param name="filePathUpdated">Updated DOCX file</param>		
		public bool ImportUpdatedFile(string filePathInput, string filePathOutput, string filePathUpdated, string targetLanguage)
		{
			var contentWriter = new ContentWriter(_settings, _analysisBands, filePathUpdated, targetLanguage, _segmentBuilder);
			var converter = _fileTypeManager.GetConverterToDefaultBilingual(filePathInput, filePathOutput, null);
			converter.AddBilingualProcessor(contentWriter);

			converter.Parse();

			SourceLanguage = contentWriter.SourceLanguage;
			TargetLanguage = contentWriter.TargetLanguage;

			ConfirmationStatistics = contentWriter.ConfirmationStatistics;
			TranslationOriginStatistics = contentWriter.TranslationOriginStatistics;

			return true;
		}
	}
}
