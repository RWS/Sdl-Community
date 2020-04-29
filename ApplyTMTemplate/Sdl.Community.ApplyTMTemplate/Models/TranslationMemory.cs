using System;
using System.Collections.Generic;
using System.Linq;
using Sdl.Community.ApplyTMTemplate.Utilities;
using Sdl.Community.ApplyTMTemplate.ViewModels;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.Core.Segmentation;
using Sdl.LanguagePlatform.TranslationMemoryApi;

namespace Sdl.Community.ApplyTMTemplate.Models
{
	public class TranslationMemory : ModelBase
	{
		private string _sourceStatus;
		private string _targetStatus;
		private string _sourceStatusToolTip;
		private string _targetStatusToolTip;
		private bool _isSelected;

		public TranslationMemory(FileBasedTranslationMemory tm)
		{
			var languageFlags = new LanguageFlags();
			_sourceStatusToolTip = "Nothing processed yet";
			_targetStatusToolTip = "Nothing processed yet";
			_isSelected = false;
			_sourceStatus = "";
			_targetStatus = "";
			Tm = tm;
			SourceLanguageFlagUri = languageFlags.GetImageStudioCodeByLanguageCode(tm.LanguageDirection.SourceLanguage.Name);
			TargetLanguageFlagUri= languageFlags.GetImageStudioCodeByLanguageCode(tm.LanguageDirection.TargetLanguage.Name);
			SourceLanguage = tm.LanguageDirection.SourceLanguage.Name;
			TargetLanguage = tm.LanguageDirection.TargetLanguage.Name;
		}

		public string SourceLanguageFlagUri { get; set; }
		public string TargetLanguageFlagUri { get; set; }

		public string SourceLanguage { get; set; }
		public string TargetLanguage { get; set; }

		public string SourceStatus
		{
			get => _sourceStatus;
			set
			{
				if (_sourceStatus.ToLower().Contains("unchecked") || _sourceStatus == "")
				{
					_sourceStatus = value;
				}

				OnPropertyChanged();
			}
		}

		public string Icon => @"../Resources/FileBasedTM.ico";

		public bool IsSelected
		{
			get => _isSelected;
			set
			{
				_isSelected = value;
				OnPropertyChanged(nameof(IsSelected));
			}
		}

		public string SourceStatusToolTip
		{
			get => _sourceStatusToolTip;
			set
			{
				if (!_sourceStatusToolTip.Equals(PluginResources.SourceModifiedMarker))
				{
					_sourceStatusToolTip = value;
				}
				OnPropertyChanged();
			}
		}

		public string Name => Tm.Name;

		public FileBasedTranslationMemory Tm { get; }

		public string TargetStatus
		{
			get => _targetStatus;
			set
			{
				if (_targetStatus.ToLower().Contains("unchecked") || _targetStatus == "")
				{
					_targetStatus = value;
				}

				OnPropertyChanged();
			}
		}

		public string TargetStatusToolTip
		{
			get => _targetStatusToolTip;
			set
			{
				if (!_targetStatusToolTip.Equals(PluginResources.TargetModifiedMarker))
				{
					_targetStatusToolTip = value;
				}
				OnPropertyChanged();
			}
		}

		public void MarkSourceModified()
		{
			SourceStatus = "../Resources/Checked.ico";
			SourceStatusToolTip = PluginResources.SourceModifiedMarker;
		}

		public void MarkSourceNotModified()
		{
			SourceStatus = "../Resources/Unchecked.ico";
			SourceStatusToolTip = PluginResources.SourceNotModifiedMarker;
		}

		public void MarkTargetModified()
		{
			TargetStatus = "../Resources/Checked.ico";
			TargetStatusToolTip = PluginResources.TargetModifiedMarker;
		}

		public void MarkTargetNotModified()
		{
			TargetStatus = "../Resources/Unchecked.ico";
			TargetStatusToolTip = PluginResources.TargetNotModified;
		}

		public void MarkTmCorrupted()
		{
			SourceStatus = "../Resources/Error.ico";
			TargetStatus = "../Resources/Error.ico";
			SourceStatusToolTip = PluginResources.TmCorruptedMarker;
			TargetStatusToolTip = PluginResources.TmCorruptedMarker;
		}

		public void UnmarkTm()
		{
			_sourceStatus = "";
			_sourceStatusToolTip = "";
			_targetStatus = "";
			_targetStatusToolTip = "";
		}

		public void ApplyTemplate(LanguageResourceBundle languageResourceBundle)
		{
			try
			{
				AddLanguageResourceBundleToTm(languageResourceBundle);
			}
			catch (Exception)
			{
				MarkTmCorrupted();
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
					if (correspondingBundleInTemplate.SegmentationRules.Rules.All(oldRule => !string.Equals(newRule.Description.Text, oldRule.Description.Text, StringComparison.OrdinalIgnoreCase)))
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

		private static void AddItemsToWordlist(LanguageResourceBundle newLanguageResourceBundle, LanguageResourceBundle template, string property)
		{
			var templateBundleGetter = (typeof(LanguageResourceBundle).GetProperty(property)?.GetMethod.Invoke(template, null) as Wordlist);
			var templateBundleSetter = typeof(LanguageResourceBundle).GetProperty(property)?.SetMethod;
			var newBundleGetter = (typeof(LanguageResourceBundle).GetProperty(property)?.GetMethod.Invoke(newLanguageResourceBundle, null) as Wordlist);

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
				templateBundleSetter?.Invoke(template, new[] { new Wordlist(newBundleGetter) });
			}
		}

		private void AddLanguageResourceBundleToTm(LanguageResourceBundle languageResourceBundle)
		{
			ValidateTm();

			MarkSourceNotModified();
			MarkTargetNotModified();

			var cultureOfNewBundle = languageResourceBundle.Language;
			var cultureOfSource = Tm.LanguageDirection.SourceLanguage;
			var cultureOfTarget = Tm.LanguageDirection.TargetLanguage;
			bool thisLangResIsValid = false;

			if (cultureOfNewBundle.Equals(cultureOfSource))
			{
				MarkSourceModified();
				thisLangResIsValid = true;
			}

			if (cultureOfNewBundle.Equals(cultureOfTarget))
			{
				MarkTargetModified();
				thisLangResIsValid = true;
			}

			if (!thisLangResIsValid) return;

			var properties = new List<string> { "Abbreviations", "OrdinalFollowers", "Variables" };
			foreach (var property in properties)
			{
				AddItemsToWordlist(languageResourceBundle, Tm.LanguageResourceBundles[cultureOfNewBundle], property);
			}
			AddSegmentationRulesToBundle(languageResourceBundle, Tm.LanguageResourceBundles[cultureOfNewBundle]);
			Tm.Save();
		}

		private void ValidateTm()
		{
			if (Tm.LanguageResourceBundles.Count < 2)
			{
				var sourceLanguage = Tm?.LanguageDirection?.SourceLanguage;
				var targetLanguage = Tm?.LanguageDirection?.TargetLanguage;
				if (Tm.LanguageResourceBundles[sourceLanguage] == null)
				{
					Tm.LanguageResourceBundles.Add(new LanguageResourceBundle(sourceLanguage));
				}

				if (Tm.LanguageResourceBundles[targetLanguage] == null)
				{
					Tm.LanguageResourceBundles.Add(new LanguageResourceBundle(targetLanguage));
				}
			}
		}
	}
}