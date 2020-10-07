using System.Collections.Generic;
using System.Globalization;
using Sdl.Community.Transcreate.FileTypeSupport.MSOffice.Visitors;
using Sdl.Community.Transcreate.FileTypeSupport.MSOffice.Writers;
using Sdl.Community.Transcreate.Model;
using Sdl.FileTypeSupport.Framework.IntegrationApi;

namespace Sdl.Community.Transcreate.FileTypeSupport.MSOffice
{
	public class ImportProcessor
	{
		private readonly ImportOptions _settings;
		private readonly List<AnalysisBand> _analysisBands;
		private readonly TokenVisitor _tokenVisitor;
		private readonly IFileTypeManager _fileTypeManager;

		public ImportProcessor(IFileTypeManager fileTypeManager, TokenVisitor tokenVisitor, 
			ImportOptions settings, List<AnalysisBand> analysisBands)
		{
			_fileTypeManager = fileTypeManager;
			_analysisBands = analysisBands;
			_settings = settings;

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
			var contentWriter = new ContentWriter(_settings, _analysisBands, filePathUpdated, targetLanguage);
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
