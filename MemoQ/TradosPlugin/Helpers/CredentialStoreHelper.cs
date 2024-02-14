using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.TranslationMemory;
using Sdl.LanguagePlatform.TranslationMemoryApi;

using TMProvider;

namespace TradosPlugin
{
    public static class CredentialStoreHelper
    {
        public static void AddCredentialsToProviderFromStore(ITranslationProviderCredentialStore credStore, MemoQTranslationProvider transProvider)
        {
            if (transProvider.MemoQTMProviders == null || transProvider.MemoQTMProviders.Count == 0) return;

            foreach (TMProviderBase p in transProvider.MemoQTMProviders)
            {
                TranslationProviderCredential userData = credStore.GetCredential(new Uri(p.Settings.URL));
                if (userData != null)
                {
                    string[] cred = userData.Credential.Split(new string[] { " ;" }, StringSplitOptions.None);
                    if (cred.Length == 2)
                    {
                        p.Settings.UserName = cred[0];
                        p.Settings.Password = cred[1];
                    }
                }
            }
        }

    }
}
