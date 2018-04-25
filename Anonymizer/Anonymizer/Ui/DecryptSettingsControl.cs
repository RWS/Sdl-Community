using System;
using System.Windows.Forms;
using Sdl.Community.projectAnonymizer.Batch_Task;
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

		protected override void OnLoad(EventArgs e)
		{
			SetSettings(Settings);
		}
		private void SetSettings(DecryptSettings settings)
		{
			Settings = settings;
			SettingsBinder.DataBindSetting<string>(encryptionBox, "Text", Settings, nameof(Settings.EncryptionKey));
		}

		public DecryptSettings Settings { get ; set ; }
	}
}
