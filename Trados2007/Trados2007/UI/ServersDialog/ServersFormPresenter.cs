using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sdl.TranslationStudio.Plugins.Trados2007.UI.ServersDialog
{
    class ServersFormPresenter
    {
        private ServersFormModel model;
        private ServersForm form;
        private ServersForm serversForm;
        private Sdl.LanguagePlatform.TranslationMemoryApi.ITranslationProviderCredentialStore credentialStore;
        
        public ServersFormPresenter(ServersForm serversForm, ServersFormModel model, Sdl.LanguagePlatform.TranslationMemoryApi.ITranslationProviderCredentialStore credentialStore)
        {
            this.form = serversForm;
            this.model = model;

            this.credentialStore = credentialStore;

            this.form.Servers = model.Servers;
        }
        
        internal void OnAdd()
        {
            AddEditServerDialog dialog = new AddEditServerDialog();

            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK && dialog.ServerAccount != null)
            {
                UpdateCredentials(dialog.ServerAccount);

                model.Servers.Add(dialog.ServerAccount);

                form.Servers = model.Servers;
            }
        }

        private void UpdateCredentials(Trados2007ServerAccount account)
        {
            var credentials =
               CredentialsUtility.CreateTranslationProviderCredential(account.Login, account.Password);

            this.credentialStore.AddCredential(account.ConnectionPointUri, credentials);
        }

        internal void OnEdit()
        {
            var selectedServer = GetSelectedServer();
            
            AddEditServerDialog dialog = new AddEditServerDialog(selectedServer);

            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK && dialog.ServerAccount != null)
            {
                UpdateCredentials(dialog.ServerAccount);

                model.Servers.Remove(selectedServer);

                model.Servers.Add(dialog.ServerAccount);

                form.Servers = model.Servers;
            }
        }

        internal void OnDelete()
        {
            if (MessagingHelpers.AskYesNoQuestion(form, PluginResources.Trados2007_ServersDialog_Delete) == System.Windows.Forms.DialogResult.Yes)
            {
                var selectedServer = GetSelectedServer();

                model.Servers.Remove(selectedServer);

                form.Servers = model.Servers;
            }
        }

        private Trados2007ServerAccount GetSelectedServer()
        {
            var selectedServer = model.Servers[form.SelectedServer];
            return selectedServer;
        }

        internal void OnCheckServer()
        {
            var selectedServer = GetSelectedServer();

            var status = model.GetServerStatus(selectedServer);

            form.UpdateServerStatus(status, form.SelectedServer);
        }
    }
}
