namespace Sdl.Community.StarTransit.Shared.Models
{
	public class StarTranslationMemoryMetadata : BaseViewModel
	{
		private int _tmPanalty;
		private bool _isChecked;
		public string SourceFile { get; set; }

		//Local path for TM/MT File
		public string TargetFile { get; set; }
		public string SourceLanguage { get; set; }
		public string TargetLanguage { get; set; }

		//Used to mark if is a Transit TM or a MT file
		public bool IsMtFile { get; set; }
		public string Name { get; set; }


		public int TmPenalty
		{
			get => _tmPanalty;
			set
			{
				if (_tmPanalty == value) return;
				_tmPanalty = value;
				OnPropertyChanged(nameof(TmPenalty));
			}
		}

		public bool IsChecked
		{
			get => _isChecked;
			set
			{
				if (_isChecked == value) return;
				_isChecked = value;
				OnPropertyChanged(nameof(IsChecked));
			}
		}
	}
}
