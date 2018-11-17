using System.IO;
using System.Windows;
using System.Windows.Input;
using Sdl.Community.SdlDataProtectionSuite.SdlTmAnonymizer.Commands;
using Sdl.Community.SdlDataProtectionSuite.SdlTmAnonymizer.Controls;
using Sdl.Community.SdlDataProtectionSuite.SdlTmAnonymizer.Model;
using Sdl.Community.SdlDataProtectionSuite.SdlTmAnonymizer.Services;

namespace Sdl.Community.SdlDataProtectionSuite.SdlTmAnonymizer.ViewModel
{
	public class SettingsViewModel : ViewModelBase
	{
		private ICommand _okCommand;
		private ICommand _browseCommand;
		private ICommand _resetCommand;
		private readonly SettingsService _settingsService;
		private readonly Settings _settings;
		private readonly Window _controlWindow;
		private string _backupFullPath;
		private bool _backup;
		private string _logsFullPath;

		public SettingsViewModel(Window controlWindow, SettingsService settingsService)
		{
			_controlWindow = controlWindow;
			_settingsService = settingsService;
			_settings = _settingsService.GetSettings();

			Backup = _settings.Backup;
			BackupFullPath = _settings.BackupFullPath;
			LogsFullPath = _settings.LogsFullPath;
		}

		public bool Backup
		{
			get
			{
				return _backup;
			}
			set
			{
				if (_backup == value)
				{
					return;
				}

				_backup = value;
				OnPropertyChanged(nameof(Backup));
			}
		}

		public string BackupFullPath
		{
			get
			{
				return _backupFullPath;
			}
			set
			{
				if (_backupFullPath != null && _backupFullPath == value)
				{
					return;
				}

				_backupFullPath = value;
				OnPropertyChanged(nameof(BackupFullPath));
			}
		}

		public string LogsFullPath
		{
			get
			{
				return _logsFullPath;
			}
			set
			{
				if (_logsFullPath != null && _logsFullPath == value)
				{
					return;
				}

				_logsFullPath = value;
				OnPropertyChanged(nameof(LogsFullPath));
			}
		}

		public ICommand OkCommand => _okCommand ?? (_okCommand = new RelayCommand(Ok));

		public ICommand BrowseCommand => _browseCommand ?? (_browseCommand = new RelayCommand(Browse));

		public ICommand ResetCommand => _resetCommand ?? (_resetCommand = new RelayCommand(Reset));

		private void Ok(object parameter)
		{
			if (Backup && (string.IsNullOrEmpty(BackupFullPath) || !Directory.Exists(BackupFullPath)))
			{
				MessageBox.Show(StringResources.Invalid_Backup_TMs_path, StringResources.SDLTM_Anonymizer_Name);
			}
			else if (Backup && (string.IsNullOrEmpty(LogsFullPath) || !Directory.Exists(LogsFullPath)))
			{
				MessageBox.Show(StringResources.Invalid_Log_report_path, StringResources.SDLTM_Anonymizer_Name);
			}
			else
			{
				_settings.Backup = Backup;
				_settings.BackupFullPath = BackupFullPath;
				_settings.LogsFullPath = LogsFullPath;
				_settingsService.SaveSettings(_settings);

				_controlWindow.DialogResult = true;
			}
		}

		private void Browse(object parameter)
		{
			var folderDialog = new FolderSelectDialog
			{
				Title = StringResources.Browse_Select_folder,
				InitialDirectory = (string)parameter == nameof(Backup) ? BackupFullPath : LogsFullPath
			};

			if (folderDialog.ShowDialog())
			{
				if (!string.IsNullOrEmpty(folderDialog.FileName) && Directory.Exists(folderDialog.FileName))
				{
					if ((string)parameter == nameof(Backup))
					{
						BackupFullPath = folderDialog.FileName;
					}
					else
					{
						LogsFullPath = folderDialog.FileName;
					}
				}
			}
		}

		private void Reset(object parameter)
		{
			Backup = true;
			BackupFullPath = _settingsService.PathInfo.BackupFullPath;
			LogsFullPath = _settingsService.PathInfo.LogsFullPath;
		}
	}
}
