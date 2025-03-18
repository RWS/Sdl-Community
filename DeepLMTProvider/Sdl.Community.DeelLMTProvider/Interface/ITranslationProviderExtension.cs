using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sdl.Community.DeepLMTProvider.Interface
{
    public interface ITranslationProviderExtension
    {
        public Dictionary<string, string> LanguagesSupported { get; set; }
    }
}
