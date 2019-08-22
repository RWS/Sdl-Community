using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.TranslationMemory;
using Sdl.LanguagePlatform.TranslationMemoryApi;

namespace DtSearch4Studio
{
	[TranslationProviderFactory(Id = "DtSearch4StudioId", Name = "DtSearch4Studio", Description = "DtSearch4Studio")]
	class DtSearch4StudioProviderFactory : ITranslationProviderFactory
	{
		#region ITranslationProviderFactory Members
		// To be implemented all the methods /properties bellow
		public ITranslationProvider CreateTranslationProvider(Uri translationProviderUri, string translationProviderState, ITranslationProviderCredentialStore credentialStore)
		{
			return null;
		}

		public TranslationProviderInfo GetTranslationProviderInfo(Uri translationProviderUri, string translationProviderState)
		{
			return null;
		}

		public bool SupportsTranslationProviderUri(Uri translationProviderUri)
		{
			return true;
		}

		#endregion
	}
}
