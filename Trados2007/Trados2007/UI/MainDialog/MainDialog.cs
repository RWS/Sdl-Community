// ---------------------------------
// <copyright file="MainDialog.cs" company="SDL International">
// Copyright 2011 All Right Reserved
// </copyright>
// <author>Kostiantyn Lukianets</author>
// <email>klukianets@sdl.com</email>
// <date>2011-11-01</date>
// ---------------------------------
namespace Sdl.Community.Trados2007.UI
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Windows.Forms;

    using Sdl.LanguagePlatform.Core;
    using Sdl.LanguagePlatform.TranslationMemoryApi;

    /// <summary>
    /// Wrapper around file or server based selection logic & UI
    /// </summary>
    public class MainDialog
    {
        #region Fields

        /// <summary>
        /// The MVP model
        /// </summary>
        private MainModel model;

        /// <summary>
        /// The MVP view
        /// </summary>
        private MainForm form;

        /// <summary>
        /// The MVP presenter
        /// </summary>
        private MainPresenter presenter;

        /// <summary>
        /// supported language pairs
        /// </summary>
        private IList<LanguagePair> languagePairs;

        /// <summary>
        /// encrypted credential store
        /// </summary>
        private ITranslationProviderCredentialStore credentialStore;

        #endregion // Fields

        /// <summary>
        /// Initializes a new instance of the <see cref="MainDialog"/> class.
        /// </summary>
        /// <param name="languagePairs">The language pairs.</param>
        /// <param name="credentialStore">The credential store.</param>
        public MainDialog(LanguagePair[] languagePairs, ITranslationProviderCredentialStore credentialStore)
        {
            this.model = new MainModel(languagePairs, credentialStore);
            this.form = new MainForm();
            this.presenter = new MainPresenter(this.form, this.model);
            this.form.Presenter = this.presenter;

            this.languagePairs = languagePairs;
            this.credentialStore = credentialStore;
        }

        /// <summary>
        /// Gets the selected translation provider.
        /// </summary>
        public ITrados2007TranslationProvider SelectedTranslationProvider { get; private set; }

        /// <summary>
        /// Shows the dialog.
        /// </summary>
        /// <param name="owner">The owner.</param>
        /// <returns><see cref="DialogResult.OK"/> if selection was successful; otherwise - <see cref="DialogResult.Cancel"/>.</returns>
        public DialogResult ShowDialog(IWin32Window owner)
        {
            var result = this.form.ShowDialog(owner);

            // if ok - get data from model
            if (result == DialogResult.OK)
            {
                this.SelectedTranslationProvider = this.model.SelectedTrados2007TranslationProvider;
            }

            return result;
        }
    }
}
