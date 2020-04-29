using System;
using Sdl.Community.TMOptimizerLib;
using Sdl.LanguagePlatform.TranslationMemory;
using Sdl.LanguagePlatform.TranslationMemoryApi;

namespace Sdl.Community.TMOptimizer
{
	/// <summary>
	/// Imports a TMX file into a Studio TM
	/// </summary>
	class ImportStudioTmStep : ProcessingStep
    {
        private TranslationMemoryReference _outputTranslationMemory;
        private string _name;
        private TmxFile _tmxFile;

        public ImportStudioTmStep(TranslationMemoryReference outputTranslationMemory, string name, TmxFile tmxFile)
            : base(String.Format("Import {0}", name))
        {
            _outputTranslationMemory = outputTranslationMemory;
            _name = name;
            _tmxFile = tmxFile;
        }

        protected override void ExecuteImpl()
        {
            FileBasedTranslationMemory tm = _outputTranslationMemory.TranslationMemory;
            TranslationMemoryImporter importer = new TranslationMemoryImporter(tm.LanguageDirection);
            importer.BatchImported += (sender, e) => 
            {
                StatusMessage = String.Format("TUs: {0}/{1}", e.Statistics.TotalRead, _tmxFile.GetDetectInfo().TuCount);
                ReportProgress((int)((100.0 * e.Statistics.RawTUs) / _tmxFile.GetDetectInfo().TuCount));
            };
            importer.ImportSettings = new ImportSettings
            {
                CheckMatchingSublanguages = false,
                ExistingFieldsUpdateMode = ImportSettings.FieldUpdateMode.Merge,
                NewFields = ImportSettings.NewFieldsOption.AddToSetup,
                OverwriteExistingTUs = false,
                TUProcessingMode = ImportSettings.ImportTUProcessingMode.ProcessCleanedTUOnly
            };

            importer.Import(_tmxFile.FilePath);
        }
    }
}