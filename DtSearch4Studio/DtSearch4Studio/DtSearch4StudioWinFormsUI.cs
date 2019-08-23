using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.TranslationMemory;
using Sdl.LanguagePlatform.TranslationMemoryApi;

namespace DtSearch4Studio
{
	[TranslationProviderWinFormsUi(Id = "DtSearch4StudioUiId",
								   Name = "DtSearch4StudioTranslationProviderUi",
								   Description = "DtSearch4Studio Translation Provider")]
	public class DtSearch4StudioWinFormsUI : ITranslationProviderWinFormsUI
	{
		public string TypeName => "DtSearch4Studio Translation Provider";
		public string TypeDescription => "DtSearch4Studio Translation Provider";

		// To be implemented all the methods /properties bellow
		public ITranslationProvider[] Browse(IWin32Window owner, LanguagePair[] languagePairs, ITranslationProviderCredentialStore credentialStore)
		{
			return null;
		}

		public bool Edit(IWin32Window owner, ITranslationProvider translationProvider, LanguagePair[] languagePairs, ITranslationProviderCredentialStore credentialStore)
		{
			return true;
		}

		public bool GetCredentialsFromUser(IWin32Window owner, Uri translationProviderUri, string translationProviderState, ITranslationProviderCredentialStore credentialStore)
		{
			return true;
		}

		public TranslationProviderDisplayInfo GetDisplayInfo(Uri translationProviderUri, string translationProviderState)
		{
			return null;
		}

		public bool SupportsEditing
		{
			get => true;
		}

		public bool SupportsTranslationProviderUri(Uri translationProviderUri)
		{
			return true;
		}		
	}
}