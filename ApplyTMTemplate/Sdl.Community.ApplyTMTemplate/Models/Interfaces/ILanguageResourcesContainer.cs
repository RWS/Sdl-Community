using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sdl.LanguagePlatform.TranslationMemoryApi;

namespace Sdl.Community.ApplyTMTemplate.Models.Interfaces
{
	public interface ILanguageResourcesContainer
	{
		LanguageResourceBundleCollection LanguageResourceBundles { get; }
		void Save();
	}
}
