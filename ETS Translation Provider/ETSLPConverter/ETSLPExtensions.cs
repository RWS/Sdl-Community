using System.Collections;
using System.Globalization;
using System.Linq;

namespace ETSLPConverter
{
    public static class ETSLPExtensions
    {
	    public static string ToETSCode(this CultureInfo language)
	    {
		    var languageCode = language.IetfLanguageTag.ToLower();

		    // Make an exception for languages that require generics, but don't share a common ietf prefix with the generic
		    // (ie nb-no doesn't have "no" as its first half, which in languages.xml, it should)
		    if (Converter.ForcedGenericLPs.ContainsKey(languageCode))
		    {
			    return Converter.ForcedGenericLPs[languageCode];
		    }

		    var languageResource = ETSLanguages.ResourceManager.GetResourceSet(CultureInfo.CurrentUICulture, true, true);

		    var languagesDictionary = languageResource.Cast<DictionaryEntry>()
			    .ToDictionary(x => x.Key.ToString(),
				    x => x.Value.ToString());

		    var currentLanguage =
			    languagesDictionary.Keys.FirstOrDefault(k => k.Equals(language.ThreeLetterWindowsLanguageName.ToLower()));

		    if (!string.IsNullOrEmpty(currentLanguage))
		    {
			    return currentLanguage.ToLower();
		    }

			//the language code used for ets for en-Us is eng
		    currentLanguage =
			    languagesDictionary.Keys.FirstOrDefault(k => k.Equals(language.ThreeLetterISOLanguageName.ToLower()));
		    if (!string.IsNullOrEmpty(currentLanguage))
		    {
			    return currentLanguage;
		    }
		    return language.ThreeLetterISOLanguageName.ToLower();

	    }
    }
}