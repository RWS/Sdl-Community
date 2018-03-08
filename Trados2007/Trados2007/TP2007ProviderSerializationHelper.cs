using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sdl.TranslationStudio.Plugins.Trados2007
{
    public class TP2007ProviderSerializationHelper
    {
        public String Type { get; set; }

        public TP2007ProviderSerializationHelper()
        {
        }

        public TP2007ProviderSerializationHelper(AbstractTrados2007TranslationProvider tm)
        {
            Type = tm.GetType().ToString();
        }
    }
}
