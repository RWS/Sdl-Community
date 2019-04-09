using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sdl.LanguagePlatform.TranslationMemoryApi;

namespace Sdl.Community.ApplyTMTemplate.Models
{
	public interface IResource
	{
		void AddLanguageResourceToBundle(LanguageResourceBundle langResBundle);
	}
}
