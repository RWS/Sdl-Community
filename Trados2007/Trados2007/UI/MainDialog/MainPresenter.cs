// ---------------------------------
// <copyright file="MainPresenter.cs" company="SDL International">
// Copyright  2011 All Right Reserved
// </copyright>
// <author>Kostiantyn Lukianets</author>
// <email>klukianets@sdl.com</email>
// <date>2011-11-01</date>
// ---------------------------------
namespace Sdl.TranslationStudio.Plugins.Trados2007.UI
{
    using System;
    using System.Linq;
    using System.Windows.Forms;

    using Sdl.LanguagePlatform.Core;

    public class MainPresenter
    {
        public MainPresenter(IMainView form, MainModel mainModel)
        {
            this.Form = form;
            this.Model = mainModel;
        }

        public IMainView Form { get; private set; }

        public MainModel Model { get; private set; }

        public void SelectFileBasedTranslationMemory()
        {
            var dialog = new OpenFileDialog
                {
                    CheckFileExists = true,
                    DefaultExt = ".tmw",
                    Multiselect = false,
                    Filter = PluginResources.Trados2007_DialogFilterExtension,
                    Title = PluginResources.Trados2007_BrowseDialogTitle
                };

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                string selectedTM = dialog.FileName;
                var result = this.Model.SelectFileBasedProvider(selectedTM);
                if (result)
                {
                    this.Form.Close(DialogResult.OK);
                }
                else
                {
                    this.Form.Close(DialogResult.Cancel);
                }
            }
        }

        public void SelectServerBasedTranslationMemory()
        {
            if (Prerequisites.WarnIfServerManager2007NotInstalled())
            {
                return;
            }

            var dialog = new ServerBasedDialog(this.Model.LanguagePairs.ToArray(), this.Model.CredentialStore);

            if (dialog.ShowDialog(this.Form) == DialogResult.OK)
            {
                var result = this.Model.SelectServerBasedProvider(dialog.SelectedTranslationMemory);

                if (result)
                {
                    this.Form.Close(DialogResult.OK);
                }
                else
                {
                    this.Form.Close(DialogResult.Cancel);
                }
            }
        }

        public void ShowOnlineHelp()
        {
            System.Diagnostics.Process.Start(PluginResources.Trados2007_HelpArticleLink);
        }

        public void CancelDialog()
        {
            this.Form.Close(DialogResult.Cancel);
        }
    }
}
