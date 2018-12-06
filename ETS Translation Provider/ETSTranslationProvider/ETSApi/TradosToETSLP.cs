using System.Collections.Generic;
using System.Globalization;

namespace ETSTranslationProvider.ETSApi
{
    public class TradosToETSLP
    {
        public TradosToETSLP(CultureInfo tradosCulture, List<ETSLanguagePair> etsLPs)
        {
            TradosCulture = tradosCulture;
            ETSLPs = etsLPs;
        }
        public CultureInfo TradosCulture { get; private set; }
        public List<ETSLanguagePair> ETSLPs { get; private set; }
    }
}
