using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicrosoftTranslatorProvider.Interface
{
    public interface ITranslationProviderExtension
    {
        Dictionary<string, string> LanguagesSupported { get; set; }
    }
}
