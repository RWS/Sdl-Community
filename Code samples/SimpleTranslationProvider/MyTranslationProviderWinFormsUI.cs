using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.TranslationMemory;
using Sdl.LanguagePlatform.TranslationMemoryApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SimpleTranslationProvider
{
    [TranslationProviderWinFormsUi(Id = "Translation_Provider_Plug_inWinFormsUI",
                                   Name = "Translation_Provider_Plug_inWinFormsUI",
                                   Description = "Translation_Provider_Plug_inWinFormsUI")]
    internal class MyTranslationProviderWinFormsUI : ITranslationProviderWinFormsUI
    {
        #region ITranslationProviderWinFormsUI Members

        public bool SupportsEditing => true;
        public string TypeDescription => TRANSLATION_URI;
        public string TypeName => TRANSLATION_URI;

        public const string TRANSLATION_URI = "smtiti";

        public ITranslationProvider[] Browse(IWin32Window owner, LanguagePair[] languagePairs, ITranslationProviderCredentialStore credentialStore)
        {
            return new ITranslationProvider[] { new MyTranslationProvider() };
        }

        public bool Edit(IWin32Window owner, ITranslationProvider translationProvider, LanguagePair[] languagePairs, ITranslationProviderCredentialStore credentialStore)
        {
            return false;
        }

        public bool GetCredentialsFromUser(IWin32Window owner, Uri translationProviderUri, string translationProviderState, ITranslationProviderCredentialStore credentialStore)
        {
            return false;
        }

        public TranslationProviderDisplayInfo GetDisplayInfo(Uri translationProviderUri, string translationProviderState)
        {
            var info = new TranslationProviderDisplayInfo
            {
                Name = TRANSLATION_URI,
                TooltipText = TRANSLATION_URI,
                TranslationProviderIcon = null,
                SearchResultImage = null,
            };
            return info;
        }


        public bool SupportsTranslationProviderUri(Uri translationProviderUri)
        {
            return true;
        }


        #endregion
    }
}
