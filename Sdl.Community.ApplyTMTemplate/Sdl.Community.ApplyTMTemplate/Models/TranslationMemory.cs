using Sdl.Community.ApplyTMTemplate.ViewModels;
using Sdl.LanguagePlatform.TranslationMemoryApi;

namespace Sdl.Community.ApplyTMTemplate.Models
{
	public class TranslationMemory : ModelBase
	{
		private string _checked;
		private bool _isEnabled;
		private bool _isSelected;
		public TranslationMemory(FileBasedTranslationMemory tm)
		{
			_isSelected = false;
			_isEnabled = false;
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

		public bool IsEnabled
		{
			get => _isEnabled;
			set
			{
				_isEnabled = value;
				OnPropertyChanged();
			}
		}

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

		public void ToggleCheckedUnchecked(bool onOff)
		{
			if (onOff)
			{
				Checked = "../Resources/Checked.ico";
			}
			else
			{
				Checked = "../Resources/Unchecked.ico";
			}
		}
	}
}