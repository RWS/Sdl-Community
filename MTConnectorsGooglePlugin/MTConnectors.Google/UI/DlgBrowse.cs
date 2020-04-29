using Sdl.LanguagePlatform.MTConnectors.Google.Utils;
using Sdl.LanguagePlatform.MTConnectors.Google.DataContracts;
using Sdl.LanguagePlatform.MTConnectors.Google.TranslationProvider;
using Sdl.LanguagePlatform.TranslationMemoryApi;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Sdl.LanguagePlatform.MTConnectors.Google.UI
{
    internal partial class DlgBrowse : Form
    {
        public enum DialogMode
        {
            Credentials = 0,
            Create = 1,
            Edit = 2
        }

        #region Variables
        private ITranslationProviderCredentialStore _credentialStore;
        private Settings _settingsToEdit;
        private GoogleCredentials _googleCredentials;
        private DialogMode _mode;
        private GoogleCredentialsDataStore _dataStore;
        #endregion

        #region Constructor
        public DlgBrowse(Settings settingsToEdit, ITranslationProviderCredentialStore credentialStore, DialogMode mode)
        {
            _mode = mode;
            _credentialStore = credentialStore;

            _dataStore = new GoogleCredentialsDataStore();
            _googleCredentials = _dataStore.Load();
            _settingsToEdit = settingsToEdit;

            InitializeComponent();

            SetIcons();

            SetTitle();
            _comboBoxGoogleUser.Text = string.Empty;

            txtApiKey.Text = string.Empty;
            btnOK.Enabled = false;

            UpdateAPIKeyFromCredentials(_settingsToEdit);
            UpdateTranslationModel(_settingsToEdit);

            btnRemove.Visible = false;
        }

        private void SetIcons()
        {
            wizardTitleControl1.TitleText = PluginResources.GoogleDialog_Title;
            wizardTitleControl1.SubTitleText = PluginResources.GoogleDialog_Description;
            const int iconSize = 48;
            var imageSize = DpiAwareUtil.AdjustWidthForCurrentDpi(iconSize);
            wizardTitleControl1.TitleBitmap = IconUtil.IconToBitmap(PluginResources.Google_Cloud_Platform, imageSize,
                imageSize);
        }

    
        #endregion

        #region Properties
        public string ConfigurationName
        {
            get
            {
                return _comboBoxGoogleUser.Text.Trim();
            }
        }
        #endregion

        #region Methods
        public Settings GetSettings()
        {
            Settings settings = new Settings(txtApiKey.Text.Trim(), _comboBoxGoogleUser.Text.Trim());
            settings.TranslationModel = rbNeuralMT.Checked
                ? MachineTranslationModel.Neural
                : MachineTranslationModel.Base;
            return settings;
        }

        private void DisplayBrowser(string webAddress)
        {
            ProcessStartInfo processStartInfo = new ProcessStartInfo(webAddress);
            processStartInfo.UseShellExecute = true;
            Process.Start(processStartInfo);
        }

        private void SetTitle()
        {
            if (_mode == DialogMode.Create)
            {
                this.Text = PluginResources.GoogleDialog_TitleBarAdd;
            }
            else
            {
                this.Text = PluginResources.GoogleDialog_TitleBarEdit;
            }
        }

        private void UpdateOk()
        {
            btnOK.Enabled = string.IsNullOrWhiteSpace(txtApiKey.Text) == false;
        }

        private bool CanConnectToGoogle(Settings tmpSettings)
        {
            if (tmpSettings == null)
                return false;

            // show the wait cursor and get the list of languages
            this.Cursor = Cursors.WaitCursor;

            GoogleTranslationProvider tmpProvider = new GoogleTranslationProvider(tmpSettings);
            this.Cursor = Cursors.Default;
            return tmpProvider.IsAPIKeyValid();

        }

        /// <summary>
        /// Gets the API key from the credentials store and adds it to the settings.
        /// As Google settings generate a valid credentialsUri for the case when no User Key is set (to accomodate V1 google providers)
        /// then the API Key will always be overwritten with the key in the credentials store if one is present
        /// Also copies the API Key to the Text bo on the UI
        /// </summary>
        /// <param name="settings">Initial settings</param>
        /// <returns>Settings with apikey filled in</returns>
        private void UpdateAPIKeyFromCredentials(Settings settings)
        {
            if (settings != null)
            {
                TranslationProviderCredential credential = _credentialStore.GetCredential(settings.CredentialsUri);
                if (credential != null)
                {
                    settings.ApiKey = this.txtApiKey.Text = credential.Credential;
                }
            }
        }

        private void UpdateTranslationModel(Settings settings)
        {
            if (settings != null)
            {
                rbNeuralMT.Checked = settings.TranslationModel == MachineTranslationModel.Neural;
                rbPhraseBasedMT.Checked = settings.TranslationModel == MachineTranslationModel.Base;
            }
        }
        #endregion

        #region Event Handlers
        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            DisplayBrowser("https://code.google.com/apis/console/?api=translate");
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            Settings singleSettings = GetSettings();
            _settingsToEdit = singleSettings;


            bool canConnect;
            do
            {
                canConnect = CanConnectToGoogle(singleSettings);
            }
            while (!canConnect && ShowRetryCancelInformation(this,
                       PluginResources.Google_ApiKeyCannotBeValidated, PluginResources.Google_InvalidApiKey) == DialogResult.Retry);
            if (canConnect)
            {

                SaveConnectionDetails(singleSettings);
                DialogResult = DialogResult.OK;
                Close();
            }
        }

        private void SaveConnectionDetails(Settings settings)
        {
            if (!String.IsNullOrEmpty(ConfigurationName))
            {
                if (_googleCredentials.DoesDifferentNamedSettingAlreadyExist(settings, ConfigurationName))
                {
                    if (AskYesNoQuestion(this, String.Format(PluginResources.Google_OverwriteConnectionQuestion, ConfigurationName),
                        PluginResources.Google_OverwriteConnectionCaption) != DialogResult.Yes)
                    {
                        return;
                    }
                }

                if (settings != null && !String.IsNullOrEmpty(ConfigurationName))
                {
                    // save to key/named storage if config name is set
                    _googleCredentials.UpdateKeyToUserStorage(ConfigurationName, settings);
                    _dataStore.Save(_googleCredentials);
                }
            }
        }

        private void _comboBoxGoogleUser_SelectedIndexChanged(object sender, EventArgs e)
        {
            ConnectionEntry storedCredential = _googleCredentials.Credentials
                .Where(z => String.Compare(z.Name, (_comboBoxGoogleUser.SelectedItem as ConnectionEntry).Name) == 0)
                .FirstOrDefault();

            if (storedCredential != null)
            {
                this._settingsToEdit.UserKey = this._comboBoxGoogleUser.Text = storedCredential.UserKey;
                UpdateAPIKeyFromCredentials(_settingsToEdit);
            }
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            var selectedItem = this._comboBoxGoogleUser.SelectedItem;
            if (selectedItem != null)
            {
                if (AskYesNoQuestion(this,
                        String.Format(PluginResources.Google_DeleteConnectionQuestion, selectedItem.ToString()),
                        PluginResources.Google_DeleteConnectionCaption) == DialogResult.Yes)
                {
                    ConnectionEntry entry = selectedItem as ConnectionEntry;
                    _googleCredentials.DeleteNamedEntry(entry.UserKey);
                    _dataStore.Save(_googleCredentials);
                    Settings tempSettings = new Settings("", entry.UserKey);
                    _credentialStore.RemoveCredential(tempSettings.CredentialsUri);
                    _comboBoxGoogleUser.Items.Remove(selectedItem);     // delete from the save connections name list
                }
            }
        }

        private void _comboBoxGoogleUser_Click(object sender, EventArgs e)
        {
            if (String.Equals(_comboBoxGoogleUser.Text, PluginResources.GoogleDialog_ConnectionName))
            {
                _comboBoxGoogleUser.Text = "";
                _comboBoxGoogleUser.ForeColor = SystemColors.WindowText;
            }
        }

        private void _comboBoxGoogleUser_TextUpdate(object sender, EventArgs e)
        {
            UpdateOk();
        }

        private void txtApiKey_TextChanged(object sender, EventArgs e)
        {
            UpdateOk();
        }

        private DialogResult ShowRetryCancelInformation(IWin32Window owner, string text, string caption)
        {
            var result = MessageBox.Show(owner, text, caption, MessageBoxButtons.RetryCancel, MessageBoxIcon.Information);
       
            return result;
        }

        public DialogResult AskYesNoQuestion(IWin32Window owner, string question, string caption)
        {
            var result = MessageBox.Show(owner, question, caption, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
          
            return result;
        }

        #endregion
    }
}
