using System;
using System.Drawing;
using System.Windows.Forms;
using Sdl.Community.projectAnonymizer.Batch_Task;
using Sdl.Community.projectAnonymizer.Helpers;
using Sdl.Community.projectAnonymizer.Process_Xliff;
using Sdl.Desktop.IntegrationApi;

namespace Sdl.Community.projectAnonymizer.Ui
{
	public partial class DecryptSettingsControl : UserControl, ISettingsAware<AnonymizerSettings>
	{
		public DecryptSettingsControl()
		{
			InitializeComponent();
			encryptionBox.LostFocus += EncryptionBox_LostFocus;
		}

		public AnonymizerSettings Settings { get; set; }

		public string EncryptionKey
		{
			get => encryptionBox.Text;
			set => encryptionBox.Text = value;
		}

		protected override void OnLoad(EventArgs e)
		{
			SetSettings(Settings);

			if (Settings.EncryptionState == State.DefaultState)
			{
				Settings.EncryptionState = IsProjectAnonymized() ? State.DataEncrypted : State.Decrypted;
			}
		}

		private bool IsProjectAnonymized()
		{
			return Settings.EncryptionKey != "<dummy-encryption-key>";
		}

		private void SetSettings(AnonymizerSettings settings)
		{
			Settings = settings;
		}

		private void CheckIfKeysMatch()
		{
			var anonymizerKey = Settings.SettingsBundle.GetSettingsGroup<AnonymizerSettings>("AnonymizerSettings").EncryptionKey;
			if (!anonymizerKey.Equals(AnonymizeData.EncryptData(encryptionBox.Text, Constants.Key)))
			{
				messageLbl.Visible = true;
				messageLbl.Text = "Decryption key doesn't match the encryption key.";
				messageLbl.ForeColor = Color.Crimson;
			}
		}

		private void EncryptionBox_LostFocus(object sender, EventArgs e)
		{
			messageLbl.Visible = false;
			CheckIfKeysMatch();
		}
	}
}