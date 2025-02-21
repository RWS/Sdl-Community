using Sdl.LanguagePlatform.TranslationMemory;
using Sdl.LanguagePlatform.TranslationMemoryApi;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace Sdl.Community.TMLifting.Processor
{
    public class TmImporter
    {
        private readonly FileBasedTranslationMemory _tm;
		public ReadOnlyCollection<ServerBasedTranslationMemory> _stm;

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