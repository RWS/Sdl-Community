using Sdl.LanguagePlatform.TranslationMemoryApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sdl.Community.ReindexTms.TranslationMemory
{
    public class TranslationMemoryInfo
    {
        public Uri Uri { get; set; }

        public string Name { get; set; }

        public string FilePath { get; set; }

        public bool IsStudioTm { get; set; }

        public TranslationMemoryInfo(string tmFilePath, bool isStudioTm)
        {
            FilePath = tmFilePath;
            Uri = FileBasedTranslationMemory.GetFileBasedTranslationMemoryUri(tmFilePath);
            Name = FileBasedTranslationMemory.GetFileBasedTranslationMemoryName(Uri);
            IsStudioTm = isStudioTm;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
