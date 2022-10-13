using System;
using System.Windows;
using System.Windows.Threading;
using Multilingual.Excel.FileType.BatchTasks.Settings;
using Multilingual.Excel.FileType.BatchTasks.ViewModels;
using Multilingual.Excel.FileType.BatchTasks.Views;
using Sdl.Core.Settings;
using Sdl.Desktop.IntegrationApi;

namespace Multilingual.Excel.FileType.BatchTasks.Pages
{
	public class ExportSettingsPage : DefaultSettingsPage<ExportSettingsView, MultilingualExcelExportSettings> 
	{
		private ExportSettingsView _settingsView;
		private ExportSettingsViewModel _settingsViewModel;
		private MultilingualExcelExportSettings _settings;

		public override object GetControl()
		{
			if (_settingsView != null)
			{
				return _settingsView;
			}

			Dispatcher.CurrentDispatcher.Invoke(DispatcherPriority.Normal, new Action(delegate
			{
				_settings = ((ISettingsBundle)DataSource).GetSettingsGroup<MultilingualExcelExportSettings>();
				_settingsView = base.GetControl() as ExportSettingsView;
				if (_settingsView != null)
				{
					_settingsViewModel = new ExportSettingsViewModel(_settings);
					_settingsView.Loaded += LanguageMappingViewOnLoaded;
				}
			}));


			return _settingsView;
		}

		private void LanguageMappingViewOnLoaded(object sender, RoutedEventArgs e)
		{
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
				_settingsView.Loaded -= LanguageMappingViewOnLoaded;
			}

			base.Dispose();
		}
	}
}
