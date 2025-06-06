﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Sdl.Community.ApplyTMTemplate.Models;
using Sdl.Community.ApplyTMTemplate.Models.Interfaces;
using Sdl.Community.ApplyTMTemplate.Services.Interfaces;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.Core.Tokenization;
using Sdl.LanguagePlatform.TranslationMemory;
using Sdl.LanguagePlatform.TranslationMemoryApi;

namespace Sdl.Community.ApplyTMTemplate.Services
{
	public class ResourceManager : IResourceManager
	{
		private readonly IExcelResourceManager _excelResourceManager;
		private readonly IMessageService _messageService;

		public ResourceManager(IExcelResourceManager excelResourceManager, IMessageService messageService)
		{
			_messageService = messageService;
			_excelResourceManager = excelResourceManager;
		}

        public void TransferResourcesFromBundle(Settings settings, LanguageResourceBundle sourceBundle, LanguageResourceBundle targetBundle, bool overwrite)
		{
			//TODO: this method would be very useful if there would be a class for each resource and we could just Invoke the class' method for adding each specific type
			if (targetBundle == null) return;
			foreach (var propertyInfo in typeof(Settings).GetProperties())
			{
				if (propertyInfo.CanWrite)
				{
					if ((bool)propertyInfo.GetValue(settings))
					{
						var property = targetBundle?.GetType().GetProperties()
							.Where(prop => prop.Name.Contains(propertyInfo.Name.Substring(0, 4)));
						property?.ToList().ForEach(bundleProp => bundleProp?.SetValue(targetBundle, bundleProp.GetValue(sourceBundle)));
					}
				}
			}
		}

		public void ApplyTemplateToTms(
			ILanguageResourcesContainer languageResourcesContainer, 
			List<TranslationMemory> translationMemories, 
			Settings settings, 
			bool overwrite)
		{
			// Don't forget about the global settings
			foreach (var translationMemory in translationMemories)
			{
				var sourceLang = translationMemory.LanguageDirection.SourceLanguage;
				var targetLang = translationMemory.LanguageDirection.TargetLanguage;

				var lrContainer = languageResourcesContainer.LanguageResourceBundles;

				//take just the bundles the TM supports
				var newBundles = new List<LanguageResourceBundle> { lrContainer[sourceLang], lrContainer[targetLang] };
				if (newBundles.All(bundle => bundle == null)) return;

				ExcludeWhatIsNotNeeded(settings, newBundles);
				ExcludeWhatIsNotNeeded(settings, translationMemory.LanguageResourceBundles, overwrite);

				AddNewBundles(newBundles, translationMemory);
				AddGlobalSettings(
					translationMemory,
					settings.RecognizersChecked,
					languageResourcesContainer.Recognizers,
					settings.WordCountFlagsChecked,
					languageResourcesContainer.WordCountFlags, overwrite);
				
				translationMemory.Save();
			}
		}

		public void ExportResourcesToExcel(ILanguageResourcesContainer languageResourcesContainer, string filePathTo,
			Settings settings)
		{
			//TODO: refactor so settings don't have to be passed to the _excelResourceManager; use ExcludeWhatIsNotNeeded method
			_excelResourceManager.ExportResourcesToExcel(languageResourcesContainer, filePathTo, settings);
		}

		public void ImportResourcesFromExcel(string excelFilePath, ILanguageResourcesContainer languageResourcesContainer, Settings settings, bool overwrite)
		{
			var newLanguageResourceBundles = _excelResourceManager.GetResourceBundlesFromExcel(excelFilePath);
            var newBundlesLanguageCodes = newLanguageResourceBundles.Select(bundle => bundle.LanguageCode);

            ExcludeWhatIsNotNeeded(settings, newLanguageResourceBundles);

            ExcludeWhatIsNotNeeded(
                settings,
                languageResourcesContainer?.LanguageResourceBundles
                    .Where(bundle => newBundlesLanguageCodes.Contains(bundle.LanguageCode)),
                overwrite);
			AddNewBundles(newLanguageResourceBundles, languageResourcesContainer);

			var (recognizers, wordCountFlags) = _excelResourceManager.GetTemplateGlobalSettings(excelFilePath, settings);
			AddGlobalSettings(languageResourcesContainer, settings.RecognizersChecked, recognizers, settings.WordCountFlagsChecked, wordCountFlags, overwrite);

			SaveTemplate(languageResourcesContainer);
		}

		public void ImportResourcesFromSdltm(List<TranslationMemory> translationMemories, ILanguageResourcesContainer languageResourcesContainer, Settings settings, bool overwrite)
		{
			var newLanguageResourceBundles = GetResourcesFromTMs(translationMemories);
            var newBundlesLanguageCodes = newLanguageResourceBundles.Select(bundle => bundle.LanguageCode);

            if (newLanguageResourceBundles.Count == 0)
			{
				_messageService.ShowWarningMessage(PluginResources.Warning, PluginResources.No_Resources_in_TMs);
			}

			ExcludeWhatIsNotNeeded(settings, newLanguageResourceBundles);
			ExcludeWhatIsNotNeeded(
				settings,
				languageResourcesContainer?.LanguageResourceBundles
					.Where(bundle => newBundlesLanguageCodes.Contains(bundle.LanguageCode)),
				overwrite);
			AddNewBundles(newLanguageResourceBundles, languageResourcesContainer);

			BuiltinRecognizers recognizers = BuiltinRecognizers.RecognizeNone;
			WordCountFlags wordcountflags = WordCountFlags.NoFlags;

			foreach (var tm in translationMemories)
			{
				recognizers |= tm.Recognizers;
				wordcountflags |= tm.WordCountFlags;
				
			}

            AddGlobalSettings(
                    languageResourcesContainer, settings.RecognizersChecked, recognizers,
                    settings.WordCountFlagsChecked, wordcountflags, overwrite);

            SaveTemplate(languageResourcesContainer);
		}

		private static List<LanguageResourceBundle> GetResourcesFromTMs(List<TranslationMemory> translationMemories)
		{
			var newLanguageResourceBundles = new List<LanguageResourceBundle>();
			foreach (var tm in translationMemories)
			{
				if (tm.LanguageResourceBundles.Count == 0) continue;
				foreach (var bundle in tm.LanguageResourceBundles)
				{
					newLanguageResourceBundles.Add(bundle);
				}
			}

			return newLanguageResourceBundles;
		}

		private void AddGlobalSettings(
			ILanguageResourcesContainer languageResourceContainer,
			bool updateRecognizers,
			BuiltinRecognizers recognizers,
			bool updateWordCountFlags,
			WordCountFlags wordCountFlags,
			bool overwrite)
		{
			if (updateRecognizers)
			{
				if (overwrite) languageResourceContainer.Recognizers = recognizers;
				else languageResourceContainer.Recognizers |= recognizers;
			}

			if (updateWordCountFlags)
			{
				if (overwrite) languageResourceContainer.WordCountFlags = wordCountFlags;
				else languageResourceContainer.WordCountFlags |= wordCountFlags;
			}

		}

		private Wordlist AddItemsToWordlist(Wordlist listOne, Wordlist listTwo)
		{
			if (listTwo != null)
			{
				listOne?.Items.ToList().ForEach(newItem => listTwo.Add(newItem));
			}

			return listTwo?.Count > 0 ? listTwo : null;
		}

		private void AddMeasurementUnits(LanguageResourceBundle newBundle, LanguageResourceBundle correspondingBundleInTemplate)
		{
			if (newBundle.MeasurementUnits != null)
			{
				if (correspondingBundleInTemplate.MeasurementUnits != null)
				{
					foreach (var unit in newBundle.MeasurementUnits)
					{
						if (!correspondingBundleInTemplate.MeasurementUnits.ContainsKey(unit.Key))
						{
							correspondingBundleInTemplate.MeasurementUnits.Add(unit.Key, unit.Value);
						}
					}
				}
				else
				{
					correspondingBundleInTemplate.MeasurementUnits = newBundle.MeasurementUnits;
				}
			}

			if (correspondingBundleInTemplate.MeasurementUnits.Count == 0)
			{
				correspondingBundleInTemplate.MeasurementUnits = null;
			}
		}

		// Maybe put some merge option
		private void AddNewBundles(
			List<LanguageResourceBundle> newLanguageResourceBundles, 
			ILanguageResourcesContainer languageResourcesContainer)
		{
			foreach (var newBundle in newLanguageResourceBundles)
			{
				if (newBundle == null) continue;

				var currentBundle = languageResourcesContainer.LanguageResourceBundles[newBundle.Language];
				var correspondingBundleInTemplate = currentBundle is null
					? new LanguageResourceBundle(newBundle.Language)
					: currentBundle.Clone();

                languageResourcesContainer.LanguageResourceBundles.Remove(currentBundle);
                languageResourcesContainer.LanguageResourceBundles.Add(correspondingBundleInTemplate);

				//to be able to deal with all the resources uniformly and easily, we must ensure that none are null
				InitializeBundle(correspondingBundleInTemplate);

				AddWordlists(correspondingBundleInTemplate, newBundle);
				AddSegmentationRulesToBundle(newBundle, correspondingBundleInTemplate);
				AddSimpleResources(correspondingBundleInTemplate, newBundle);
				AddMeasurementUnits(newBundle, correspondingBundleInTemplate);
			}
		}

		private void AddSegmentationRulesToBundle(LanguageResourceBundle newBundle, LanguageResourceBundle correspondingBundleInTemplate)
		{
			if (newBundle.SegmentationRules != null)
			{
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
					correspondingBundleInTemplate.SegmentationRules = newBundle.SegmentationRules;
				}
			}

			if (correspondingBundleInTemplate.SegmentationRules.Count == 0)
			{
				correspondingBundleInTemplate.SegmentationRules = null;
			}
		}

		private void AddSimpleResources(LanguageResourceBundle correspondingBundleInTemplate, LanguageResourceBundle newBundle)
		{
			correspondingBundleInTemplate.LongDateFormats = GetUnionOfListsOfObjects(newBundle.LongDateFormats,
				correspondingBundleInTemplate.LongDateFormats);
			correspondingBundleInTemplate.ShortDateFormats = GetUnionOfListsOfObjects(newBundle.ShortDateFormats,
				correspondingBundleInTemplate.ShortDateFormats);
			correspondingBundleInTemplate.LongTimeFormats = GetUnionOfListsOfObjects(newBundle.LongTimeFormats,
				correspondingBundleInTemplate.LongTimeFormats);
			correspondingBundleInTemplate.ShortTimeFormats = GetUnionOfListsOfObjects(newBundle.ShortTimeFormats,
				correspondingBundleInTemplate.ShortTimeFormats);

			correspondingBundleInTemplate.NumbersSeparators = GetUnionOfListsOfObjects(newBundle.NumbersSeparators,
				correspondingBundleInTemplate.NumbersSeparators);
			correspondingBundleInTemplate.CurrencyFormats = GetUnionOfListsOfObjects<CurrencyFormat>(newBundle.CurrencyFormats,
				correspondingBundleInTemplate.CurrencyFormats, new CurrencyComparer());
		}

		private void AddWordlists(LanguageResourceBundle correspondingBundleInTemplate, LanguageResourceBundle newBundle)
		{
			correspondingBundleInTemplate.Abbreviations =
				AddItemsToWordlist(newBundle.Abbreviations, correspondingBundleInTemplate.Abbreviations);
			correspondingBundleInTemplate.OrdinalFollowers =
				AddItemsToWordlist(newBundle.OrdinalFollowers, correspondingBundleInTemplate.OrdinalFollowers);
			correspondingBundleInTemplate.Variables =
				AddItemsToWordlist(newBundle.Variables, correspondingBundleInTemplate.Variables);
		}

        private void ExcludeWhatIsNotNeeded(Settings settings, IEnumerable<LanguageResourceBundle> languageResourceBundles, bool overwrite)
		{
            foreach (var languageResourceBundle in languageResourceBundles)
            {
                if (languageResourceBundle is null) continue;
                foreach (var propertyInfo in typeof(Settings).GetProperties())
                {
                    if (propertyInfo.CanWrite)
                    {
                        if (((bool)propertyInfo.GetValue(settings) && overwrite))
                        {
                            var property = languageResourceBundle.GetType().GetProperties()
                                .Where(prop => prop.Name.Contains(propertyInfo.Name.Substring(0, 4)));
                            property.ToList().ForEach(bundleProp => bundleProp?.SetValue(languageResourceBundle, null));
                        }
                    }
                }
            }
        }

		private void ExcludeWhatIsNotNeeded(Settings settings, IEnumerable<LanguageResourceBundle> languageResourceBundles)
		{
			foreach (var languageResourceBundle in languageResourceBundles)
			{
				if (languageResourceBundle is null) continue;
				foreach (var propertyInfo in typeof(Settings).GetProperties())
				{
					if (propertyInfo.CanWrite)
					{
						if (!(bool)propertyInfo.GetValue(settings))
						{
							var property = languageResourceBundle.GetType().GetProperties()
								.Where(prop => prop.Name.Contains(propertyInfo.Name.Substring(0, 4)));
							property.ToList().ForEach(bundleProp => bundleProp?.SetValue(languageResourceBundle, null));
						}
					}
				}
			}
		}

		private List<T> GetUnionOfListsOfObjects<T>(List<T> listOne, List<T> listTwo, IEqualityComparer<T> equalityComparer = null)
		{
			if (listOne != null)
			{
				listTwo = equalityComparer != null
					? listTwo?.Union(listOne, equalityComparer).ToList()
					: listTwo?.Union(listOne).ToList();
			}

			return listTwo?.Count == 0 ? null : listTwo;
		}

		private void InitializeBundle(LanguageResourceBundle correspondingBundleInTemplate)
		{
			foreach (var propertyInfo in typeof(LanguageResourceBundle).GetProperties())
			{
				if (propertyInfo.CanWrite)
				{
					if (propertyInfo.GetValue(correspondingBundleInTemplate) == null)
						propertyInfo.SetValue(correspondingBundleInTemplate, Activator.CreateInstance(propertyInfo.PropertyType));
				}
			}
		}

		private void SaveTemplate(ILanguageResourcesContainer languageResourcesContainer)
		{
			languageResourcesContainer.Save();
		}
	}
}