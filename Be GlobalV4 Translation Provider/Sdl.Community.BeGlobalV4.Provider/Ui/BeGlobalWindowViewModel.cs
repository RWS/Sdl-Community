using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sdl.Community.BeGlobalV4.Provider.Studio;
using Sdl.LanguagePlatform.TranslationMemoryApi;

namespace Sdl.Community.BeGlobalV4.Provider.Ui
{
	public class BeGlobalWindowViewModel
	{
		public BeGlobalTranslationOptions Options { get; set; }

		public BeGlobalWindowViewModel(BeGlobalTranslationOptions options, TranslationProviderCredential credentialStore)
		{
			Options = options;
		}
	}
}
