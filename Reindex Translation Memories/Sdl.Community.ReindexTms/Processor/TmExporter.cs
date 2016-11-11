using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sdl.LanguagePlatform.TranslationMemoryApi;

namespace Sdl.Community.ReindexTms.Processor
{
    public class TmExporter
    {
        private readonly FileBasedTranslationMemory _tm;

        public TmExporter(FileBasedTranslationMemory tm)
        {
            _tm = tm;
        }

        public async Task<string> Export()
        {
            var exporter = new TranslationMemoryExporter(_tm.LanguageDirection);

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
  
            t.Start();
            await t;

            return exportFullPath;
        }
    }
}
