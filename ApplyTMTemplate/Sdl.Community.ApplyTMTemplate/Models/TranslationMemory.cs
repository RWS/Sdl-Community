using Sdl.Community.ApplyTMTemplate.ViewModels;
using Sdl.LanguagePlatform.TranslationMemoryApi;

namespace Sdl.Community.ApplyTMTemplate.Models
{
	public class TranslationMemory : ModelBase
	{
		private string _checked;
		private bool _isSelected;

		public TranslationMemory(FileBasedTranslationMemory tm)
		{
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

		public string Name => Tm.Name;

		public FileBasedTranslationMemory Tm { get; }

		public void MarkTmApplied()
		{
			if (Checked != "") return;

			Checked = "../Resources/Checked.ico";
		}

		public void MarkTmNotApplied()
		{
			if (Checked != "") return;

			Checked = "../Resources/Unchecked.ico";
		}
	}
}