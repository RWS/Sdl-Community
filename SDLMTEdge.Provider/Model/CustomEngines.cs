using System.Collections.Generic;

namespace Sdl.Community.MTEdge.Provider.Model
{
	public class CustomEngines
	{
		public CustomEngines() { }

		public string FrenchCanadaEngineCode => "FRC";

		public string PortugueseSourceEngineCode => "POR";

		public string SpanishLatinAmericanEngineCode => "ESL";

		public List<string> LatinAmericanLanguageCodes => new()
        {
			"ESS","ESV","ESB","ZZZ","ESL","ESO","ESC","ESK","ESD","ESF","ESE","ESG","ESH","ESJ","ESP","ESM","ESP","ESI","ESA","ESZ","ESR","ESU","EST","ESY"
		};
    }
}