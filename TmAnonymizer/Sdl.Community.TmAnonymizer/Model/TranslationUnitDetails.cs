using System.Collections.Generic;
using Sdl.LanguagePlatform.TranslationMemory;

namespace Sdl.Community.SdlTmAnonymizer.Model
{
	public class TranslationUnitDetails
	{
		public bool IsSourceMatch { get; set; }
		public bool IsTargetMatch { get; set; }	
		public TranslationUnit TranslationUnit { get; set; }
		public List<WordDetails> SelectedWordsDetails{ get; set; }
		public List<WordDetails> RemovedWordsFromMatches { get; set; }

	}
}
