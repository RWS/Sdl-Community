using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Input;
using Sdl.Community.SdlTmAnonymizer.Commands;
using Sdl.Community.SdlTmAnonymizer.Helpers;
using Sdl.Community.SdlTmAnonymizer.Model;
using Sdl.Community.SdlTmAnonymizer.Services;
using Sdl.Community.SdlTmAnonymizer.View;
using Sdl.LanguagePlatform.TranslationMemoryApi;

namespace Sdl.Community.SdlTmAnonymizer.ViewModel
{
	public class TranslationMemoryViewModel : ViewModelBase, IDisposable
	{
		private bool _selectAll;
		private ICommand _selectFoldersCommand;
		private ICommand _removeCommand;
		private ICommand _selectTmCommand;
		private ICommand _dragEnterCommand;
		private ICommand _loadServerTmCommand;
		private IList _selectedItems;
		private ObservableCollection<TmFile> _tmsCollection;
		private bool _isEnabled;
		private LoginWindowViewModel _loginWindowViewModel;

		public TranslationMemoryViewModel(SettingsService settingsService)
		{
			SettingsService = settingsService;
			TmService = new TmService(settingsService);

			IsEnabled = true;
			TmsCollection = new ObservableCollection<TmFile>(SettingsService.GetTmFiles());
		}

		public TmService TmService { get; set; }

		public SettingsService SettingsService { get; set; }

		public ObservableCollection<TmFile> TmsCollection
		{
			get => _tmsCollection;
			set
			{
				if (Equals(value, _tmsCollection))
				{
					return;
				}

				if (value != null)
				{
					foreach (var tm in value)
					{
						tm.PropertyChanged -= Tm_PropertyChanged;
					}
				}

				_tmsCollection = value;

				if (_tmsCollection != null)
				{
					foreach (var tm in _tmsCollection)
					{
						tm.PropertyChanged += Tm_PropertyChanged;
					}
				}

				OnPropertyChanged(nameof(TmsCollection));
			}
		}

		public IList SelectedItems
		{
			get => _selectedItems;
			set
			{
				_selectedItems = value;

				OnPropertyChanged(nameof(SelectedItems));
			}
		}

		public bool SelectAll
		{
			get => _selectAll;
			set
			{
				if (Equals(value, _selectAll))
				{
					return;
				}
				_selectAll = value;
				OnPropertyChanged(nameof(SelectAll));
			}
		}

		public bool IsEnabled
		{
			get => _isEnabled;
			set
			{
				if (Equals(value, _isEnabled))
				{
					return;
				}
				_isEnabled = value;
				OnPropertyChanged(nameof(IsEnabled));
			}
		}

		public ICommand SelectFoldersCommand => _selectFoldersCommand ?? (_selectFoldersCommand = new CommandHandler(SelectFolder, true));

		public ICommand RemoveCommand => _removeCommand ?? (_removeCommand = new CommandHandler(RemoveTm, true));

		public ICommand SelectTmCommand => _selectTmCommand ?? (_selectTmCommand = new CommandHandler(AddTm, true));

		public ICommand DragEnterCommand => _dragEnterCommand ?? (_dragEnterCommand = new RelayCommand(HandlePreviewDrop));

		public ICommand LoadServerTmCommand => _loadServerTmCommand ?? (_loadServerTmCommand = new CommandHandler(AddServerTranslationMemory, true));

		private void AddServerTranslationMemory()
		{
			var settings = SettingsService.GetSettings();
			var loginWindow = new LoginWindow();

			var credentials = new Credentials
			{
				Url = settings.LastUsedServerUri,
				UserName = settings.LastUsedServerUserName
			};

			_loginWindowViewModel = new LoginWindowViewModel(loginWindow, SettingsService, credentials);

			loginWindow.DataContext = _loginWindowViewModel;
			loginWindow.ShowDialog();

			if (loginWindow.DialogResult.HasValue && loginWindow.DialogResult.Value)
			{
				settings.LastUsedServerUri = _loginWindowViewModel.Credentials.Url;
				settings.LastUsedServerUserName = _loginWindowViewModel.Credentials.UserName;
				SettingsService.SaveSettings(settings);
			}

			if (_loginWindowViewModel.TranslationProviderServer != null)
			{
				var selectServers = new SelectServersWindow();
				var model = new SelectServersWindowViewModel(selectServers, SettingsService, _loginWindowViewModel.TranslationProviderServer);

				selectServers.DataContext = model;
				selectServers.ShowDialog();

				if (selectServers.DialogResult.HasValue && selectServers.DialogResult.Value)
				{
					foreach (var tmFile in model.TranslationMemories)
					{
						if (tmFile.IsSelected)
						{
							tmFile.Credentials = _loginWindowViewModel.Credentials;
							AddTm(tmFile);
						}
					}
				}
			}
		}

		private void HandlePreviewDrop(object parameter)
		{
			if (parameter != null && parameter is System.Windows.DragEventArgs eventArgs)
			{
				var fileDrop = eventArgs.Data.GetData(DataFormats.FileDrop, false);
				if (fileDrop is string[] filesOrDirectories && filesOrDirectories.Length > 0)
				{
					foreach (var fullPath in filesOrDirectories)
					{
						if (!string.IsNullOrEmpty(fullPath) && Path.GetExtension(fullPath).Equals(".sdltm"))
						{
							AddTm(fullPath);
						}
					}
				}
			}
		}

		private void SaveSetttings()
		{
			var settings = SettingsService.GetSettings();
			settings.TmFiles = TmsCollection.ToList();
			SettingsService.SaveSettings(settings);
		}

		private void AddTm()
		{
			var fileDialog = new OpenFileDialog();
			if (fileDialog.ShowDialog() == DialogResult.OK)
			{
				var tmFilePath = fileDialog.FileName;
				if (!string.IsNullOrEmpty(tmFilePath) && Path.GetExtension(tmFilePath).Equals(".sdltm"))
				{
					AddTm(tmFilePath);
				}
			}
		}

		private void RemoveTm()
		{
			var result = MessageBox.Show(StringResources.RemoveTm_Do_you_want_to_remove_selected_tms, StringResources.Confirmation, MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
			if (result == DialogResult.OK && SelectedItems != null)
			{
				var selectedTms = new List<TmFile>();

				foreach (TmFile selectedItem in SelectedItems)
				{
					var rule = new TmFile
					{
						Path = selectedItem.Path
					};

					selectedTms.Add(rule);
				}

				SelectedItems.Clear();
				foreach (var selectedTm in selectedTms)
				{
					var tm = TmsCollection.FirstOrDefault(r => r.Path.Equals(selectedTm.Path));
					if (tm != null)
					{
						RemoveTm(tm);
					}
				}
			}
		}

		private void SelectFolder()
		{
			var folderDialog = new FolderSelectDialog();
			if (folderDialog.ShowDialog())
			{
				var folderPath = folderDialog.FileName;
				var files = Directory.GetFiles(folderPath, "*.sdltm");
				foreach (var file in files)
				{
					AddTm(file);
				}
			}
		}

		private bool TmAlreadyExist(string tmPath)
		{
			return TmsCollection.Any(t => t.Path.Equals(tmPath));
		}

		private void AddTm(string tmPath)
		{
			if (!TmAlreadyExist(tmPath))
			{
				var tmFileInfo = new FileInfo(tmPath);

				var fileBasedTm = new FileBasedTranslationMemory(tmFileInfo.FullName);
				var unitsCount = fileBasedTm.LanguageDirection.GetTranslationUnitCount();

				var tm = new TmFile
				{
					Name = tmFileInfo.Name,
					Path = tmFileInfo.FullName,
					TranslationUnits = unitsCount,
					TmLanguageDirections = new List<TmLanguageDirection>
					{
						new TmLanguageDirection
						{
							Source = fileBasedTm.LanguageDirection.SourceLanguage,
							Target = fileBasedTm.LanguageDirection.TargetLanguage,
							TranslationUnitsCount = unitsCount
						}
					}
				};


				AddTm(tm);
			}
		}

		private void AddTm(TmFile tm)
		{
			tm.IsSelected = false;
			tm.PropertyChanged += Tm_PropertyChanged;
			TmsCollection.Insert(0, tm);
			SaveSetttings();
		}

		private void RemoveTm(TmFile tm)
		{
			tm.PropertyChanged -= Tm_PropertyChanged;
			TmsCollection.Remove(tm);
			SaveSetttings();
		}

		private void Tm_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			if (e.PropertyName.Equals(nameof(TmFile.IsSelected)) && sender is TmFile tmFile)
			{
				if ((tmFile.IsSelected && tmFile.IsServerTm && tmFile.Credentials == null)
					|| tmFile.Credentials != null && tmFile.Credentials.Password == null && tmFile.Credentials.CanAuthenticate)
				{
					var authenticated = TmsCollection.FirstOrDefault(a => 
										a.Credentials != null && 
										a.Credentials.Url.Equals(tmFile.Credentials.Url) &&
										a.Credentials.UserName.Equals(tmFile.Credentials.UserName) &&
					                    a.Credentials.IsAuthenticated);

					if (authenticated != null)
					{
						tmFile.Credentials = authenticated.Credentials;
					}
					else
					{
						var settings = SettingsService.GetSettings();

						if (tmFile.Credentials == null)
						{
							tmFile.Credentials = new Credentials
							{
								Url = settings.LastUsedServerUri,
								UserName = settings.LastUsedServerUserName
							};
						}

						var loginWindow = new LoginWindow();
						_loginWindowViewModel = new LoginWindowViewModel(loginWindow, SettingsService, tmFile.Credentials);

						loginWindow.DataContext = _loginWindowViewModel;
						loginWindow.ShowDialog();

						if (loginWindow.DialogResult.HasValue && loginWindow.DialogResult.Value)
						{
							tmFile.Credentials = _loginWindowViewModel.Credentials;
							SettingsService.SaveSettings(settings);
						}
						else
						{
							tmFile.Credentials.CanAuthenticate = false;
							tmFile.IsSelected = false;
						}
					}
				}

				if (tmFile.Credentials != null)
				{
					tmFile.Credentials.CanAuthenticate = true;
				}

				SaveSetttings();

				OnPropertyChanged(nameof(TmsCollection));
			}
		}

		public void Dispose()
		{
			if (TmsCollection != null)
			{
				foreach (var tm in TmsCollection)
				{
					tm.PropertyChanged -= Tm_PropertyChanged;
				}
			}
		}
	}
}
