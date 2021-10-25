using Sdl.Core.Settings;
using Sdl.Desktop.IntegrationApi;

namespace Sdl.Community.TargetWordCount
{
	public class WordCountBatchTaskSettingsPage : DefaultSettingsPage<WordCountBatchTaskSettingsControl, WordCountBatchTaskSettings>
	{
		private WordCountBatchTaskSettingsControl _control;
		private WordCountBatchTaskSettings _settings;

		public override void Save()
		{
			base.Save();
			if (_settings is null) return;
			_settings.CharactersPerLine = _control.charPerLineTextBox.Text;
			_control.SetInvoiceRates();

		}
		public override object GetControl()
		{
			_control = base.GetControl() as WordCountBatchTaskSettingsControl;
			_settings = ((ISettingsBundle)DataSource).GetSettingsGroup<WordCountBatchTaskSettings>();

			if (_control != null)
			{
				_control.Settings = _settings;
			}

			return _control;
		}	
	}
}