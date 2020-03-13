using Sdl.Community.SDLBatchAnonymize.Ui;
using Sdl.Core.Settings;
using Sdl.Desktop.IntegrationApi;

namespace Sdl.Community.SDLBatchAnonymize.BatchTask
{
	public class BatchAnonymizerSettingsPage:DefaultSettingsPage<BatchAnonymizerSettingsControl,BatchAnonymizerSettings>
	{
		private BatchAnonymizerSettings _settings;
		private BatchAnonymizerSettingsControl _control;
		public override object GetControl()
		{
			_settings = ((ISettingsBundle)DataSource).GetSettingsGroup<BatchAnonymizerSettings>();
			_control = base.GetControl() as BatchAnonymizerSettingsControl;
			if (_control != null)
			{
				_control.Settings = _settings;
			}
			return _control;
		}
	}
}
