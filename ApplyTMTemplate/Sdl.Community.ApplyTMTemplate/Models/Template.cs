using System.Collections.Generic;
using Sdl.LanguagePlatform.TranslationMemoryApi;

namespace Sdl.Community.ApplyTMTemplate.Models
{
	public class Template
	{
		public Template(FileBasedLanguageResourcesTemplate template)
		{
			ResourceTemplate = template;
		}

		public FileBasedLanguageResourcesTemplate ResourceTemplate { get; set; }

		public void ApplyTmTemplate(List<TranslationMemory> translationMemories, Settings settings)
		{
			var resourceBundlesWithOptions = new List<LanguageResourceBundle>();

			foreach (var resourceBundle in ResourceTemplate.LanguageResourceBundles)
			{
				var newResourceBundle = new LanguageResourceBundle(resourceBundle.Language);

				if (settings.VariablesChecked)
				{
					newResourceBundle.Variables = resourceBundle.Variables;
				}

				if (settings.AbbreviationsChecked)
				{
					newResourceBundle.Abbreviations = resourceBundle.Abbreviations;
				}

				if (settings.OrdinalFollowersChecked)
				{
					newResourceBundle.OrdinalFollowers = resourceBundle.OrdinalFollowers;
				}

				if (settings.SegmentationRulesChecked)
				{
					newResourceBundle.SegmentationRules = resourceBundle.SegmentationRules;
				}

				resourceBundlesWithOptions.Add(newResourceBundle);
			}

			foreach (var languageResourceBundle in resourceBundlesWithOptions)
			{
				foreach (var translationMemory in translationMemories)
				{
					translationMemory.ApplyTemplate(languageResourceBundle);
				}
			}
		}
	}
}