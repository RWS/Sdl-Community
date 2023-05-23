using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.TranslationMemory;
using Sdl.LanguagePlatform.TranslationMemoryApi;

namespace LanguageWeaverProvider
{
	[TranslationProviderWinFormsUi(Id = "Translation_Provider_Plug_inWinFormsUI",
								   Name = "Translation_Provider_Plug_inWinFormsUI",
								   Description = "Translation_Provider_Plug_inWinFormsUI")]
	internal class MyTranslationProviderWinFormsUI : ITranslationProviderWinFormsUI
	{
		#region ITranslationProviderWinFormsUI Members

		public ITranslationProvider[] Browse(IWin32Window owner, LanguagePair[] languagePairs, ITranslationProviderCredentialStore credentialStore)
		{
			throw new NotImplementedException();
		}

		public bool Edit(IWin32Window owner, ITranslationProvider translationProvider, LanguagePair[] languagePairs, ITranslationProviderCredentialStore credentialStore)
		{
			throw new NotImplementedException();
		}

		public bool GetCredentialsFromUser(IWin32Window owner, Uri translationProviderUri, string translationProviderState, ITranslationProviderCredentialStore credentialStore)
		{
			throw new NotImplementedException();
		}

		public TranslationProviderDisplayInfo GetDisplayInfo(Uri translationProviderUri, string translationProviderState)
		{
			throw new NotImplementedException();
		}

		public bool SupportsEditing
		{
			get { throw new NotImplementedException(); }
		}

		public bool SupportsTranslationProviderUri(Uri translationProviderUri)
		{
			throw new NotImplementedException();
		}

		public string TypeDescription
		{
			get { throw new NotImplementedException(); }
		}

		public string TypeName
		{
			get { throw new NotImplementedException(); }
		}

		#endregion
	}
}
