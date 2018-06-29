using System.Collections.Generic;

namespace Sdl.Community.SdlTmAnonymizer.Model
{
    public class SourceSearchResult : ModelBase
	{
		private bool _tuSelected;
		public string Id { get; set; }
		public string TmSegmentId { get; set; }
		public string SegmentNumber { get; set; }
		public string SourceText { get; set; }
		public string TargetText { get; set; }
		public string TmFilePath { get; set; }
		public string IconFilePath { get; set; }
		public MatchResult MatchResult { get; set; }
		public MatchResult TargetMatchResult { get; set; }
		public bool IsServer { get; set; }
		public List<WordDetails> SelectedWordsDetails { get; set; }	
		public List<WordDetails> DeSelectedWordsDetails { get; set; }
		public List<WordDetails> TargetSelectedWordsDetails { get; set; }
		public List<WordDetails> TargetDeSelectedWordsDetails { get; set; }
		public bool IsSourceMatch { get; set; }
		public bool IsTargetMatch { get; set; }
		public bool TuSelected
		{

			get => _tuSelected;
			set
			{
				_tuSelected = value;
				OnPropertyChanged(nameof(TuSelected));
			}
		}

	}
}
