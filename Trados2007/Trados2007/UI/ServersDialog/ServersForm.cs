using Sdl.TranslationStudio.Plugins.Trados2007.UI.ServersDialog;
// ---------------------------------
// <copyright file="ServersDialog.cs" company="SDL International">
// Copyright  2011 All Right Reserved
// </copyright>
// <author>Kostiantyn Lukianets</author>
// <email>klukianets@sdl.com</email>
// <date>2011-11-01</date>
// ---------------------------------
namespace Sdl.TranslationStudio.Plugins.Trados2007.UI
{
    using System.Windows.Forms;
    using System;
    using Sdl.LanguagePlatform.TranslationMemoryApi;

    /// <summary>
    /// Server Dialog view
    /// </summary>
    public partial class ServersForm : Form
    {
        private ServersFormPresenter presenter;
        private ServersFormModel model;

        public ServersForm(System.Collections.Generic.IList<Trados2007ServerAccount> servers, 
            ITranslationProviderCredentialStore credentialStore)
        {
            // TODO: Complete member initialization
            this.InitializeComponent();

            this.model = new ServersFormModel(servers);
            this.presenter = new ServersFormPresenter(this, model, credentialStore);
            UpdateButtons();
        }

        private void OnAddButtonClick(object sender, System.EventArgs e)
        {
            this.presenter.OnAdd();
        }

        private void OnEditButtonClick(object sender, System.EventArgs e)
        {
            this.presenter.OnEdit();
        }

        private void OnDeleteButtonClick(object sender, System.EventArgs e)
        {
            this.presenter.OnDelete();
        }

        private void OnCheckServerButtonClick(object sender, System.EventArgs e)
        {
            this.presenter.OnCheckServer();
        }

        public System.Collections.Generic.IList<Trados2007ServerAccount> Servers 
        { 
            get
            {
                return this.model.Servers;
            } 
            set
            {
                bool notEmpty = value != null && value.Count != 0;

                ShowHideColumns(notEmpty);

                serversGridView.Rows.Clear();

                if (notEmpty)
                {
                    foreach (var el in value)
                    {
                        serversGridView.Rows.Add(new String[] { el.ToString(), model.GetServerStatus(el) });
                    }
                }
            }
        }

        public int SelectedServer
        {
            get
            {
                var selectedRows = serversGridView.SelectedRows;

                return selectedRows.Count == 0 ? -1 : selectedRows[0].Index;
            }
        }

        public void UpdateServerStatus(string status, int server)
        {
            serversGridView.Rows[server].Cells[1].Value = status;
        }

        private void ShowHideColumns(bool visible)
        {
            foreach (DataGridViewTextBoxColumn col in serversGridView.Columns)
            {
                col.Visible = visible;
            }
        }

        private void serversGridView_SelectionChanged(object sender, EventArgs e)
        {
            UpdateButtons();
        }

        private void UpdateButtons()
        {
            this.toolStripDeleteButton.Enabled =
                this.toolStripEditButton.Enabled =
                this.toolStripCheckServerButton.Enabled = IsSelectedServer();
        }

        private bool IsSelectedServer()
        {
            return SelectedServer != -1;
        }
    }
}
