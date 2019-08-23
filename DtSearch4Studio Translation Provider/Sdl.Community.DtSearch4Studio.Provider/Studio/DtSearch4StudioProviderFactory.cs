using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.TranslationMemory;
using Sdl.LanguagePlatform.TranslationMemoryApi;

namespace Sdl.Community.DtSearch4Studio.Provider.Studio
{
	[TranslationProviderFactory(Id = "DtSearch4StudioFactoryId", Name = "DtSearch4StudioFactory", Description = "DtSearch4Studio Translation Provider Factory")]
	public class DtSearch4StudioProviderFactory : ITranslationProviderFactory
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
