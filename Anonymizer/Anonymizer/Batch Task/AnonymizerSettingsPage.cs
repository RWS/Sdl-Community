using Sdl.Community.Anonymizer.Ui;
using Sdl.Core.Settings;
using Sdl.Desktop.IntegrationApi;

namespace Sdl.Community.Anonymizer.Batch_Task
{
	public class AnonymizerSettingsPage : DefaultSettingsPage<AnonymizerSettingsControl, AnonymizerSettings>
	{
		private AnonymizerSettings _settings;
		private AnonymizerSettingsControl _control;

		public override object GetControl()
		{
			_settings = ((ISettingsBundle)DataSource).GetSettingsGroup<AnonymizerSettings>();
			_control = base.GetControl() as AnonymizerSettingsControl;
			return _control;
		}

		public override void Save()
		{
			_settings.EncryptionKey = _control.EncryptionKey;
			_settings.RegexPatterns = _control.RegexPatterns;
		}

		public override void OnActivate()
		{
			_control.EncryptionKey = _settings.EncryptionKey;
			_control.RegexPatterns = _settings.RegexPatterns;
		}

	}
}
