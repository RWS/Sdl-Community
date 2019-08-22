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
	[TranslationProviderWinFormsUi(Id = "Translation_Provider_Plug_inWinFormsUI",
								   Name = "Translation_Provider_Plug_inWinFormsUI",
								   Description = "Translation_Provider_Plug_inWinFormsUI")]
	public class DtSearch4StudioWinFormsUI : ITranslationProviderWinFormsUI
	{
		#region ITranslationProviderWinFormsUI Members

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

		public string TypeDescription
		{
			get => string.Empty;
		}

		public string TypeName
		{
			get => string.Empty;
		}

		#endregion
	}
}
