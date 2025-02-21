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
	public class ImportSettingsPage : DefaultSettingsPage<ImportSettingsView, MultilingualExcelImportSettings> 
	{
		private ImportSettingsView _settingsView;
		private ImportSettingsViewModel _settingsViewModel;
		private MultilingualExcelImportSettings _settings;

		public override object GetControl()
		{
			if (_settingsView != null)
			{
				return _settingsView;
			}

			Dispatcher.CurrentDispatcher.Invoke(DispatcherPriority.Normal, new Action(delegate
			{
				_settings = ((ISettingsBundle)DataSource).GetSettingsGroup<MultilingualExcelImportSettings>();
				_settingsView = base.GetControl() as ImportSettingsView;
				if (_settingsView != null)
				{
					_settingsViewModel = new ImportSettingsViewModel(_settings);
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
