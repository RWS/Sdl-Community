using System.Collections.Generic;
using System.Globalization;
using Google.Cloud.Translate.V3;
using System.Linq;
using Google.Protobuf.WellKnownTypes;

namespace GoogleTranslatorProvider.Models
{
	public class RetrievedGlossary
	{
		public RetrievedGlossary(Glossary glossary, string projectId = null, string projectLocation = null)
		{
			Glossary = glossary;
			GlossaryID = Glossary?.GlossaryName?.GlossaryId;
			SetGlossaryDetails(projectId, projectLocation);
		}

		public Glossary Glossary { get; private set; }

		public string DisplayName { get; private set; }

		public string GlossaryID { get; private set; }

		public string GlossaryResourceLocation { get; private set; }

		public CultureInfo SourceLanguage { get; private set; }

		public CultureInfo TargetLanguage { get; private set; }

		public List<CultureInfo> Languages { get; private set; } = new();

		public int LanguagesCount { get; private set; }

		private void SetGlossaryDetails(string projectId, string projectLocation)
		{
			var languagePair = Glossary?.LanguagePair;
			var languageSet = Glossary?.LanguageCodesSet;
			if (languagePair is not null && languageSet is null)
			{
				var sourceLanguage = languagePair.SourceLanguageCode;
				var targetLanguage = languagePair.TargetLanguageCode;
				DisplayName = $"{GlossaryID} : {sourceLanguage} - {targetLanguage}";
				SourceLanguage = new CultureInfo(sourceLanguage);
				TargetLanguage = new CultureInfo(targetLanguage);
			}

			if (languageSet is not null && languagePair is null)
			{
				LanguagesCount = languageSet.LanguageCodes.Count;
				DisplayName = $"{GlossaryID} : {LanguagesCount} languages";
				Languages.AddRange(languageSet.LanguageCodes.Select(languageCode => CultureInfo.CreateSpecificCulture(languageCode)));
			}

			if (languagePair is null&& languageSet is null)
			{
				DisplayName = Glossary is null ? "No glossaries available" : "No glossary selected";
			}

			if (projectId is not null && projectLocation is not null)
			{
				GlossaryResourceLocation = string.Format(
					"projects/{0}/locations/{1}/glossaries/{2}",
					projectId,
					projectLocation,
					GlossaryID);
			}
		}
	}
}