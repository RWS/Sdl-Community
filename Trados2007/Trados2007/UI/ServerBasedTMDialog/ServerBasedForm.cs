// ---------------------------------
// <copyright file="ServerBasedDialog.cs" company="SDL International">
// Copyright  2011 All Right Reserved
// </copyright>
// <author>Kostiantyn Lukianets</author>
// <email>klukianets@sdl.com</email>
// <date>2011-11-02</date>
// ---------------------------------
namespace Sdl.TranslationStudio.Plugins.Trados2007.UI
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Windows.Forms;

    using Sdl.LanguagePlatform.Core;

    internal partial class ServerBasedForm : Form
    {
        #region Fields

        #endregion // Fields

        #region Ctor

        public ServerBasedForm()
        {
            this.InitializeComponent();

            UpdateButtons();
        }

        #endregion // Ctor

        #region Properties

        public ServerBasedPresenter Presenter { set; private get; }

        public IList<LanguagePair> LanguagePairs
        {
            get
            {
                var list = new List<LanguagePair>(this.languagePairListBox.Items.Count);
                list.AddRange(
                    from object languagePair in this.languagePairListBox.Items
                    select languagePair as LanguagePair);

                return list;
            }

            set
            {
                this.languagePairListBox.Items.Clear();
                foreach (var languagePair in value)
                {
                    this.languagePairListBox.Items.Add(languagePair);
                }
            }
        }

        public IList<Trados2007ServerAccount> Servers
        {
            get
            {
                var list = new List<Trados2007ServerAccount>(this.serversComboBox.Items.Count);
                list.AddRange(
                    from object trados2007ServerAccount in this.serversComboBox.Items
                    select trados2007ServerAccount as Trados2007ServerAccount);

                return list;
            }

            set
            {
                var saveSelectedServer = this.SelectedServer;

                this.serversComboBox.Items.Clear();

                if (value != null)
                {
                    foreach (var account in value)
                    {
                        this.serversComboBox.Items.Add(account);
                    }
                }

                if (saveSelectedServer != null && value.Contains(saveSelectedServer))
                {
                    this.SelectedServer = saveSelectedServer;
                }
            }
        }

        public Trados2007ServerAccount SelectedServer
        {
            get
            {
                return this.serversComboBox.SelectedItem as Trados2007ServerAccount;
            }

            set
            {
                this.serversComboBox.SelectedItem = value;
            }
        }

        public IList<ServerBasedTrados2007TranslationMemory> TranslationMemories
        {
            set
            {
                bool notEmpty = value != null && value.Count != 0;

                ShowHideColumns(notEmpty);

                translationMemoriesGridView.Rows.Clear();

                if (notEmpty)
                {
                    foreach (var el in value)
                    {
                        translationMemoriesGridView.Rows.Add(new String[] { el.Name, el.Container });
                    }
                }
            }
        }

        private void ShowHideColumns(bool visible)
        {
            foreach (DataGridViewTextBoxColumn col in translationMemoriesGridView.Columns)
            {
                col.Visible = visible;
            }
        }

        public int SelectedTranslationMemory 
        { 
            get
            {
                var selectedRows = translationMemoriesGridView.SelectedRows;

                return selectedRows.Count == 0 ? -1 : selectedRows[0].Index;
            }
        }

        public bool IsLanguageFilteringEnabled
        {
            get
            {
                return this.languageFilterCheckBox.Checked;
            }

            set
            {
                this.languageFilterCheckBox.Checked = value;
            }
        }

        #endregion // Properties

        #region Event Handlers

        private void OnBrowseButtonClick(object sender, EventArgs e)
        {
            if (this.Presenter != null)
            {
                this.Presenter.ShowServersDialog();
            }
        }

        private void LanguageFilterCheckBoxCheckedChanged(object sender, EventArgs e)
        {
            this.languagePairListBox.Enabled = this.languageFilterCheckBox.Checked;
        }

        private void OnOkButtonClick(object sender, EventArgs e)
        {
            OnOkButton();
        }

        private void OnOkButton()
        {
            if (this.Presenter != null)
            {
                this.Presenter.OnSelectTranslationMemory();
            }

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void OnCancelButtonClick(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void OnServersComboBoxSelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.Presenter != null)
            {
                this.Presenter.OnSelectServer();
            }
        }

        #endregion // Event handlers

        private void translationMemoriesGridView_SelectionChanged(object sender, EventArgs e)
        {
            UpdateButtons();
        }

        private void UpdateButtons()
        {
            this.okButton.Enabled = CanOk();
        }

        private bool CanOk()
        {
            return SelectedTranslationMemory != -1;
        }

        private void OnCellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.RowIndex != -1)
            {
                OnOkButton();
            }
        }

        internal void StartInfiniteProgress()
        {
            loadingPictureBox.Visible = true;
        }

        internal void StopInfiniteProgress()
        {
            loadingPictureBox.Visible = false;
        }
    }
}
