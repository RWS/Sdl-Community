using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sdl.LanguagePlatform.TranslationMemory;
using Sdl.LanguagePlatform.TranslationMemoryApi;

namespace Sdl.Community.ReindexTms.Processors
{
    class TmImporter
    {

        public ImportStatistics Results { get; private set; }
        private int TotalUnits { get; set; }

        public event EventHandler<ProgressEventArgs> OnProgressChanged;

        private readonly FileBasedTranslationMemory _tm;

        public TmImporter(FileBasedTranslationMemory tm, int totalUnits)
        {
            _tm = tm;
            Results = null;
            TotalUnits = totalUnits;
        }

        public async Task<ImportStatistics> Import(string tmxPath)
        {
            var importer = new TranslationMemoryImporter(_tm.LanguageDirection);


            importer.BatchImported += importer_BatchImported;

            GetImportSettings(importer.ImportSettings);


            var t = new Task(() => importer.Import(tmxPath));



#pragma warning disable 4014
            t.ContinueWith(task =>
#pragma warning restore 4014
            {
                if (task.IsFaulted)
                    throw new Exception(ProcessorUtil.ExceptionToMsg(task.Exception));
            }, TaskScheduler.FromCurrentSynchronizationContext());

            t.Start();
            await t;

            Results = importer.Statistics;
            return Results;
        }

        private static void GetImportSettings(ImportSettings importSettings)
        {
            importSettings.CheckMatchingSublanguages = true;
            importSettings.ExistingFieldsUpdateMode = ImportSettings.FieldUpdateMode.Merge;
            importSettings.ExistingTUsUpdateMode = ImportSettings.TUUpdateMode.Overwrite;
        }

        private void importer_BatchImported(object sender, BatchImportedEventArgs e)
        {
            if (OnProgressChanged != null)
                OnProgressChanged(this, new ProgressEventArgs
                {
                    Type = ProgressEventArgs.ProcessorType.TmImporter,
                    Description = string.Empty,
                    CurrentProgress = e.Statistics.TotalImported,
                    TotalUnits = TotalUnits
                });
        }
    }
}
