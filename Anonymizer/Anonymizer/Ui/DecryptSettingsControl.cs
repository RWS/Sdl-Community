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
			Timer.Tick += EncryptionBox_UserStoppedTyping;
			decryptionBox.TextChanged += (t, e) => StartSearchTimer();
		}

		public AnonymizerSettings Settings { get; set; }

		public string EncryptionKey
		{
			get => decryptionBox.Text;
			set => decryptionBox.Text = value;
		}

		private Timer Timer { get; } = new Timer() { Interval = 500 };

		protected override void OnLoad(EventArgs e)
		{
			SuspendLayout();
			lockPictureBox.SizeMode = PictureBoxSizeMode.AutoSize;
			ResumeLayout();
			if (Settings.EncryptionState == State.DefaultState)
			{
				Settings.EncryptionState = IsProjectAnonymized() ? State.DataEncrypted : State.Decrypted;
			}

			if (Settings.EncryptionState.HasFlag(State.Decrypted))
			{
				encryptedMessage.Text = "Click finish to untag the text";
				decryptionBox.Enabled = false;
			}

			SetSettings(Settings);
		}

		private void StartSearchTimer()
		{
			Timer.Stop();
			Timer.Start();
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
			if (!anonymizerKey.Equals(AnonymizeData.EncryptData(decryptionBox.Text, Constants.Key)))
			{
				messageLbl.Visible = true;
				messageLbl.Text = "Decryption key doesn't match the encryption key.";
				messageLbl.ForeColor = Color.Crimson;
			}
		}

		private void EncryptionBox_UserStoppedTyping(object sender, EventArgs e)
		{
			messageLbl.Visible = false;
			CheckIfKeysMatch();
		}
	}
}