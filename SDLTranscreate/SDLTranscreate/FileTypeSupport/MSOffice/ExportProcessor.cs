using System.Collections.Generic;
using System.Globalization;
using Sdl.FileTypeSupport.Framework.IntegrationApi;
using Trados.Transcreate.FileTypeSupport.MSOffice.Readers;
using Trados.Transcreate.FileTypeSupport.MSOffice.Visitors;
using Trados.Transcreate.Model;

namespace Trados.Transcreate.FileTypeSupport.MSOffice
{
	public class ExportProcessor
	{
		private readonly string _projectId;
		private readonly ExportOptions _settings;
		private readonly List<AnalysisBand> _analysisBands;
		private readonly TokenVisitor _tokenVisitor;
		private readonly IFileTypeManager _fileTypeManager;

		public ExportProcessor(string projectId, IFileTypeManager fileTypeManager, TokenVisitor tokenVisitor,
			ExportOptions settings, List<AnalysisBand> analysisBands)
		{
			_projectId = projectId;
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
		/// Run export on SDLXLIFF path
		/// </summary>
		public bool ExportFile(string filePath, string outputFilePath, string targetLanguage)
		{
			var contentReader = new ContentReader(_projectId, _tokenVisitor, _settings, _analysisBands, filePath, outputFilePath, targetLanguage);
			var converter = _fileTypeManager.GetConverterToDefaultBilingual(filePath, null, null);
			converter.AddBilingualProcessor(contentReader);

			converter.Parse();

			SourceLanguage = contentReader.SourceLanguage;
			TargetLanguage = contentReader.TargetLanguage;

			ConfirmationStatistics = contentReader.ConfirmationStatistics;
			TranslationOriginStatistics = contentReader.TranslationOriginStatistics;

			return true;
		}
	}
}
