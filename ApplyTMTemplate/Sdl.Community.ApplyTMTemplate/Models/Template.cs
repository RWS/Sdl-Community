using System.Collections.Generic;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.Core.Segmentation;
using Sdl.LanguagePlatform.TranslationMemoryApi;

namespace Sdl.Community.ApplyTMTemplate.Models
{
	public class Template
	{
		public Template(List<LanguageResourceBundle> resourceBundle)
		{
			ResourceBundles = resourceBundle;
		}

		public List<LanguageResourceBundle> ResourceBundles { get; set; }

		public void ApplyTmTemplate(List<TranslationMemory> translationMemories, Options options)
		{
			var resourceBundlesWithOptions = new List<LanguageResourceBundle>();

			foreach (var resourceBundle in ResourceBundles)
			{
				var newResourceBundle = new LanguageResourceBundle(resourceBundle.Language);

				if (options.VariablesChecked)
				{
					newResourceBundle.Variables = resourceBundle.Variables;
				}

				if (options.AbbreviationsChecked)
				{
					newResourceBundle.Abbreviations = resourceBundle.Abbreviations;
				}

				if (options.OrdinalFollowersChecked)
				{
					newResourceBundle.OrdinalFollowers = resourceBundle.OrdinalFollowers;
				}

				if (options.SegmentationRulesChecked)
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