using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sdl.LanguagePlatform.TranslationMemoryApi;

namespace Sdl.Community.ReindexTms.Processor
{
    public class ModelBuilder
    {
        private readonly FileBasedTranslationMemory _tm;

        public ModelBuilder(FileBasedTranslationMemory tm)
        {
            _tm = tm;
        }

        public void BuildTranslationModel()
        {
            _tm.BuildModel();
        }

    }
}
