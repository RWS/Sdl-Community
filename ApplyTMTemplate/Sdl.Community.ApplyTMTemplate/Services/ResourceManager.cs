using System;
using System.Collections.Generic;
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

		public void ApplyTemplateToTms(ILanguageResourcesContainer languageResourcesContainer, List<TranslationMemory> translationMemories, Settings settings)
		{
			var resourceBundlesWithOptions = new List<LanguageResourceBundle>();

			foreach (var resourceBundle in languageResourcesContainer.LanguageResourceBundles)
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

		public void ExportResourcesToExcel(ILanguageResourcesContainer languageResourcesContainer, string filePathTo,
			Settings settings)
		{
			_excelResourceManager.ExportResourcesToExcel(languageResourcesContainer, filePathTo, settings);
		}

		public void ImportResourcesFromExcel(string excelFilePath, ILanguageResourcesContainer languageResourcesContainer, Settings settings)
		{
			var newLanguageResourceBundles = _excelResourceManager.GetResourceBundlesFromExcel(excelFilePath);
			ExcludeWhatIsNotNeeded(settings, newLanguageResourceBundles);
			AddNewBundles(languageResourcesContainer, newLanguageResourceBundles);

			var (recognizers, wordCountFlags) = _excelResourceManager.GetTemplateGlobalSettings(excelFilePath, settings);
			AddGlobalSettings(languageResourcesContainer, recognizers, wordCountFlags);

			SaveTemplate(languageResourcesContainer);
		}

		public void ImportResourcesFromSdltm(List<TranslationMemory> translationMemories, ILanguageResourcesContainer languageResourcesContainer, Settings settings)
		{
			var newLanguageResourceBundles = GetResourcesFromTMs(translationMemories);
			if (newLanguageResourceBundles.Count == 0)
			{
				_messageService.ShowWarningMessage(PluginResources.Warning, PluginResources.No_Resources_in_TMs);
			}

			ExcludeWhatIsNotNeeded(settings, newLanguageResourceBundles);
			AddNewBundles(languageResourcesContainer, newLanguageResourceBundles);

			SaveTemplate(languageResourcesContainer);
		}

		public bool ValidateTemplate(ILanguageResourcesContainer languageResourcesContainer, bool isImport)
		{
			//TODO: do this before any action undertaken
			return languageResourcesContainer.ValidateTemplate(isImport);
		}

		private static List<LanguageResourceBundle> GetResourcesFromTMs(List<TranslationMemory> translationMemories)
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

			return newLanguageResourceBundles;
		}

		private void AddGlobalSettings(ILanguageResourcesContainer languageResourcesContainer, BuiltinRecognizers? recognizers, WordCountFlags? wordCountFlags)
		{
			languageResourcesContainer.Recognizers = recognizers;
			languageResourcesContainer.WordCountFlags = wordCountFlags;
		}

		private Wordlist AddItemsToWordlist(Wordlist listOne, Wordlist listTwo)
		{
			if (listTwo != null)
			{
				listOne?.Items.ToList().ForEach(newItem => listTwo.Add(newItem));
			}

			return listTwo?.Count > 0 ? listTwo : null;
		}

		private void AddMeasurementUnits(LanguageResourceBundle newBundle,
																			LanguageResourceBundle correspondingBundleInTemplate)
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

		private void AddNewBundles(ILanguageResourcesContainer languageResourcesContainer, List<LanguageResourceBundle> newLanguageResourceBundles)
		{
			foreach (var newBundle in newLanguageResourceBundles)
			{
				var correspondingBundleInTemplate = languageResourcesContainer.LanguageResourceBundles[newBundle.Language];
				if (correspondingBundleInTemplate == null)
				{
					correspondingBundleInTemplate = new LanguageResourceBundle(newBundle.Language);
					languageResourcesContainer.LanguageResourceBundles.Add(correspondingBundleInTemplate);
				}
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
			correspondingBundleInTemplate.CurrencyFormats = GetUnionOfListsOfObjects(newBundle.CurrencyFormats,
				correspondingBundleInTemplate.CurrencyFormats);
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

		//public void ExcludeWhatIsNotNeeded(List<LanguageResourceBundle> languageResourceBundles)
		//{
		//	//this method ensures that the defaults are used and that data that wasn't opted for isn't affected
		//	foreach (var bundle in languageResourceBundles)
		//	{
		//		if (!_settings.AbbreviationsChecked)
		//		{
		//			bundle.Abbreviations = null;
		//		}

		//		if (!_settings.OrdinalFollowersChecked)
		//		{
		//			bundle.OrdinalFollowers = null;
		//		}

		//		if (!_settings.VariablesChecked)
		//		{
		//			bundle.Variables = null;
		//		}

		//		if (!_settings.SegmentationRulesChecked)
		//		{
		//			bundle.SegmentationRules = null;
		//		}

		//		if (!_settings.DatesChecked)
		//		{
		//			bundle.LongDateFormats = null;
		//			bundle.ShortDateFormats = null;
		//		}

		//		if (!_settings.TimesChecked)
		//		{
		//			bundle.LongTimeFormats = null;
		//			bundle.ShortTimeFormats = null;
		//		}

		//		if (!_settings.NumbersChecked)
		//		{
		//			bundle.NumbersSeparators = null;
		//		}

		//		if (!_settings.MeasurementsChecked)
		//		{
		//			bundle.MeasurementUnits = null;
		//		}

		//		if (!_settings.CurrenciesChecked)
		//		{
		//			bundle.CurrencyFormats = null;
		//		}
		//	}
		//}

		private void ExcludeWhatIsNotNeeded(Settings settings, List<LanguageResourceBundle> languageResourceBundles)
		{
			foreach (var languageResourceBundle in languageResourceBundles)
			{
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

		private List<T> GetUnionOfListsOfObjects<T>(List<T> listOne, List<T> listTwo)
		{
			if (listOne != null)
			{
				listTwo = listTwo?.Union(listOne).ToList();
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

		//	if (templateBundleGetter != null && templateBundleGetter.Items.Any())
		//	{
		//		foreach (var abbrev in newBundleGetter.Items)
		//		{
		//			templateBundleGetter.Add(abbrev);
		//		}
		//	}
		//	else
		//	{
		//		templateBundleSetter?.Invoke(template, new object[] { new Wordlist(newBundleGetter) });
		//	}
		//}

		//private void AddItemsToWordlist(LanguageResourceBundle newLanguageResourceBundle, LanguageResourceBundle template, string property)
		//{
		//	var templateBundleGetter = typeof(LanguageResourceBundle).GetProperty(property)?.GetMethod.Invoke(template, null) as Wordlist;
		//	var templateBundleSetter = typeof(LanguageResourceBundle).GetProperty(property)?.SetMethod;
		//	var newBundleGetter = typeof(LanguageResourceBundle).GetProperty(property)?.GetMethod.Invoke(newLanguageResourceBundle, null) as Wordlist;

		//	if (newBundleGetter == null || !newBundleGetter.Items.Any()) return;

		//public T GetPropertyValue<T>(object languageResourceBundle, string property)
		//{
		//	var resObj = languageResourceBundle.GetType().GetProperty(property)?.GetMethod.Invoke(languageResourceBundle, null);
		//	//if (subProperty != null)
		//	//{
		//	//	var subPropValue = typeof(T).GetProperty(subProperty)?.GetMethod.Invoke(resObj, null);
		//	//	return (default, (U)subPropValue);
		//	//}
		//	return (T)resObj;
		//}
	}
}