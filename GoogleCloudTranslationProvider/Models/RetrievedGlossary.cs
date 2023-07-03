using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Google.Cloud.Translate.V3;

namespace GoogleCloudTranslationProvider.Models
{
	public class RetrievedGlossary
	{
		public RetrievedGlossary(Glossary glossary, string projectId = null, string projectLocation = null)
		{
			Glossary = glossary;
			var languagePair = Glossary?.LanguagePair;
			var languageSet = Glossary?.LanguageCodesSet;
			if (languagePair is null && languageSet is null)
			{
				DisplayName = Glossary is null ? PluginResources.RetrievedResources_NotAvailable
											   : PluginResources.RetrievedResources_Glossaries_Unselected;
				return;
			}

			GlossaryID = Glossary?.GlossaryName?.GlossaryId;
			DisplayName = GlossaryID;
			GlossaryResourceLocation = string.Format(
				"projects/{0}/locations/{1}/glossaries/{2}",
				projectId, projectLocation, GlossaryID);

			if (languagePair is not null)
			{
				var sourceLanguage = languagePair.SourceLanguageCode;
				var targetLanguage = languagePair.TargetLanguageCode;
				SourceLanguage = new CultureInfo(sourceLanguage);
				TargetLanguage = new CultureInfo(targetLanguage);
			}
			else if (languageSet is not null)
			{
				Languages = languageSet.LanguageCodes.ToHashSet();
			}
		}

		public Glossary Glossary { get; set; }

		public string GlossaryID { get; set; }

		public string DisplayName { get; set; }

		public string GlossaryResourceLocation { get; set; }

		public CultureInfo SourceLanguage { get; set; }

		public CultureInfo TargetLanguage { get; set; }

		public HashSet<string> Languages { get; set; }
	}
}