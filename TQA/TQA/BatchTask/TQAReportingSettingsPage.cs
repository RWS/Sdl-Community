using NLog;
using Sdl.Core.Settings;
using Sdl.Desktop.IntegrationApi;
using Sdl.Community.TQA.Providers;

namespace Sdl.Community.TQA.BatchTask
{
	public class TQAReportingSettingsPage : DefaultSettingsPage<TQAReportingSettingsControl, TQAReportingSettings>
    {
        private TQAReportingSettingsControl _settingsView;
        private TQAReportingSettings _settings;

		public override object GetControl()
		{
			_settings = ((ISettingsBundle)DataSource).GetSettingsGroup<TQAReportingSettings>();
            _settingsView = base.GetControl() as TQAReportingSettingsControl;
            if (!(_settingsView is null))
            {
				var logger = LogManager.GetCurrentClassLogger();
				var reportProvider = new ReportProvider(logger);
				var categoriesProvider = new CategoriesProvider();
	            var qualitiesProvider = new QualitiesProvider();
	            var projectSettings = (ISettingsBundle)DataSource;

	            _settingsView.Settings = _settings;
	            _settingsView.ReportProvider = reportProvider;
	            _settingsView.CategoriesProvider = categoriesProvider;
	            _settingsView.QualitiesProvider = qualitiesProvider;
				_settingsView.ProjectSettings = projectSettings;
            }

            return _settingsView;
        }

		public override void Save()
        {
			_settings = _settingsView.SaveSettings();
			base.Save();
		}

		public override void ResetToDefaults()
		{
			_settings = _settingsView.ResetToDefaults();
		}
    }
}