using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Sdl.Community.ApplyTMTemplate.Utilities;
using Sdl.Community.ApplyTMTemplate.ViewModels;
using Sdl.Core.Globalization;
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
			_sourceStatusToolTip = "Nothing processed yet";
			_targetStatusToolTip = "Nothing processed yet";
			_isSelected = false;
			_sourceStatus = "";
			_targetStatus = "";
			Tm = tm;
			SourceLanguageFlag = new Language(tm.LanguageDirection.SourceLanguage).GetFlagImage();
			TargetLanguageFlag = new Language(tm.LanguageDirection.TargetLanguage).GetFlagImage();
			SourceLanguage = tm.LanguageDirection.SourceLanguage.Name;
			TargetLanguage = tm.LanguageDirection.TargetLanguage.Name;
		}

		public Image SourceLanguageFlag { get; set; }
		public Image TargetLanguageFlag { get; set; }

		public string SourceLanguage { get; set; }
		public string TargetLanguage { get; set; }

		public string SourceStatus
		{
			get => _sourceStatus;
			set
			{
				if (_sourceStatus.ToLower().Contains("unchecked") || _sourceStatus == string.Empty)
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
				if (_targetStatus.ToLower().Contains("unchecked") || _targetStatus == string.Empty)
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

		public void ResetAnnotations()
		{
			_sourceStatus = string.Empty;
			_sourceStatusToolTip = string.Empty;
			_targetStatus = string.Empty;
			_targetStatusToolTip = string.Empty;
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
			var templateBundle = (typeof(LanguageResourceBundle).GetProperty(property)?.GetMethod.Invoke(template, null) as Wordlist);
			var templateBundleSetter = typeof(LanguageResourceBundle).GetProperty(property)?.SetMethod;
			var newBundleGetter = (typeof(LanguageResourceBundle).GetProperty(property)?.GetMethod.Invoke(newLanguageResourceBundle, null) as Wordlist);

			if (newBundleGetter == null || !newBundleGetter.Items.Any()) return;

			if (templateBundle != null && templateBundle.Items.Any())
			{
				foreach (var abbrev in newBundleGetter.Items)
				{
					templateBundle.Add(abbrev);
				}
			}
			else
			{
				templateBundleSetter?.Invoke(template, new object[] { new Wordlist(newBundleGetter) });
			}
		}

		private void AddLanguageResourceBundleToTm(LanguageResourceBundle languageResourceBundle)
		{
			AddEmptyLanguageResourceBundles();

			MarkSourceNotModified();
			MarkTargetNotModified();

			var cultureOfNewBundle = languageResourceBundle.Language;
			var cultureOfSource = Tm.LanguageDirection.SourceLanguage;
			var cultureOfTarget = Tm.LanguageDirection.TargetLanguage;
			var thisLangResIsValid = false;

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

		private void AddEmptyLanguageResourceBundles()
		{
			if (Tm.LanguageResourceBundles.Count >= 2) return;
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