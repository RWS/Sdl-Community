using System;
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
			//SettingsBinder.DataBindSetting<string>(encryptionBox, "Text", Settings, nameof(Settings.EncryptionKey));
			SettingsBinder.DataBindSetting<bool>(ignoreEncrypted, "Checked", Settings, nameof(Settings.IgnoreEncrypted));
		}

		public DecryptSettings Settings { get ; set ; }
	}
}
