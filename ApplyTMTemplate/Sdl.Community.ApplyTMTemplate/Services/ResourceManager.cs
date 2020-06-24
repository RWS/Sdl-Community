using System;
using System.Collections.Generic;
using System.Linq;
using Sdl.Community.ApplyTMTemplate.Models;
using Sdl.Community.ApplyTMTemplate.Models.Interfaces;
using Sdl.Community.ApplyTMTemplate.Services.Interfaces;
using Sdl.Core.LanguageProcessing.Tokenization;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.Core.Segmentation;
using Sdl.LanguagePlatform.TranslationMemoryApi;

namespace Sdl.Community.ApplyTMTemplate.Services
{
	public class ResourceManager
	{
		private readonly IExcelResourceManager _excelResourceManager;
		private readonly ILanguageResourcesContainer _languageResourcesContainer;
		private readonly Settings _settings;

		public ResourceManager(Settings settings, IExcelResourceManager excelResourceManager, ILanguageResourcesContainer languageResourcesContainer)
		{
			_languageResourcesContainer = languageResourcesContainer;
			_settings = settings;
			_excelResourceManager = excelResourceManager;
		}

		public bool ValidateTemplate(bool isImport)
		{
			return _languageResourcesContainer.ValidateTemplate(isImport);
		}

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
			//this method ensures that the defaults are used and that data that wasn't opted for isn't affected
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
			_excelResourceManager.ExportResourcesToExcel(_languageResourcesContainer, filePathTo, settings);
		}

		public void ImportResourcesFromExcel(string filePathFrom)
		{
			var newLanguageResourceBundles = _excelResourceManager.GetResourceBundlesFromExcel(filePathFrom, _settings);
			AddNewBundles(newLanguageResourceBundles);

			//TODO: add Global Settings to template
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

		private void AddItemsToWordlist(LanguageResourceBundle newLanguageResourceBundle, LanguageResourceBundle template, string property)
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

		private void AddNumberSeparators(LanguageResourceBundle newBundle, LanguageResourceBundle correspondingBundleInTemplate)
		{
			if (newBundle.NumbersSeparators?.Count > 0)
			{
				if (correspondingBundleInTemplate.NumbersSeparators != null &&
					correspondingBundleInTemplate.NumbersSeparators.Count > 0)
				{
					foreach (var separator in newBundle.NumbersSeparators)
					{
						if (!correspondingBundleInTemplate.NumbersSeparators.Contains(separator))
						{
							correspondingBundleInTemplate.NumbersSeparators.Add(separator);
						}
					}
				}
				else
				{
					correspondingBundleInTemplate.NumbersSeparators = newBundle.NumbersSeparators;
				}
			}
		}

		private void AddSegmentationRulesToBundle(LanguageResourceBundle newBundle, LanguageResourceBundle correspondingBundleInTemplate)
		{
			if (newBundle.SegmentationRules == null) return;
			if (correspondingBundleInTemplate.SegmentationRules != null)
			{
				foreach (var newRule in newBundle.SegmentationRules.Rules)
				{
					if (!correspondingBundleInTemplate.SegmentationRules.Rules.Any(oldRule =>
						string.Equals(newRule.Description.Text, oldRule.Description.Text,
							StringComparison.OrdinalIgnoreCase)))
					{
						correspondingBundleInTemplate.SegmentationRules.AddRule(newRule);
					}
				}
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
					AddDates(newBundle, correspondingBundleInTemplate);
					AddTimes(newBundle, correspondingBundleInTemplate);
					AddNumberSeparators(newBundle, correspondingBundleInTemplate);
					newBundle.CurrencyFormats.C
				}
				//otherwise, just add the newBundle
				else
				{
					_languageResourcesContainer.LanguageResourceBundles.Add(newBundle);
				}
			}
		}



		private void AddTimes(LanguageResourceBundle newBundle, LanguageResourceBundle correspondingBundleInTemplate)
		{
			if (newBundle.LongTimeFormats?.Count > 0)
			{
				if (correspondingBundleInTemplate.LongTimeFormats != null &&
					correspondingBundleInTemplate.LongTimeFormats.Count > 0)
				{
					foreach (var longTime in newBundle.LongTimeFormats)
					{
						if (!correspondingBundleInTemplate.LongTimeFormats.Contains(longTime))
						{
							correspondingBundleInTemplate.LongTimeFormats.Add(longTime);
						}
					}
				}
				else
				{
					correspondingBundleInTemplate.LongTimeFormats = newBundle.LongTimeFormats;
				}
			}

			if (newBundle.ShortTimeFormats?.Count > 0)
			{
				if (correspondingBundleInTemplate.ShortTimeFormats != null && correspondingBundleInTemplate.ShortTimeFormats.Count > 0)
				{
					foreach (var shortTime in newBundle.ShortTimeFormats)
					{
						if (!correspondingBundleInTemplate.ShortTimeFormats.Contains(shortTime))
						{
							correspondingBundleInTemplate.ShortTimeFormats.Add(shortTime);
						}
					}
				}
				else
				{
					correspondingBundleInTemplate.ShortTimeFormats = newBundle.ShortTimeFormats;
				}
			}
		}

		private void AddDates(LanguageResourceBundle newBundle, LanguageResourceBundle correspondingBundleInTemplate)
		{
			if (newBundle.LongDateFormats?.Count > 0)
			{
				if (correspondingBundleInTemplate.LongDateFormats != null && correspondingBundleInTemplate.LongDateFormats.Count > 0)
				{
					foreach (var longDate in newBundle.LongDateFormats)
					{
						if (!correspondingBundleInTemplate.LongDateFormats.Contains(longDate))
						{
							correspondingBundleInTemplate.LongDateFormats.Add(longDate);
						}
					}
				}
				else
				{
					correspondingBundleInTemplate.LongDateFormats = newBundle.LongDateFormats;
				}
			}

			if (newBundle.ShortDateFormats?.Count > 0)
			{
				if (correspondingBundleInTemplate.ShortDateFormats != null && correspondingBundleInTemplate.ShortDateFormats.Count > 0)
				{
					foreach (var shortDate in newBundle.ShortDateFormats)
					{
						if (!correspondingBundleInTemplate.ShortDateFormats.Contains(shortDate))
						{
							correspondingBundleInTemplate.ShortDateFormats.Add(shortDate);
						}
					}
				}
				else
				{
					correspondingBundleInTemplate.ShortDateFormats = newBundle.ShortDateFormats;
				}
			}
		}
	}
}