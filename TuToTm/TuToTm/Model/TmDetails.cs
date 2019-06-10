using System.Drawing;
using Sdl.LanguagePlatform.TranslationMemoryApi;

namespace Sdl.Community.TuToTm.Model
{
	public class TmDetails:BaseModel
	{
		private string _name;
		private bool _isSelected;	

		public string Name
		{
			get => _name;
			set
			{
				_name = value;
				OnPropertyChanged(nameof(Name));
			}
		}
		public bool IsSelected
		{
			get => _isSelected;
			set
			{
				_isSelected = value;
				OnPropertyChanged(nameof(IsSelected));
			}
		}

		public string SourceLanguage { get; set; }
		public string TargetLanguage { get; set; }
		public FileBasedTranslationMemory FileBasedTranslationMemory { get; set; }
		public string TmPath { get; set; }
		public Image SourceFlag { get; set; }
		public Image TargetFlag { get; set; }
		public string Description { get; set; }
	}
}
	