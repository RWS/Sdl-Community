// ---------------------------------
// <copyright file="ServerBasedModel.cs" company="SDL International">
// Copyright  2011 All Right Reserved
// </copyright>
// <author>Kostiantyn Lukianets</author>
// <email>klukianets@sdl.com</email>
// <date>2011-11-24</date>
// ---------------------------------
namespace Sdl.TranslationStudio.Plugins.Trados2007.UI
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Sdl.LanguagePlatform.TranslationMemoryApi;

    public class ServerBasedModel
    {
        public ServerBasedModel(Sdl.LanguagePlatform.Core.LanguagePair[] languagePairs, ITranslationProviderCredentialStore credentialStore)
        {
            this.LanguagePairs = languagePairs;
            this.CredentialStore = credentialStore;

            // this property is serialized via TP2007ServerBasedDialogSerializationHelper class that's why it could not be null
            this.Servers = new List<Trados2007ServerAccount>(); 
        }

        public void Initialize(TP2007ServerBasedDialogSerializationHelper helper)
        {
            if (helper.Servers == null)
                return;

            List<Trados2007ServerAccount> accounts = new List<Trados2007ServerAccount>(helper.Servers.Length);
            foreach (var serverString in helper.Servers)
            {
                Uri serverUri;
                try
                {
                    serverUri = new Uri(string.Format("{0}://{1}", "http", serverString));
                }
                catch (UriFormatException)
                {
                    continue;
                }

                TranslationProviderCredential credential = this.CredentialStore.GetCredential(serverUri);

                string username;
                string password;
                if (CredentialsUtility.TryParse(credential, out username, out password))
                {
                    Trados2007ServerAccount account = new Trados2007ServerAccount(serverString, username, password);
                    accounts.Add(account);
                }
            }

            this.Servers = accounts;
        }

        public IList<Trados2007ServerAccount> Servers { get; set; }

        public Sdl.LanguagePlatform.Core.LanguagePair[] LanguagePairs { get; set; }

        public ServerBasedTrados2007TranslationMemory SelectedTranslationMemory { get; set; }

        public Trados2007ServerAccount SelectedServer { get; set; }

        public IList<ServerBasedTrados2007TranslationMemory> TranslationMemories { get; set; }

        public ITranslationProviderCredentialStore CredentialStore { get; set; }
    }
}
