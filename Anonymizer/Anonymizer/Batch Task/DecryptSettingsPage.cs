using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sdl.Community.Anonymizer.Ui;
using Sdl.Core.Settings;
using Sdl.Desktop.IntegrationApi;

namespace Sdl.Community.Anonymizer.Batch_Task
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
		}
	}

	
}
