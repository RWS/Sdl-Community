using System;
using Sdl.Community.TMOptimizerLib;
using Sdl.Core.Globalization;
using Sdl.LanguagePlatform.TranslationMemoryApi;

namespace Sdl.Community.TMOptimizer.Control
{
    /// <summary>
    /// Export a Studio TM to TMX
    /// </summary>
    class ExportStudioTmStep : ProcessingStep
    {
        private FileBasedTranslationMemory _translationMemory;
        private TmxFile _exportTmxFile;

        public ExportStudioTmStep(FileBasedTranslationMemory translationMemory, TmxFile exportTmxFile)
            : base(String.Format("Exporting translation memory {0}", translationMemory.Name))
        {
            _translationMemory = translationMemory;
            _exportTmxFile = exportTmxFile;
        }

        protected override void ExecuteImpl()
        {
            TranslationMemoryExporter exporter = new TranslationMemoryExporter(_translationMemory.LanguageDirection);
            int tuCount = _translationMemory.LanguageDirection.GetTranslationUnitCount();
            exporter.BatchExported += (sender, e) =>
            {
                StatusMessage = String.Format("TUs: {0}/{1}", e.TotalProcessed, tuCount);
                ReportProgress((int)((100.0 * e.TotalExported) / tuCount));
            };
            exporter.TmxExportFormat = Sdl.LanguagePlatform.TranslationMemory.TranslationUnitFormat.SDLTradosStudio2009;
            
            exporter.Export(_exportTmxFile.FilePath, true);

            _exportTmxFile.DetectInfo = new DetectInfo
            {
                DetectedVersion = DetectInfo.Versions.Studio,
                SourceLanguage = new Language(_translationMemory.LanguageDirection.SourceLanguage),
                TargetLanguage = new Language(_translationMemory.LanguageDirection.TargetLanguage),
                OriginalSourceLanguage = _translationMemory.LanguageDirection.SourceLanguage.Name,
                OriginalTargetLanguage = _translationMemory.LanguageDirection.TargetLanguage.Name,
                TuCount = tuCount
            };
        }
    }
}
