using System;
using System.Drawing;
using System.Windows.Forms;
using Sdl.Community.projectAnonymizer.Batch_Task;
using Sdl.Community.projectAnonymizer.Helpers;
using Sdl.Community.projectAnonymizer.Process_Xliff;
using Sdl.Desktop.IntegrationApi;

namespace Sdl.Community.projectAnonymizer.Ui
{
	public partial class DecryptSettingsControl : UserControl, ISettingsAware<DecryptSettings>
	{
		public DecryptSettingsControl()
		{
			InitializeComponent();
			encryptionBox.LostFocus += EncryptionBox_LostFocus;
		}

		private void EncryptionBox_LostFocus(object sender, EventArgs e)
		{
			messageLbl.Visible = false;
			CheckIfKeysMatch();
		}

		public string EncryptionKey
		{
			get => encryptionBox.Text;
			set => encryptionBox.Text = value;
		}

		public bool IgnoreEncrypted
		{
			get => ignoreEncrypted.Checked;
			set => ignoreEncrypted.Checked = value;
		}

		protected override void OnLoad(EventArgs e)
		{
			SetSettings(Settings);
		}
		private void SetSettings(DecryptSettings settings)
		{
			Settings = settings;
			var key = Settings.GetSetting<string>(nameof(Settings.EncryptionKey)).Value;
			if (!string.IsNullOrEmpty(key))
			{
				encryptionBox.Text = AnonymizeData.DecryptData(key, Constants.Key);
			}
			CheckIfKeysMatch();
			SettingsBinder.DataBindSetting<bool>(ignoreEncrypted, "Checked", Settings, nameof(Settings.IgnoreEncrypted));
		}

		private void CheckIfKeysMatch()
		{
			var anonymizerKey = Settings.SettingsBundle.GetSettingsGroup<AnonymizerSettings>("AnonymizerSettings").EncryptionKey;
			if (!anonymizerKey.Equals(AnonymizeData.EncryptData(encryptionBox.Text, Constants.Key)))
			{
				messageLbl.Visible = true;
				messageLbl.Text = @"Decryption key doesn't match with the encryption key.";
				messageLbl.ForeColor = Color.Crimson;
			}
		}
		public DecryptSettings Settings { get ; set ; }
	}
}
