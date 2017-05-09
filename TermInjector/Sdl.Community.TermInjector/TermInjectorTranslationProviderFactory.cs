using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.TranslationMemory;
using Sdl.LanguagePlatform.TranslationMemoryApi;
using Sdl.Community.TermInjector;
using System.Windows.Forms;

namespace Sdl.Community.TermInjector
{
    #region "Declaration"
    [TranslationProviderFactory(
        Id = "TermInjectorTranslationProviderFactory",
        Name = "TermInjectorTranslationProviderFactory",
        Description = "Inserts terms into new segments or fuzzy matches.")]
    #endregion

    public class TermInjectorTranslationProviderFactory : ITranslationProviderFactory
    {
        #region ITranslationProviderFactory Members
        #region "CreateTranslationProvider"
        private static Dictionary<string,TermInjectorTranslationProvider> activeProviders = new Dictionary<string,TermInjectorTranslationProvider>();
        public ITranslationProvider CreateTranslationProvider(Uri translationProviderUri, string translationProviderState, ITranslationProviderCredentialStore credentialStore)
        {
            var createOptions = new TermInjectorTranslationOptions(translationProviderUri);
            if (createOptions.UseBoundaryCharacters == null)
            {
                createOptions.UseBoundaryCharacters = "true";
            }
            if (createOptions.InstanceGUID == null)
            {
                createOptions.InstanceGUID = Guid.NewGuid().ToString();
            }
            TermInjectorTranslationProvider tp;
            
            //Don't create the provider if it is already active
            if (activeProviders.ContainsKey(createOptions.InstanceGUID))
            {
                if (activeProviders[createOptions.InstanceGUID] != null)
                {
                    return activeProviders[createOptions.InstanceGUID];
                }
                else
                {
                    tp = new TermInjectorTranslationProvider(new TermInjectorTranslationOptions(translationProviderUri));
                    activeProviders[createOptions.InstanceGUID] = tp;
                    return tp;
                }   
            }
            if (!SupportsTranslationProviderUri(translationProviderUri))
            {
                throw new Exception("Cannot handle URI.");
            }
            //if (TermInjectorTranslationProviderFactory.activeProviders.ContainsKey(translationProviderUri.
            
            //Uri has not been added
            tp = new TermInjectorTranslationProvider(new TermInjectorTranslationOptions(translationProviderUri));
            activeProviders.Add(createOptions.InstanceGUID,tp);
            return tp;
            
            
        }
        #endregion

        #region "SupportsTranslationProviderUri"
        public bool SupportsTranslationProviderUri(Uri translationProviderUri)
        {
            if (translationProviderUri == null)
            {
                throw new ArgumentNullException("Translation provider URI not supported.");
            }
            return String.Equals(translationProviderUri.Scheme, TermInjectorTranslationProvider.TermInjectorTranslationProviderScheme, StringComparison.OrdinalIgnoreCase);
        }
        #endregion

        #region "GetTranslationProviderInfo"
        public TranslationProviderInfo GetTranslationProviderInfo(Uri translationProviderUri, string translationProviderState)
        {
            TranslationProviderInfo info = new TranslationProviderInfo();

            #region "TranslationMethod"
            info.TranslationMethod = TermInjectorTranslationOptions.ProviderTranslationMethod;
            #endregion

            #region "Name"
            info.Name = PluginResources.Plugin_NiceName;
            #endregion

            return info;
        }
        #endregion

        #endregion
    }
}
