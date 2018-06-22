using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;

namespace Sdl.Community.TmAnonymizer.Model
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
		public object Document { get; set; }
		public MatchResult MatchResult { get; set; }
		public MatchResult TargetMatchResult { get; set; }
		public bool IsServer { get; set; }
		public List<WordDetails> SelectedWordsDetails { get; set; }	
		public List<WordDetails> DeSelectedWordsDetails { get; set; }
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
