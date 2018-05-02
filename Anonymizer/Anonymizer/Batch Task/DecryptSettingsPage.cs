using Sdl.Community.projectAnonymizer.Ui;
using Sdl.Core.Settings;
using Sdl.Desktop.IntegrationApi;

namespace Sdl.Community.projectAnonymizer.Batch_Task
{
	public class DecryptSettingsPage: DefaultSettingsPage<DecryptSettingsControl,DecryptSettings>
	{
		private DecryptSettings _settings;
		private DecryptSettingsControl _control;
		public override object GetControl()
		{
			_settings = ((ISettingsBundle)DataSource).GetSettingsGroup<DecryptSettings>();
			_control = base.GetControl() as DecryptSettingsControl;
			return _control;
		}

		public override void Save()
		{
			_settings.EncryptionKey = _control.EncryptionKey;
			_settings.IgnoreEncrypted = _control.IgnoreEncrypted;
		}
	}

	
}
