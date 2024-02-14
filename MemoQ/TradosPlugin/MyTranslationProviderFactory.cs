using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.TranslationMemory;
using Sdl.LanguagePlatform.TranslationMemoryApi;
using TMProvider;

namespace TradosPlugin
{
    [TranslationProviderFactory(
        Id = "memoQTMPluginFactory",
        Name = "memoQTMPluginFactory",
        Description = "memoQTMPluginFactory")]
    public class MyTranslationProviderFactory : ITranslationProviderFactory
    {
        #region ITranslationProviderFactory Members

        public ITranslationProvider CreateTranslationProvider(Uri translationProviderUri, string translationProviderState, ITranslationProviderCredentialStore credentialStore)
        {
            Log.ClearLogFile();
      
            List<TMProviderBase> providers = new List<TMProviderBase>();
            GeneralTMSettings generalSettings = new GeneralTMSettings(true, false, false, false);
            try
            {
                List<MemoQTMSettings> providerSettings = new List<MemoQTMSettings>();
                generalSettings = SettingsURICreator.CreateSettingsFromURI(translationProviderUri, out providerSettings);
                foreach (MemoQTMSettings settings in providerSettings)
                {
                    try
                    {
                        if (settings.ProviderType == MemoQTMProviderTypes.MemoQServer)
                        {
                            providers.Add(new TMProviderMQServer(settings));
                        }
                    }
                    catch(Exception e)
                    { 
                        // just don't add the provider and move on
                        Log.WriteToLog(e.ToString());
                        // if we throw an error, the whole plugin is disabled
                    }
                }
                MemoQTranslationProvider mqstmp = new MemoQTranslationProvider(generalSettings, providers);
                CredentialStoreHelper.AddCredentialsToProviderFromStore(credentialStore, mqstmp);
                return mqstmp;
            }
            catch (Exception e)
            {
                ExceptionHelper.ShowMessage(e);
                Log.WriteToLog(e.ToString());
                return new MemoQTranslationProvider(generalSettings, providers); 
            }
        }

        public TranslationProviderInfo GetTranslationProviderInfo(Uri translationProviderUri, string translationProviderState)
        {
            TranslationProviderInfo tpi = new TranslationProviderInfo();
            tpi.Name = PluginResources.Plugin_Name;
            tpi.TranslationMethod = TranslationMethod.TranslationMemory;
            return tpi;
        }

        public bool SupportsTranslationProviderUri(Uri translationProviderUri)
        {
            return true;
        }

        #endregion
    }
}
