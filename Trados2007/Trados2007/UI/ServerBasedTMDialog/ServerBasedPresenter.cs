// ---------------------------------
// <copyright file="ServerBasedPresenter.cs" company="SDL International">
// Copyright  2011 All Right Reserved
// </copyright>
// <author>Kostiantyn Lukianets</author>
// <email>klukianets@sdl.com</email>
// <date>2011-11-01</date>
// ---------------------------------
namespace Sdl.TranslationStudio.Plugins.Trados2007.UI
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using System.ComponentModel;

    internal class ServerBasedPresenter
    {
        private ServerBasedForm form;
        
        private ServerBasedModel model;

        public ServerBasedPresenter(ServerBasedForm form, ServerBasedModel model)
        {
            this.form = form;
            this.model = model;
        }

        public void ShowServersDialog()
        {
            ServersForm dialog = new ServersForm(model.Servers, model.CredentialStore);
            dialog.ShowDialog();

            model.Servers = dialog.Servers;

            Update();
        }

        public void OnSelectTranslationMemory()
        {
            model.SelectedTranslationMemory = model.TranslationMemories[form.SelectedTranslationMemory];
        }

        public void OnSelectServer()
        {
            model.SelectedServer = form.SelectedServer;
            OnConnectToTradosServer();
        }

        private BackgroundWorker worker;

        public void OnConnectToTradosServer()
        {
            if (model.SelectedServer != null)
            {
                if (!model.SelectedServer.IsServerUp)
                {
                    MessagingHelpers.ShowInformation(this.form, string.Format(PluginResources.Information_ServerIsDown, model.SelectedServer));
                }

                this.worker = new BackgroundWorker();

                this.worker.DoWork += (obj, args) =>
                    {
                        args.Result = model.SelectedServer.GetTranslationMemories();
                    };

                this.worker.RunWorkerCompleted += (obj, args) =>
                    {
                        model.TranslationMemories = args.Result as IList<ServerBasedTrados2007TranslationMemory>;
                        form.TranslationMemories = model.TranslationMemories;
                        form.StopInfiniteProgress();
                    };

                form.StartInfiniteProgress();
                this.worker.RunWorkerAsync();
            }
        }

        internal void Update()
        {
            form.Servers = model.Servers;

            if (model.SelectedServer == null)
            {
                model.TranslationMemories = null;
                model.SelectedTranslationMemory = null;
            }

            form.LanguagePairs = model.LanguagePairs;
            form.TranslationMemories = model.TranslationMemories;
        }
    }
}
