using Sdl.Community.SDLBatchAnonymize.Control;
using Sdl.Core.Settings;
using Sdl.Desktop.IntegrationApi;

namespace Sdl.Community.SDLBatchAnonymize.BatchTask
{
	public class BatchAnonymizerSettingsPage : DefaultSettingsPage<BatchAnonymizerHostControl, BatchAnonymizerSettings>
	{
		private BatchAnonymizerHostControl _control;
		private BatchAnonymizerSettings _settings;

		public override object GetControl()
		{
			_settings = ((ISettingsBundle)DataSource).GetSettingsGroup<BatchAnonymizerSettings>();
			_control = base.GetControl() as BatchAnonymizerHostControl;
			if (_control != null)
			{
				_control.BatchAnonymizerSettingsViewModel.Settings = _settings;
			}
			return _control;
		}

		public override void Save()
		{
			base.Save();
			if (_settings is null) return;
			_settings.AnonymizeComplete = _control.BatchAnonymizerSettingsViewModel.AnonymizeAllSettings;
			_settings.UseGeneral = _control.BatchAnonymizerSettingsViewModel.UseGeneral;
			_settings.ClearSettings = _control.BatchAnonymizerSettingsViewModel.ClearSettings;
			_settings.CreatedByChecked = _control.BatchAnonymizerSettingsViewModel.CreatedByChecked;
			_settings.CreatedByName = _control.BatchAnonymizerSettingsViewModel.CreatedByName;
			_settings.ModifyByChecked = _control.BatchAnonymizerSettingsViewModel.ModifyByChecked;
			_settings.ModifyByName = _control.BatchAnonymizerSettingsViewModel.ModifyByName;
			_settings.CommentChecked = _control.BatchAnonymizerSettingsViewModel.CommentChecked;
			_settings.CommentAuthorName = _control.BatchAnonymizerSettingsViewModel.CommentAuthorName;
			_settings.TrackedChecked = _control.BatchAnonymizerSettingsViewModel.TrackedChecked;
			_settings.TrackedName = _control.BatchAnonymizerSettingsViewModel.TrackedName;
			_settings.ChangeMtChecked = _control.BatchAnonymizerSettingsViewModel.ChangeMtChecked;
			_settings.ChangeTmChecked = _control.BatchAnonymizerSettingsViewModel.ChangeTmChecked;
			_settings.SetSpecificResChecked = _control.BatchAnonymizerSettingsViewModel.SetSpecificResChecked;
			_settings.FuzzyScore = _control.BatchAnonymizerSettingsViewModel.FuzzyScore;
			_settings.TmName = _control.BatchAnonymizerSettingsViewModel.TmName;
			_settings.RemoveMtCloudMetadata = _control.BatchAnonymizerSettingsViewModel.RemoveMtCloudMetadata;
		}
	}
}