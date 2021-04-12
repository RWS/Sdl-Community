namespace Sdl.Community.StarTransit.Shared.Models
{
	public class StarTranslationMemoryMetadata : BaseViewModel
	{
		private int _tmPanalty;

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
	}
}
