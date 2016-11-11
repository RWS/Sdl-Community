using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sdl.LanguagePlatform.TranslationMemory;
using Sdl.LanguagePlatform.TranslationMemoryApi;

namespace Sdl.Community.ReindexTms.Processor
{
    public class TmImporter
    {
        private readonly FileBasedTranslationMemory _tm;

        public TmImporter(FileBasedTranslationMemory tm)
        {
            _tm = tm;
        }

        private static void GetImportSettings(ImportSettings importSettings)
        {
            importSettings.CheckMatchingSublanguages = true;
            importSettings.ExistingFieldsUpdateMode = ImportSettings.FieldUpdateMode.Merge;
            importSettings.ExistingTUsUpdateMode = ImportSettings.TUUpdateMode.Overwrite;
        }

        public async Task<ImportStatistics> Import(string tmxPath)
        {
            var importer = new TranslationMemoryImporter(_tm.LanguageDirection);

            GetImportSettings(importer.ImportSettings);


            var t = new Task(() => importer.Import(tmxPath));
            t.Start();
            await t;
            return importer.Statistics;
        }

    }
}
