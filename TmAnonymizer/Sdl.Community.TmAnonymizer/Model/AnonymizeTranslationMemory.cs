using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sdl.LanguagePlatform.TranslationMemory;

namespace Sdl.Community.TmAnonymizer.Model
{
	public class AnonymizeTranslationMemory
	{	
		public string TmPath { get; set; }	
		public List<TranslationUnit> TranslationUnits { get; set; }
		public List<TranslationUnitDetails> TranslationUnitDetails { get; set; }
	}
}
