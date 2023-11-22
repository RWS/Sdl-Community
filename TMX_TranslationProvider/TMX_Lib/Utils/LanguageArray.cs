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
		private Dictionary<string, HashSet<string>> _languages = new Dictionary<string, HashSet<string>>();
		private HashSet<string> _languageAndLocale = new HashSet<string>();

		public IReadOnlyList<string> LanguageAndLocaleArray() { 
			lock(this)
				return _languageAndLocale.ToList();
		}

		public void LoadLanguages(IReadOnlyList<string> languages)
		{
			foreach (var languageAndLocale in languages)
				AddLanguage(languageAndLocale);
		}
		public void AddLanguage(string languageAndLocale) {
			var (language, locale) = Util.NormalizeLanguage(languageAndLocale);
			lock (this) {
				if (!_languages.ContainsKey(language))
					_languages.Add(language, new HashSet<string>());
				_languages[language].Add(locale);
				_languageAndLocale.Add($"{language}-{locale}");
			}
		}

		// returns null if we could not find an equivalent
		public bool SupportsLanguage(string languageAndLocale, bool careForLocale)
		{
			var (language, locale) = Util.NormalizeLanguage(languageAndLocale);
			lock (this) { 
				var supportsLanguage = _languages.ContainsKey(language);
				if (supportsLanguage) {
					if (!careForLocale)
						return true;
					// care for locale as well
					var locales = _languages[language];
					return locales.Contains(locale);
				}
			}

			return false;
		}
	}
}
