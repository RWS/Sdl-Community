using System;
using System.Windows.Forms;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.TranslationMemoryApi;

namespace SampleTranslationProvider
{
    [TranslationProviderWinFormsUi(
        Id = "SampleTranslationProviderWinFormsUI",
        Name = "SampleTranslationProviderWinFormsUI",
        Description = "SampleTranslationProviderWinFormsUI")]
    public class SampleTranslationProviderWinFormsUI : ITranslationProviderWinFormsUI
    {
       
        public ITranslationProvider[] Browse(IWin32Window owner, LanguagePair[] languagePairs, ITranslationProviderCredentialStore credentialStore)
        {
            SampleTranslationProvider testProvider = new SampleTranslationProvider();
            return new ITranslationProvider[] { testProvider };
        }
      

       
        public bool SupportsEditing
        {
            get { return true; }
        }
      
        public bool Edit(IWin32Window owner, ITranslationProvider translationProvider, LanguagePair[] languagePairs, ITranslationProviderCredentialStore credentialStore)
        {
            SampleTranslationProvider editProvider = translationProvider as SampleTranslationProvider;
            if (editProvider == null)
            {
                return false;
            }

            return true;
        }
        
        
        public bool GetCredentialsFromUser(IWin32Window owner, Uri translationProviderUri, string translationProviderState, ITranslationProviderCredentialStore credentialStore)
        {
            return true;
        }
        
        
        public TranslationProviderDisplayInfo GetDisplayInfo(Uri translationProviderUri, string translationProviderState)
        {
            TranslationProviderDisplayInfo info = new TranslationProviderDisplayInfo
            {
                Name = PluginResources.Plugin_NiceName,
                TranslationProviderIcon = PluginResources.band_aid_icon,
                TooltipText = PluginResources.Plugin_Tooltip,
                SearchResultImage = PluginResources.band_aid_symbol
            };

            return info;
        }
        

        public bool SupportsTranslationProviderUri(Uri translationProviderUri)
        {
            if (translationProviderUri == null)
            {
                throw new ArgumentNullException("URI not supported by the plug-in.");
            }
            return string.Equals(translationProviderUri.Scheme, SampleTranslationProvider.ListTranslationProviderScheme, StringComparison.CurrentCultureIgnoreCase);
        }

        public string TypeDescription => PluginResources.Plugin_Description;

        public string TypeName => PluginResources.Plugin_NiceName;
    }
}
