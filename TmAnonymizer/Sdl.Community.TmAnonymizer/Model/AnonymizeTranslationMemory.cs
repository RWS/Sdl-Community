using System.Collections.Generic;
using Sdl.LanguagePlatform.TranslationMemory;

namespace Sdl.Community.SdlTmAnonymizer.Model
{
	public class AnonymizeTranslationMemory
	{	
		public string TmPath { get; set; }	

		public List<TranslationUnit> TranslationUnits { get; set; }

		public List<TranslationUnitDetails> TranslationUnitDetails { get; set; }
	}
}
