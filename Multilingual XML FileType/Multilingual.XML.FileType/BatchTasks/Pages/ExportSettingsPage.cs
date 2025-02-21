using System;
using System.Windows;
using System.Windows.Threading;
using Multilingual.XML.FileType.BatchTasks.Settings;
using Multilingual.XML.FileType.BatchTasks.ViewModels;
using Multilingual.XML.FileType.BatchTasks.Views;
using Multilingual.XML.FileType.FileType.Settings;
using Sdl.Core.Settings;
using Sdl.Desktop.IntegrationApi;

namespace Multilingual.XML.FileType.BatchTasks.Pages
{
	public class ExportSettingsPage : DefaultSettingsPage<ExportSettingsView, MultilingualXmlExportSettings> 
	{
		private ExportSettingsView _settingsView;
		private ExportSettingsViewModel _settingsViewModel;
		private MultilingualXmlExportSettings _settings;

		public override object GetControl()
		{
			if (_settingsView != null)
			{
				return _settingsView;
			}

			Dispatcher.CurrentDispatcher.Invoke(DispatcherPriority.Normal, new Action(delegate
			{
				//var writerSettings = new WriterSettings();
				//writerSettings.PopulateFromSettingsBundle((ISettingsBundle)DataSource, Constants.FileTypeDefinitionId);
				

				_settings = ((ISettingsBundle)DataSource).GetSettingsGroup<MultilingualXmlExportSettings>();
				//_settings.MonoLanguage = writerSettings.LanguageMappingMonoLanguage;

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

			//var writerSettings = new WriterSettings();
			//writerSettings.PopulateFromSettingsBundle((ISettingsBundle)DataSource, Constants.FileTypeDefinitionId);
			//writerSettings.LanguageMappingMonoLanguage = _settings.MonoLanguage;
			//writerSettings.SaveToSettingsBundle((ISettingsBundle)DataSource, Constants.FileTypeDefinitionId);

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
