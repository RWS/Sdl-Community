using Sdl.Core.Settings;
using Sdl.Desktop.IntegrationApi;

namespace Sdl.Community.TargetWordCount
{
	public class WordCountBatchTaskSettingsPage : DefaultSettingsPage<WordCountBatchTaskSettingsControl, WordCountBatchTaskSettings>
	{
		private WordCountBatchTaskSettingsControl _control;
		public override void Save()
		{
			base.Save();
			_control.SetInvoiceRates();
		}
		public override object GetControl()
		{
			_control = base.GetControl() as WordCountBatchTaskSettingsControl;
			return _control;
		}
	}
}