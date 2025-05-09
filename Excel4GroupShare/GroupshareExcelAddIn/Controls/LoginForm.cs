using GroupshareExcelAddIn.Helper;
using GroupshareExcelAddIn.Models;
using GroupshareExcelAddIn.Properties;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Windows.Forms;
using GroupshareExcelAddIn.Interfaces;
using NLog;
using System.Net;

namespace GroupshareExcelAddIn.Controls
{
    public partial class LoginForm : Form
    {
        private const string _jsonFileName = "GSExcelPluginSettings.json";
        private readonly Logger _logger = Log.GetLogger(nameof(LoginForm));
        private readonly IGroupshareConnection _groupShareConnection;
        private readonly string _settingsFolderPath = $@"C:\Users\{Environment.UserName}\AppData\Roaming\Trados AppStore\GSExcelPlugin";

        public LoginForm(IGroupshareConnection groupShareConnection)
        {
            InitializeComponent();

            _groupShareConnection = groupShareConnection;

            var credentials = GetStoredUserDetails();
            SetCredentialsOnUi(credentials);
        }

        public event EventHandler LoginEnsured;

        public bool LoggedIn => _groupShareConnection.IsConnected;

        public new void Hide()
        {
            if (InvokeRequired)
            {
                Invoke(new Action(base.Hide));
            }
            else
            {
                base.Hide();
            }
        }

        public void Login(bool newLogin, IWin32Window owner)
        {
            if (newLogin || !LoggedIn)
            {
                Show(owner);
            }
            else
            {
                LoginEnsured?.Invoke(owner, EventArgs.Empty);
            }

            if (newLogin)
            {
                Owner = null;
            }
        }

        private void cancelBtnServerBased_Click(object sender, EventArgs e)
        {
            ResetCredentialsOnUi();
            Hide();
        }

        private void DeleteUserDetails()
        {
            SetCredentialsOnUi(null);
            var docPath = Path.Combine(_settingsFolderPath, _jsonFileName);

            try
            {
                if (File.Exists(docPath))
                {
                    File.Delete(docPath);
                }
            }
            catch (Exception)
            {
                MessageBox.Show(Resources.File_used_by_another_process);
            }
        }

        private Credentials GetStoredUserDetails()
        {
            var docPath = Path.Combine(_settingsFolderPath, _jsonFileName);
            if (File.Exists(docPath))
            {
                using (var r = new StreamReader(docPath))
                {
                    var json = r.ReadToEnd();
                    var credentials = JsonConvert.DeserializeObject<Credentials>(json);
                    return credentials;
                }
            }
            return null;
        }

        private void LoginForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            ResetCredentialsOnUi();
            Hide();
        }

        private async void OkButton_Click(object sender, EventArgs e)
        {
            try
            {
                var credentials = new Credentials
                {
                    UserName = _userNameTxtBox.Text,
                    Password = _passwordTxtBox.Text,
                    ServerURI = _serverNameTxtBox.Text,
                    Remember = _rememberCheckBox.Checked
                };
                await _groupShareConnection.ConnectToGroupShare(credentials);

                PersistCredentials(credentials);
                Hide();

                LoginEnsured?.Invoke(Owner, EventArgs.Empty);
                _inputInfoLabel.Visible = false;
            }
            catch (Exception ex)
            {
                _inputInfoLabel.Visible = true;
                switch (ex)
                {
                    case HttpRequestException _:
                        _inputInfoLabel.Text = Resources.Please_introduce_your_correct_credentials;
                        break;

                    case ArgumentException _:
                        _inputInfoLabel.Text = ex.Message;
                        break;
                }

                _logger.Error($"Login {ex.Message}\n {ex.StackTrace}");
                _groupShareConnection.Disconnect();
            }
        }

        private void passwordTxtBox_TextChanged(object sender, EventArgs e)
        {
            UpdateOkButton();
        }

        private void PersistCredentials(Credentials credentials)
        {
            if (_rememberCheckBox.Checked)
            {
                SetUserDetails(credentials);
            }
            else
            {
                DeleteUserDetails();
            }
        }

        private void ResetCredentialsOnUi()
        {
            var credentials = GetStoredUserDetails();
            SetCredentialsOnUi(credentials);
            _inputInfoLabel.Visible = false;
        }

        private void serverNameTxtBox_TextChanged(object sender, EventArgs e)
        {
            UpdateOkButton();
        }

        private void SetCredentialsOnUi(Credentials credentials)
        {
            _serverNameTxtBox.Text = credentials?.ServerURI ?? string.Empty;
            _userNameTxtBox.Text = credentials?.UserName ?? string.Empty;
            _rememberCheckBox.Checked = credentials?.Remember ?? false;
        }

        private void SetUserDetails(Credentials credentials)
        {
            if (!Directory.Exists(_settingsFolderPath))
            {
                Directory.CreateDirectory(_settingsFolderPath);
            }
            var docPath = Path.Combine(_settingsFolderPath, _jsonFileName);
            var jsonResult = JsonConvert.SerializeObject(credentials);

            using (var stream = File.Create(docPath))
            {
                var jsonResultBytes = Encoding.ASCII.GetBytes(jsonResult);
                stream.Write(jsonResultBytes, 0, jsonResultBytes.Length);
            }
        }

        private void UpdateOkButton()
        {
            if (!string.IsNullOrEmpty(_serverNameTxtBox.Text) &&
                !string.IsNullOrEmpty(_userNameTxtBox.Text) &&
                !string.IsNullOrEmpty(_passwordTxtBox.Text))
            {
                _okButton.Enabled = true;
            }
            else
            {
                _okButton.Enabled = false;
            }
        }

        private void userNameTxtBox_TextChanged(object sender, EventArgs e)
        {
            UpdateOkButton();
        }
    }
}