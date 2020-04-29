using System.Collections.Generic;
using System.IO;

namespace Sdl.Community.SdlTmAnonymizer.Model
{
    public class ContentSearchResult : ModelBase
	{

		public ContentSearchResult()
		{
			SelectedWordsDetails = new List<WordDetails>();
			DeSelectedWordsDetails = new List<WordDetails>();
			TargetDeSelectedWordsDetails = new List<WordDetails>();
			TargetSelectedWordsDetails = new List<WordDetails>();
		}

		private bool _tuSelected;		
		public TmTranslationUnit TranslationUnit { get; set; }		
		public string SourceText { get; set; }
		public string TargetText { get; set; }
		public string TmFilePath { get; set; }

		public string TmFileName => !string.IsNullOrEmpty(TmFilePath) ? Path.GetFileName(TmFilePath) : string.Empty;

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
