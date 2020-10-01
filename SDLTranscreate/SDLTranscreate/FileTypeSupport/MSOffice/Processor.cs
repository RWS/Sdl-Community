using System.Collections.Generic;
using System.Globalization;
using Sdl.Community.Transcreate.FileTypeSupport.MSOffice.Model;
using Sdl.Community.Transcreate.FileTypeSupport.MSOffice.Readers;
using Sdl.Community.Transcreate.FileTypeSupport.MSOffice.Visitors;
using Sdl.Community.Transcreate.FileTypeSupport.MSOffice.Writers;
using Sdl.Community.Transcreate.Model;
using Sdl.FileTypeSupport.Framework.IntegrationApi;

namespace Sdl.Community.Transcreate.FileTypeSupport.MSOffice
{
	public class Processor
	{
		private readonly GeneratorSettings _settings;
		private readonly List<AnalysisBand> _analysisBands;
		private readonly TokenVisitor _tokenVisitor;
		private readonly IFileTypeManager _fileTypeManager;

		public Processor(IFileTypeManager fileTypeManager, TokenVisitor tokenVisitor, 
			GeneratorSettings settings, List<AnalysisBand> analysisBands)
		{
			_fileTypeManager = fileTypeManager;
			_analysisBands = analysisBands;
			_settings = settings;

			_tokenVisitor = tokenVisitor;
			_tokenVisitor.Settings = settings;
		}

		public ConfirmationStatistics ConfirmationStatistics { get; private set; }

		public TranslationOriginStatistics TranslationOriginStatistics { get; private set; }

		public CultureInfo SourceLanguage { get; private set; }

		public CultureInfo TargetLanguage { get; private set; }

		/// <summary>
		/// Run export on SDLXLIFF path
		/// </summary>
		public bool ExportFile(string filePath)
		{
			var contentReader = new ContentReader(_tokenVisitor, _settings, _analysisBands, filePath);
			var converter = _fileTypeManager.GetConverterToDefaultBilingual(filePath, null, null);
			converter.AddBilingualProcessor(contentReader);

			converter.Parse();

			SourceLanguage = contentReader.SourceLanguage;
			TargetLanguage = contentReader.TargetLanguage;

			ConfirmationStatistics = contentReader.ConfirmationStatistics;
			TranslationOriginStatistics = contentReader.TranslationOriginStatistics;

			return true;
		}

		/// <summary>
		/// Imports updated DOCX file into the SDLXLIFF file
		/// </summary>
		/// <param name="filePathInput">Original SDLXLIFF file</param>
		/// <param name="filePathOutput">Output SDLXLIFF file</param>
		/// <param name="filePathUpated">Updated DOCX file</param>		
		public bool ImportUpdatedFile(string filePathInput, string filePathOutput, string filePathUpated)
		{
			var contentWriter = new ContentWriter(_settings, _analysisBands, filePathUpated);
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
