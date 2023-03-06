using System.Collections.Generic;
using System.Globalization;

namespace Sdl.Community.MTEdge.Provider.Model
{
    public class TradosToMTEdgeLanguagePair
    {
        public TradosToMTEdgeLanguagePair(string languagePair, CultureInfo tradosCulture, List<MTEdgeLanguagePair> languagePairs)
        {
            LanguagePair = languagePair;
            TradosCulture = tradosCulture;
            MtEdgeLanguagePairs = languagePairs;
        }

        public string LanguagePair { get; set; }

        public CultureInfo TradosCulture { get; }

        public List<DictionaryModel> Dictionaries { get; set; }

        public List<MTEdgeLanguagePair> MtEdgeLanguagePairs { get; }
    }
}