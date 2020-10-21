using System.Collections.Generic;
using System.Globalization;
using Sdl.Community.MTEdge.Provider.Model;

namespace Sdl.Community.MTEdge.Provider.SDLMTEdgeApi
{
	public class TradosToMTEdgeLP
    {
        public TradosToMTEdgeLP(CultureInfo tradosCulture, List<SDLMTEdgeLanguagePair> mtEdgeLPs)
        {
            TradosCulture = tradosCulture;
            MtEdgeLPs = mtEdgeLPs;
        }

        public CultureInfo TradosCulture { get; }
        public List<SDLMTEdgeLanguagePair> MtEdgeLPs { get; }
		public List<DictionaryModel> Dictionaries { get; set; }
    }
}