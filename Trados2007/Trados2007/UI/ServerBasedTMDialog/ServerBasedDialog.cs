// ---------------------------------
// <copyright file="ServerBasedDialog.cs" company="SDL International">
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
    using System.Globalization;
    using System.IO;
    using System.Reflection;
    using System.Windows.Forms;
    using System.Xml.Serialization;

    using Sdl.LanguagePlatform.Core;
    using Sdl.LanguagePlatform.TranslationMemoryApi;

    /// <summary>
    /// Wrapper around Server-based TM selection logic & UI
    /// </summary>
    public sealed class ServerBasedDialog
    {
        #region Fields

        /// <summary>
        /// Path to Trados 2007 TP.state file.
        /// </summary>
        private static readonly string StatePath = string.Format(
            CultureInfo.CurrentCulture,
            "{0}{1}SDL{1}OpenExchange{1}SDL Trados2007 TranslationProvider{1}{2}{1}Trados2007TranslationProvider.state",
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            Path.DirectorySeparatorChar,
            Assembly.GetExecutingAssembly().GetName().Version);

        private ServerBasedModel model;

        private ServerBasedForm form;

        private ServerBasedPresenter presenter;

        private IList<LanguagePair> languagePairs;

        private ITranslationProviderCredentialStore credentialStore;

        #endregion // Fields

        #region Ctor

        public ServerBasedDialog(LanguagePair[] languagePairs, ITranslationProviderCredentialStore credentialStore)
        {
            this.model = new ServerBasedModel(languagePairs, credentialStore);
            this.form = new ServerBasedForm();
            this.presenter = new ServerBasedPresenter(this.form, this.model);
            this.form.Presenter = this.presenter;

            this.languagePairs = languagePairs;
            this.credentialStore = credentialStore;

            this.Deserialize();
        }

        #endregion // Ctor

        #region Properties

        public ServerBasedTrados2007TranslationMemory SelectedTranslationMemory { get; private set; }

        #endregion // Properties

        #region Methods

        public DialogResult ShowDialog(IWin32Window owner)
        {
            var result = this.form.ShowDialog(owner);

            if (result == DialogResult.OK)
            {
                this.SelectedTranslationMemory = this.model.SelectedTranslationMemory;
            }
            else
            {
                this.SelectedTranslationMemory = null;
            }

            this.Serialize();

            return result;
        }

        private void Deserialize()
        {
            if (File.Exists(ServerBasedDialog.StatePath))
            {
                using (var stream = new FileStream(ServerBasedDialog.StatePath, FileMode.OpenOrCreate))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(TP2007ServerBasedDialogSerializationHelper));
                    
                    TP2007ServerBasedDialogSerializationHelper helper;

                    try
                    {
                        helper = (TP2007ServerBasedDialogSerializationHelper)serializer.Deserialize(stream);
                    }
                    catch (Exception) // bad decision, To do - find proper exception
                    {
                        return;
                    }

                    this.model.Initialize(helper);
                    this.presenter.Update();
                }
            }
        }

        private void Serialize()
        {
            if (!Directory.Exists(Path.GetDirectoryName(ServerBasedDialog.StatePath)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(ServerBasedDialog.StatePath));
            }

            using (var stream = new FileStream(ServerBasedDialog.StatePath, FileMode.Create))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(TP2007ServerBasedDialogSerializationHelper));
                serializer.Serialize(stream, new TP2007ServerBasedDialogSerializationHelper(this.model));
            }
        }

        #endregion // Methods
    }
}
