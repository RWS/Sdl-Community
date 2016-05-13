using System;
using System.IO;
using Sdl.FileTypeSupport.Framework.IntegrationApi;
using Sdl.FileTypeSupport.Framework.Core.Utilities.IntegrationApi;
using Sdl.LanguagePlatform.TranslationMemoryApi;
using Sdl.LanguagePlatform.TranslationMemory;
using Sdl.ProjectAutomation.Core;
using System.Globalization;
using Sdl.LanguagePlatform.Core.Tokenization;

namespace Sdl.Community.StarTransit.Shared.Import
{
    public class TransitTmImporter
    {
        private readonly IFileTypeManager _fileTypeManager;
        private readonly CultureInfo _sourceCulture;
        private readonly CultureInfo _targetCulture;
        private readonly bool _createTm;

        public TransitTmImporter(CultureInfo sourceCulture,
            CultureInfo targetCulture,
            bool createTm,
            IFileTypeManager fileTypeManager)
        {
            _sourceCulture = sourceCulture;
            _targetCulture = targetCulture;
            _createTm = createTm;
            _fileTypeManager = fileTypeManager;
        }

        public TranslationProviderReference ImportStarTransitTM(string starTransitTM,string studioTranslationMemory)
        {
            //starTransitTMs.Substring(0, starTransitTMs.LastIndexOf(@"\", StringComparison.Ordinal))
            //GetTmName(starTransitTM)

            string sdlXliffFullPath = CreateTemporarySdlXliff(starTransitTM);

            ImportSdlXliffIntoTM(studioTranslationMemory, sdlXliffFullPath);
            return new TranslationProviderReference(studioTranslationMemory, true);
        }

        private void ImportSdlXliffIntoTM(string studioTranslationMemory, string sdlXliffFullPath)
        {
            FileBasedTranslationMemory fileBasedTM;
            if (_createTm)
            {


                fileBasedTM = new FileBasedTranslationMemory(studioTranslationMemory,
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
                fileBasedTM = new FileBasedTranslationMemory(studioTranslationMemory);
            }

            var tmImporter = new TranslationMemoryImporter(fileBasedTM.LanguageDirection);
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
