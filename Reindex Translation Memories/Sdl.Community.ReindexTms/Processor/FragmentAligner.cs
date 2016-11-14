using System;
using System.CodeDom;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sdl.LanguagePlatform.TranslationMemory;
using Sdl.LanguagePlatform.TranslationMemoryApi;

namespace Sdl.Community.ReindexTms.Processor
{
    public class FragmentAligner
    {
        private readonly FileBasedTranslationMemory _tm;

        public FragmentAligner(FileBasedTranslationMemory tm)
        {
            _tm = tm;
        }

        public void AlignTranslationUnits()
        {
            var iterator = new RegularIterator();
            _tm.AlignTranslationUnits(false, true, ref iterator);

        }
    }
}
