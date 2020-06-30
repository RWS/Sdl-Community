using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sdl.Community.ApplyTMTemplate.Models.Interfaces;
using Sdl.LanguagePlatform.Core.Tokenization;
using Sdl.LanguagePlatform.TranslationMemory;
using Sdl.LanguagePlatform.TranslationMemoryApi;

namespace Sdl.Community.ApplyTMTemplate.Models
{
	public class Cozonac : FileBasedTranslationMemory, ILanguageResourcesContainer
	{
		public bool ValidateTemplate(bool isImport)
		{
			throw new NotImplementedException();
		}

		public Cozonac(string tmFilePath) : base(tmFilePath)
		{
		}
	}
}
