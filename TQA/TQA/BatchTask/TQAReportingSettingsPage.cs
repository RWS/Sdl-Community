using System.Windows;
using NLog;
using Sdl.Core.Settings;
using Sdl.Desktop.IntegrationApi;
using Sdl.Community.TQA.BatchTask.View;
using Sdl.Community.TQA.BatchTask.ViewModel;
using Sdl.Community.TQA.Providers;

namespace Sdl.Community.TQA.BatchTask
{
	public class TQAReportingSettingsPage : DefaultSettingsPage<TQAReportingSettingsView, TQAReportingSettings>
    {
        private TQAReportingSettingsView _settingsView;
        private TQAReportingSettings _settings;
        private TQAReportingSettingsViewModel _settingsViewModel;

		public override object GetControl()
		{
			_settings = ((ISettingsBundle)DataSource).GetSettingsGroup<TQAReportingSettings>();
            _settingsView = base.GetControl() as TQAReportingSettingsView;
            if (!(_settingsView is null))
            {
				var logger = LogManager.GetCurrentClassLogger();
				var reportProvider = new ReportProvider(logger);
				var categoriesProvider = new CategoriesProvider();
	            var qualitiesProvider = new QualitiesProvider();
	            var projectSettings = (ISettingsBundle)DataSource;
	            
				_settingsViewModel = new TQAReportingSettingsViewModel(projectSettings, _settings, reportProvider, categoriesProvider, qualitiesProvider);
	            _settingsView.Loaded += SettingsPageOnLoaded;
            }

            return _settingsView;
        }

        private void SettingsPageOnLoaded(object sender, RoutedEventArgs e)
        {
	        _settingsView.Loaded -= SettingsPageOnLoaded;
			_settingsView.DataContext = _settingsViewModel;
        }

		public override void Save()
        {
			_settings = _settingsViewModel.SaveSettings();
			base.Save();
		}

		public override void ResetToDefaults()
		{
			_settings = _settingsViewModel.ResetToDefaults();
		}

		public override void Dispose()
		{
			if (_settingsView != null)
			{
				_settingsView.Loaded -= SettingsPageOnLoaded;
			}

			base.Dispose();
		}
    }
}