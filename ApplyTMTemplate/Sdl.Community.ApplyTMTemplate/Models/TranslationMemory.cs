using System.Collections.Specialized;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.CompilerServices;
using Sdl.Community.ApplyTMTemplate.Models.Interfaces;
using Sdl.Core.Globalization;
using Sdl.LanguagePlatform.TranslationMemoryApi;

namespace Sdl.Community.ApplyTMTemplate.Models
{
	public class TranslationMemory : FileBasedTranslationMemory, ILanguageResourcesContainer, INotifyPropertyChanged
	{
		private bool _isSelected;
		private bool? _sourceModified;
		private string _sourceStatusToolTip;
		private bool? _targetModified;
		private string _targetStatusToolTip;

		public TranslationMemory(string tmPath) : base(tmPath)
		{
			_sourceStatusToolTip = PluginResources.Nothing_processed_yet;
			_targetStatusToolTip = PluginResources.Nothing_processed_yet;
			_isSelected = false;
			LanguageResourceBundles.CollectionChanged += LanguageResourceBundles_CollectionChanged;

			var bundles = this.LanguageResourceBundles;

			// Method used to make sure the bundle properties are populated
			// Might remove it when initializing the TM works
			foreach (var bundle in bundles)
			{
				_ = bundle.Abbreviations;
				_ = bundle.CurrencyFormats;
				_ = bundle.LongDateFormats;
				_ = bundle.LongTimeFormats;
				_ = bundle.MeasurementUnits;
				_ = bundle.NumbersSeparators;
				_ = bundle.OrdinalFollowers;
				_ = bundle.SegmentationRules;
				_ = bundle.ShortDateFormats;
				_ = bundle.ShortTimeFormats;
				_ = bundle.Variables;
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;

		public bool IsSelected
		{
			get => _isSelected;
			set
			{
				_isSelected = value;
				OnPropertyChanged(nameof(IsSelected));
			}
		}

		public string SourceLanguage
		{
			get => LanguageDirection.SourceLanguage.Name;
		}

		public Image SourceLanguageFlag => new Language(LanguageDirection.SourceLanguage.Name).GetFlagImage();

		public bool? SourceModified
		{
			get => _sourceModified;
			set
			{
				if (_sourceModified == value || _sourceModified == true && value != null) return;
				_sourceModified = value;
				OnPropertyChanged(nameof(SourceModified));
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
				OnPropertyChanged(nameof(SourceStatusToolTip));
			}
		}

		public string TargetLanguage => LanguageDirection.TargetLanguage.Name;

		public Image TargetLanguageFlag => new Language(LanguageDirection.TargetLanguage.Name).GetFlagImage();

		public bool? TargetModified
		{
			get => _targetModified;
			set
			{
				if (_targetModified == value || _targetModified == true && value != null) return;
				_targetModified = value;
				OnPropertyChanged(nameof(TargetModified));
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
				OnPropertyChanged(nameof(TargetStatusToolTip));
			}
		}

		public void MarkSourceModified()
		{
			SourceModified = true;
			SourceStatusToolTip = PluginResources.SourceModifiedMarker;
		}

		public void MarkSourceNotModified()
		{
			SourceModified = false;
			SourceStatusToolTip = PluginResources.SourceNotModifiedMarker;
		}

		public void MarkTargetModified()
		{
			TargetModified = true;
			TargetStatusToolTip = PluginResources.TargetModifiedMarker;
		}

		public void MarkTargetNotModified()
		{
			TargetModified = false;
			TargetStatusToolTip = PluginResources.TargetNotModified;
		}

		public void ResetAnnotations()
		{
			SourceModified = null;
			TargetModified = null;
			foreach (var languageResourceBundle in LanguageResourceBundles)
			{
				languageResourceBundle.PropertyChanged += LanguageResourceBundle_PropertyChanged;
			}
		}

		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		private void LanguageResourceBundle_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (((LanguageResourceBundle)sender).Language.Name.Equals(LanguageDirection.SourceLanguage.Name))
			{
				MarkSourceModified();
				LanguageResourceBundles[LanguageDirection.SourceLanguage].PropertyChanged -=
					LanguageResourceBundle_PropertyChanged;
				MarkTargetNotModified();
			}
			if (((LanguageResourceBundle)sender).Language.Name.Equals(LanguageDirection.TargetLanguage.Name))
			{
				MarkTargetModified();
				LanguageResourceBundles[LanguageDirection.TargetLanguage].PropertyChanged -=
					LanguageResourceBundle_PropertyChanged;
				MarkSourceNotModified();
			}
		}

		private void LanguageResourceBundles_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			if (e.Action == NotifyCollectionChangedAction.Add)
			{
				if (LanguageDirection.SourceLanguage.Name.Equals(
					((LanguageResourceBundle)e.NewItems[0]).Language.Name))
				{
					MarkSourceModified();
				}
				if (LanguageDirection.TargetLanguage.Name.Equals(
					((LanguageResourceBundle)e.NewItems[0]).Language.Name))
				{
					MarkTargetModified();
				}

				((LanguageResourceBundle)e.NewItems[0]).PropertyChanged += LanguageResourceBundle_PropertyChanged;
			}
		}
	}
}