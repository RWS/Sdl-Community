using System;
using System.Globalization;
using System.IO;
using Sdl.FileTypeSupport.Framework.IntegrationApi;
using Sdl.LanguagePlatform.Core.Tokenization;
using Sdl.LanguagePlatform.TranslationMemory;
using Sdl.LanguagePlatform.TranslationMemoryApi;
using Sdl.ProjectAutomation.Core;

namespace Sdl.Community.StarTransit.Shared.Import
{
	public class TransitTmImporter
    {
        private readonly IFileTypeManager _fileTypeManager;
        private readonly CultureInfo _sourceCulture;
        private readonly CultureInfo _targetCulture;
        private readonly bool _createTm;
        private FileBasedTranslationMemory _fileBasedTM;

        public TransitTmImporter(CultureInfo sourceCulture,
            CultureInfo targetCulture,
            bool createTm,
            IFileTypeManager fileTypeManager,
            string studioTranslationMemory)
        {
            _sourceCulture = sourceCulture;
            _targetCulture = targetCulture;
            _createTm = createTm;
            _fileTypeManager = fileTypeManager;

            if (_createTm)
            {				
                _fileBasedTM = new FileBasedTranslationMemory(studioTranslationMemory,
                    string.Empty,
                    _sourceCulture,
                    _targetCulture,
                    FuzzyIndexes.SourceWordBased | FuzzyIndexes.SourceCharacterBased | FuzzyIndexes.TargetCharacterBased | FuzzyIndexes.TargetWordBased,
                     BuiltinRecognizers.RecognizeAll,
                     TokenizerFlags.DefaultFlags,
                       WordCountFlags.BreakOnTag | WordCountFlags.BreakOnDash | WordCountFlags.BreakOnApostrophe);
            }
            else
            {
                _fileBasedTM = new FileBasedTranslationMemory(studioTranslationMemory);
            }
        }

        public void ImportStarTransitTm(string starTransitTm)
        {
            string sdlXliffFullPath = CreateTemporarySdlXliff(starTransitTm);

            ImportSdlXliffIntoTm(sdlXliffFullPath);            
        }

        private void ImportSdlXliffIntoTm( string sdlXliffFullPath)
        {
            var tmImporter = new TranslationMemoryImporter(_fileBasedTM.LanguageDirection);
            var importSettings = new ImportSettings()
            {
                IsDocumentImport = false,
                CheckMatchingSublanguages = false,
                IncrementUsageCount = false,
                NewFields = ImportSettings.NewFieldsOption.Ignore,
                PlainText = false,
                ExistingTUsUpdateMode = ImportSettings.TUUpdateMode.AddNew
            };
            tmImporter.ImportSettings = importSettings;

            tmImporter.Import(sdlXliffFullPath);
        }

        public TranslationProviderReference GeTranslationProviderReference()
        {
            return new TranslationProviderReference(_fileBasedTM.FilePath,true);
        }

        /// <summary>
        /// Create temporary bilingual file (sdlxliff) used to import the information
        /// in Studio translation memories
        /// </summary>
        /// <param name="starTransitTM"></param>
        /// <param name="pathToExtractFolder"></param>
        /// <returns></returns>
        private string CreateTemporarySdlXliff(string starTransitTM)
        {
            var pathToExtractFolder = CreateFolderToExtract(Path.GetDirectoryName(starTransitTM));

            var generatedXliffName = string.Format("{0}{1}",
                Path.GetFileNameWithoutExtension(starTransitTM), ".sdlxliff");

            var sdlXliffFullPath = Path.Combine(pathToExtractFolder, generatedXliffName);

            var converter = _fileTypeManager.GetConverterToDefaultBilingual(starTransitTM,
                sdlXliffFullPath,
                null);

            converter.Parse();
            return sdlXliffFullPath;
        }

        /// <summary>
        /// Create temporary folder for TM import
        /// </summary>
        /// <param name="pathToTemp"></param>
        /// <returns></returns>
        private string CreateFolderToExtract(string pathToTemp)
        {
            var pathToExtractFolder = Path.Combine(pathToTemp, "TmExtract");
            if (!Directory.Exists(pathToExtractFolder))
            {
                Directory.CreateDirectory(pathToExtractFolder);
            }

            return pathToExtractFolder;
        }

        private string GetTemporarySdlXliffPath(string tmFilePath)
        {
            var intermediateName =
               tmFilePath.Substring(tmFilePath.LastIndexOf(@"\", StringComparison.Ordinal) + 1);

            var tmName = intermediateName.Substring(0, intermediateName.LastIndexOf(".", StringComparison.Ordinal));

            return tmName;
        }
    }
}
