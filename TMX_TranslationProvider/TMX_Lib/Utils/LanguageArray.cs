using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMX_Lib.Utils
{
	// holds an array of languages
	// if you ask for a language, it will return an equivalent language, if it finds one
	public class LanguageArray
	{
		private IReadOnlyList<string> _languages = new List<string>();

		public IReadOnlyList<string> Languages
		{
			get => _languages;
			set => _languages = value;
		}

		// returns null if we could not find an equivalent
		public string TryGetEquivalentLanguage(string language)
		{
			var foundExact = _languages.FirstOrDefault(l => l.Equals(language, StringComparison.InvariantCultureIgnoreCase));
			if (foundExact != null)
				return foundExact;

			var foundEquivalent = _languages.FirstOrDefault(l => CompareLanguages.Equivalent(l, language));
			return foundEquivalent;
		}
	}
}
