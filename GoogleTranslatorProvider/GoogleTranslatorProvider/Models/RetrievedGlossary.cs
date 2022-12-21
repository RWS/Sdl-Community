using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Google.Cloud.Translate.V3;

namespace GoogleTranslatorProvider.Models
{
	public class RetrievedGlossary
	{
		private readonly Glossary _glossary;
		private readonly string _glossaryID;
		private readonly string _displayName;
		private readonly string _glossaryResourceLocation;
		private readonly CultureInfo _sourceLanguage;
		private readonly CultureInfo _targetLanguage;
		private readonly HashSet<string> _languages;

		public RetrievedGlossary(Glossary glossary, string projectId = null, string projectLocation = null)
		{
			_glossary = glossary;
			_glossaryID = Glossary?.GlossaryName?.GlossaryId;
			var languagePair = Glossary?.LanguagePair;
			var languageSet = Glossary?.LanguageCodesSet;
			if (languagePair is null && languageSet is null)
			{
				_displayName = _glossary is null ? "> No glossaries available" : "> No glossary selected";
				return;
			}

			_glossaryResourceLocation = string.Format(
				"projects/{0}/locations/{1}/glossaries/{2}",
				projectId, projectLocation, _glossaryID);

			if (languagePair is not null)
			{
				var sourceLanguage = languagePair.SourceLanguageCode;
				var targetLanguage = languagePair.TargetLanguageCode;
				_displayName = $"{GlossaryID} : {sourceLanguage} - {targetLanguage}";
				_sourceLanguage = new CultureInfo(sourceLanguage);
				_targetLanguage = new CultureInfo(targetLanguage);
			}
			else if (languageSet is not null)
			{
				_displayName = $"{GlossaryID} : {languageSet.LanguageCodes.Count} languages";
				_languages = languageSet.LanguageCodes.ToHashSet();
			}
		}

		public Glossary Glossary => _glossary;

		public string GlossaryID => _glossaryID;

		public string DisplayName => _displayName;

		public string GlossaryResourceLocation => _glossaryResourceLocation;

		public CultureInfo SourceLanguage => _sourceLanguage;

		public CultureInfo TargetLanguage => _targetLanguage;

		public HashSet<string> Languages => _languages;
	}
}