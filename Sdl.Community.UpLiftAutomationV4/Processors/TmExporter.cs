using Sdl.LanguagePlatform.TranslationMemoryApi;
using System;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;

namespace Sdl.Community.FragmentAlignmentAutomation.Processors
{
    public class TmExporter
    {
        public int TotalExported { get; private set; }
        public int TotalProcessed { get; private set; }
        private int TotalUnits { get; set; }

        public event EventHandler<ProgressEventArgs> OnProgressChanged;

        private readonly FileBasedTranslationMemory _tm;

        public TmExporter(FileBasedTranslationMemory tm, int totalUnits)
        {
            _tm = tm;
            TotalExported = 0;
            TotalProcessed = 0;
            TotalUnits = totalUnits;
        }
        public async Task<string> Export()
        {
            TotalExported = 0;

            var exporter = new TranslationMemoryExporter(_tm.LanguageDirection);
            TotalUnits = exporter.TranslationMemoryLanguageDirection.GetTranslationUnitCount();

            exporter.BatchExported += exporter_BatchExported;
            var exportName = string.Format(
                CultureInfo.CurrentCulture,
                "{0}_{1}_{2}.tmx",
                _tm.Name,
                _tm.LanguageDirection.SourceLanguage,
                _tm.LanguageDirection.TargetLanguage);

            var exportFullPath = Path.Combine(Path.GetDirectoryName(_tm.FilePath), exportName);

            if (File.Exists(exportFullPath))
                File.Delete(exportFullPath);


            var t = new Task(() => exporter.Export(exportFullPath, true));

            t.ContinueWith(task =>
            {
                if (task.IsFaulted)
                    throw new Exception(ProcessorUtil.ExceptionToMsg(task.Exception));
            }, TaskScheduler.FromCurrentSynchronizationContext());

            t.Start();
            await t;

            return exportFullPath;
        }

        private void exporter_BatchExported(object sender, BatchExportedEventArgs e)
        {
            TotalExported = e.TotalExported;
            TotalProcessed = e.TotalProcessed;

            if (OnProgressChanged != null)
            {
                OnProgressChanged.Invoke(this, new ProgressEventArgs
                {
                    Type = ProgressEventArgs.ProcessorType.TmExporter,
                    Description = string.Empty,
                    CurrentProgress = e.TotalExported,
                    TotalUnits = TotalUnits
                });
            }            
        }
    }
}
