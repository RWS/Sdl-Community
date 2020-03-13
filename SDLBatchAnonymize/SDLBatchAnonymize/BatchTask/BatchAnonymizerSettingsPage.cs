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

		public override void Save()
		{
			base.Save();
			_settings.AnonymizeComplete = _control.AnonymizeComplete;
			_settings.AnonymizeTmMatch = _control.AnonymizeTmMatch;
			_settings.FuzzyScore = _control.Score;
			_settings.TmName = _control.TmName;
		}
	}
}
