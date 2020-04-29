using System.Collections.Generic;

namespace Sdl.Community.SdlDataProtectionSuite.SdlTmAnonymizer.Model
{
	public class AnonymizeTranslationMemory
	{	
		public TmFile TmFile { get; set; }	

		public List<TmTranslationUnit> TranslationUnits { get; set; }

		public List<TranslationUnitDetails> TranslationUnitDetails { get; set; }
	}
}
