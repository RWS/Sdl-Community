using System.Collections.Generic;

namespace ETSTranslationProvider.Model
{
	public class CustomEngines
	{
		//Key ETS engine three letter code
		//Value parent three letter iso language name
		public CustomEngines()
		{
			FrenchCanada = new KeyValuePair<string, string>("frc", "fra");
			SpanishChile = new KeyValuePair<string, string>("esl", "spa");
		}

		public KeyValuePair<string, string> FrenchCanada { get; set; }
		public KeyValuePair<string, string> SpanishChile { get; set; }

	}
}
