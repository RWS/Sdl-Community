using Sdl.LanguagePlatform.TranslationMemoryApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sdl.Community.ControlledMTProviders.Provider.Model
{
    public class ProviderItem
    {
        public ITranslationProviderWinFormsUI Provider { get; set; }

        public override string ToString()
        {
            string t = Provider.TypeName.Replace("&", "");
            if (t.EndsWith("..."))
            {
                t = t.Substring(0, t.Length - 3);
            }
            return t;
        }
    }
}
