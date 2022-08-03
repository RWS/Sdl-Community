using Sdl.Core.Settings;
using Sdl.Desktop.IntegrationApi;
using Sdl.Community.TQA.BatchTask.View;
using Sdl.Community.TQA.BatchTask.ViewModel;

namespace Sdl.Community.TQA.BatchTask
{
	public class TQAReportingSettingsPage : DefaultSettingsPage<TQAReportingSettingsView, TQAReportingSettings>
    {
        private TQAReportingSettingsView _control;
        private TQAReportingSettings _settings;

        public override object GetControl()
        {
            _settings = ((ISettingsBundle)DataSource).GetSettingsGroup<TQAReportingSettings>();
            _control = base.GetControl() as TQAReportingSettingsView;
            if (!(_control is null))
            {
                _control.ReportingSettingsViewModel.Settings = _settings;
            }

            return _control;
        }

        public override void Save()
        {
            base.Save();
            if (_settings is null) return;
           
            _settings.TQAReportOutputLocation = _control.ReportingSettingsViewModel.ReportOutputLocation;
            _settings.TQAReportingQuality = _control.ReportingSettingsViewModel.ReportQuality;
            _settings.TQAReportingQualities = _control.ReportingSettingsViewModel.ReportQualities;

		}
    }
}