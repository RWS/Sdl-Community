using System;
using Sdl.Community.ApplyTMTemplate.ViewModels;
using Sdl.LanguagePlatform.TranslationMemoryApi;

namespace Sdl.Community.ApplyTMTemplate.Models
{
	public class TranslationMemory : ModelBase
	{
		private string _checked;
		private string _statusToolTip;
		private bool _isSelected;

		public TranslationMemory(FileBasedTranslationMemory tm)
		{
			_statusToolTip = "Nothing processed yet";
			_isSelected = false;
			_checked = "";
			Tm = tm;
		}

		public string Checked
		{
			get => _checked;
			set
			{
				_checked = value;
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
				OnPropertyChanged();
			}
		}

		public string StatusToolTip
		{
			get => _statusToolTip;
			set
			{
				_statusToolTip = value;
				OnPropertyChanged();
			}
		}

		public string Name => Tm.Name;

		public FileBasedTranslationMemory Tm { get; }

		public void MarkTmApplied()
		{
			if (Checked != "") return;

			Checked = "../Resources/Checked.ico";
			StatusToolTip = "Template applied";
		}

		public void MarkTmNotApplied()
		{
			if (Checked != "") return;

			Checked = "../Resources/Unchecked.ico";
			StatusToolTip = "Template's languages don't correspond with any of this TM's languages";
		}

		public void MarkTmCorrupted()
		{
			if (Checked != "") return;

			Checked = "../Resources/Error.ico";
			StatusToolTip = "This TM is corrupted or the file is not a TM";
		}

		public void ApplyTemplate(LanguageResourceBundle languageResourceBundle)
		{
			var langDirOfTm = Tm.LanguageDirection;

			try
			{
				if (langDirOfTm.SourceLanguage.Equals(languageResourceBundle.Language) ||
				    langDirOfTm.TargetLanguage.Equals(languageResourceBundle.Language))
				{
					Tm.LanguageResourceBundles.Add(languageResourceBundle);
					Tm.Save();
					MarkTmApplied();
				}
				else
				{
					MarkTmNotApplied();
				}
			}
			catch (Exception e)
			{
				MarkTmCorrupted();
			}

		}
	}
}