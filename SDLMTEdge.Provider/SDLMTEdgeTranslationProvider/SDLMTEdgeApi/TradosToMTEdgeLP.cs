using System.Collections.Generic;
using System.Globalization;
using Sdl.Community.MTEdge.Provider.Model;

namespace Sdl.Community.MTEdge.Provider.SDLMTEdgeApi
{
	public class TradosToMTEdgeLP
	{
		public TradosToMTEdgeLP(string languagePair, CultureInfo tradosCulture, List<SDLMTEdgeLanguagePair> mtEdgeLPs)
		{
			TradosCulture = tradosCulture;
			LanguagePair = languagePair;
			MtEdgeLPs = mtEdgeLPs;
		}

		public List<DictionaryModel> Dictionaries { get; set; }
		public string LanguagePair { get; set; }
		public List<SDLMTEdgeLanguagePair> MtEdgeLPs { get; }
		public CultureInfo TradosCulture { get; }
	}
}