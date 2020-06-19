using System;
using System.Collections.Generic;
using System.Linq;
using Sdl.Community.ApplyTMTemplate.Models.Interfaces;
using Sdl.Community.ApplyTMTemplate.Services.Interfaces;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.Core.Segmentation;
using Sdl.LanguagePlatform.TranslationMemoryApi;

namespace Sdl.Community.ApplyTMTemplate.Models
{
	public class ResourceManager
	{
		private readonly IExcelResourceWriter _excelResourceWriter;
		private readonly ILanguageResourcesContainer _languageResourcesContainer;
		private readonly Settings _settings;

		public ResourceManager(Settings settings, IExcelResourceWriter excelResourceWriter, ILanguageResourcesContainer languageResourcesContainer)
		{
			_languageResourcesContainer = languageResourcesContainer;
			_settings = settings;
			_excelResourceWriter = excelResourceWriter;
		}

		public LanguageResourceBundleCollection LanguageResourceBundles =>
			_languageResourcesContainer.LanguageResourceBundles;

		public void ApplyTemplateToTms(List<TranslationMemory> translationMemories)
		{
			var resourceBundlesWithOptions = new List<LanguageResourceBundle>();

			foreach (var resourceBundle in _languageResourcesContainer.LanguageResourceBundles)
			{
				var newResourceBundle = new LanguageResourceBundle(resourceBundle.Language);

				if (_settings.VariablesChecked)
				{
					newResourceBundle.Variables = resourceBundle.Variables;
				}

				if (_settings.AbbreviationsChecked)
				{
					newResourceBundle.Abbreviations = resourceBundle.Abbreviations;
				}

				if (_settings.OrdinalFollowersChecked)
				{
					newResourceBundle.OrdinalFollowers = resourceBundle.OrdinalFollowers;
				}

				if (_settings.SegmentationRulesChecked)
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

		public void ExcludeWhatIsNotNeeded(List<LanguageResourceBundle> languageResourceBundles)
		{
			foreach (var bundle in languageResourceBundles)
			{
				if (bundle.Abbreviations != null && (!_settings.AbbreviationsChecked || bundle.Abbreviations.Count == 0))
				{
					bundle.Abbreviations = null;
				}

				if (bundle.OrdinalFollowers != null && (!_settings.OrdinalFollowersChecked || bundle.OrdinalFollowers.Count == 0))
				{
					bundle.OrdinalFollowers = null;
				}

				if (bundle.Variables != null && (!_settings.VariablesChecked || bundle.Variables.Count == 0))
				{
					bundle.Variables = null;
				}

				if (bundle.SegmentationRules != null && (!_settings.SegmentationRulesChecked || bundle.SegmentationRules.Count == 0))
				{
					bundle.SegmentationRules = null;
				}
			}
		}

		public void ExportResourcesToExcel(string filePathTo,
			Settings settings)
		{
			_excelResourceWriter.ExportResourcesToExcel(_languageResourcesContainer, filePathTo, settings);
		}

		public void ImportResourcesFromExcel(string filePathFrom)
		{
			var newLanguageResourceBundles = _excelResourceWriter.GetResourceBundlesFromExcel(filePathFrom);

			ExcludeWhatIsNotNeeded(newLanguageResourceBundles);
			AddNewBundles(newLanguageResourceBundles);

			_languageResourcesContainer.Save();
		}

		public void ImportResourcesFromSdltm(List<TranslationMemory> translationMemories)
		{
			var newLanguageResourceBundles = new List<LanguageResourceBundle>();
			foreach (var tm in translationMemories)
			{
				if (tm.Tm.LanguageResourceBundles.Count == 0) continue;
				foreach (var bundle in tm.Tm.LanguageResourceBundles)
				{
					newLanguageResourceBundles.Add(bundle);
				}
			}
			if (newLanguageResourceBundles.Count == 0) throw new Exception(PluginResources.No_Resources_in_TMs);

			ExcludeWhatIsNotNeeded(newLanguageResourceBundles);
			AddNewBundles(newLanguageResourceBundles);

			_languageResourcesContainer.Save();
		}

		private static void AddItemsToWordlist(LanguageResourceBundle newLanguageResourceBundle, LanguageResourceBundle template, string property)
		{
			var templateBundleGetter = typeof(LanguageResourceBundle).GetProperty(property)?.GetMethod.Invoke(template, null) as Wordlist;
			var templateBundleSetter = typeof(LanguageResourceBundle).GetProperty(property)?.SetMethod;
			var newBundleGetter = typeof(LanguageResourceBundle).GetProperty(property)?.GetMethod.Invoke(newLanguageResourceBundle, null) as Wordlist;

			if (newBundleGetter == null || !newBundleGetter.Items.Any()) return;

			if (templateBundleGetter != null && templateBundleGetter.Items.Any())
			{
				foreach (var abbrev in newBundleGetter.Items)
				{
					templateBundleGetter.Add(abbrev);
				}
			}
			else
			{
				templateBundleSetter?.Invoke(template, new object[] { new Wordlist(newBundleGetter) });
			}
		}

		private static void AddSegmentationRulesToBundle(LanguageResourceBundle newBundle, LanguageResourceBundle correspondingBundleInTemplate)
		{
			if (newBundle.SegmentationRules == null) return;
			if (correspondingBundleInTemplate.SegmentationRules != null)
			{
				var newSegmentationRules = new SegmentationRules();
				foreach (var newRule in newBundle.SegmentationRules.Rules)
				{
					if (correspondingBundleInTemplate.SegmentationRules.Rules.All(oldRule =>
						!string.Equals(newRule.Description.Text, oldRule.Description.Text,
							StringComparison.OrdinalIgnoreCase)))
					{
						newSegmentationRules.AddRule(newRule);
					}
				}

				correspondingBundleInTemplate.SegmentationRules.Rules.AddRange(newSegmentationRules.Rules);
			}
			else
			{
				correspondingBundleInTemplate.SegmentationRules = new SegmentationRules(newBundle.SegmentationRules);
			}
		}

		private void AddNewBundles(List<LanguageResourceBundle> newLanguageResourceBundles)
		{
			foreach (var newBundle in newLanguageResourceBundles)
			{
				var correspondingBundleInTemplate = _languageResourcesContainer.LanguageResourceBundles[newBundle.Language];

				//in case there is already a bundle for that culture, we need to go into more detail and see what to add and what is already there
				if (correspondingBundleInTemplate != null)
				{
					AddItemsToWordlist(newBundle, correspondingBundleInTemplate, "Abbreviations");
					AddItemsToWordlist(newBundle, correspondingBundleInTemplate, "OrdinalFollowers");
					AddItemsToWordlist(newBundle, correspondingBundleInTemplate, "Variables");
					AddSegmentationRulesToBundle(newBundle, correspondingBundleInTemplate);
				}
				//otherwise, just add the newBundle
				else
				{
					_languageResourcesContainer.LanguageResourceBundles.Add(newBundle);
				}
			}
		}
	}
}