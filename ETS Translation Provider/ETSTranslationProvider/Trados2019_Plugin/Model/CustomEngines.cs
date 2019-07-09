using System.Collections.Generic;

namespace ETSTranslationProvider.Model
{
	public class CustomEngines
	{
		//ETS engine three letter code
		public CustomEngines()
		{
			FrenchCanadaEngineCode = "FRC";
			SpanishLatinAmericanEngineCode = "ESL";
			LatinAmericanLanguageCodes = new List<string>
			{
				"ESS","ESV","ESB","ZZZ","ESL","ESO","ESC","ESK","ESD","ESF","ESE","ESG","ESH","ESJ","ESP","ESM","ESP","ESI","ESA","ESZ","ESR","ESU","EST","ESY"
			};
		}

		public string FrenchCanadaEngineCode { get; set; }
		public string SpanishLatinAmericanEngineCode { get; set; }
		public List<string> LatinAmericanLanguageCodes { get; set; }
	}
}
