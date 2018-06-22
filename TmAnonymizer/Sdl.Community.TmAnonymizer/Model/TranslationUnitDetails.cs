using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sdl.LanguagePlatform.TranslationMemory;

namespace Sdl.Community.TmAnonymizer.Model
{
	public class TranslationUnitDetails
	{
		public TranslationUnit TranslationUnit { get; set; }

		public List<WordDetails> SelectedWordsDetails{ get; set; }
		public List<WordDetails> RemovedWordsFromMatches { get; set; }

	}
}
