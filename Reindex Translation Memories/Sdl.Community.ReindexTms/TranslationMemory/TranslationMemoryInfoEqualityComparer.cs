using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sdl.Community.ReindexTms.TranslationMemory
{
    public class TranslationMemoryInfoEqualityComparer: IEqualityComparer<TranslationMemoryInfo>
    {
        public bool Equals(TranslationMemoryInfo x, TranslationMemoryInfo y)
        {
            return x.FilePath.Equals(y.FilePath);
        }

        public int GetHashCode(TranslationMemoryInfo obj)
        {
            return obj.GetHashCode();
        }
    }
}
