using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.Integration;
using System.Windows.Input;
using Sdl.Community.SdlTmAnonymizer.Commands;
using Sdl.Community.SdlTmAnonymizer.Controls;
using Sdl.Community.SdlTmAnonymizer.Model;
using Sdl.Community.SdlTmAnonymizer.Services;
using Sdl.Community.SdlTmAnonymizer.Studio;
using Sdl.Community.SdlTmAnonymizer.View;
using Sdl.LanguagePlatform.TranslationMemoryApi;

namespace Sdl.Community.SdlTmAnonymizer.ViewModel
{
	public class TranslationMemoryViewModel : ViewModelBase, IDisposable
	{
		private bool _selectAll;
		private ICommand _dragEnterCommand;
		private ICommand _clearTMCacheCommand;
		private ICommand _removeTMCommand;
		private IList _selectedItems;
		private ObservableCollection<TmFile> _tmsCollection;
		private bool _isEnabled;
		private LoginViewModel _loginWindowViewModel;
		private readonly SDLTMAnonymizerView _controller;
		private readonly ContentParsingService _contentParsingService;
		private readonly SerializerService _serializerService;
		private Form _controlParent;

		public TranslationMemoryViewModel(SettingsService settingsService, ContentParsingService contentParsingService,
			SerializerService serializerService, SDLTMAnonymizerView controller)
		{
			SettingsService = settingsService;

			_contentParsingService = contentParsingService;
			_serializerService = serializerService;
			_controller = controller;

			TmService = new TmService(settingsService, _contentParsingService, _serializerService);

			IsEnabled = true;
			TmsCollection = new ObservableCollection<TmFile>(SettingsService.GetTmFiles());
		}

		public Form ControlParent
		{
			get
			{
				if (_controlParent == null)
				{
					try
					{
						var elementHost = _controller?.ContentControl?.Controls[0] as ElementHost;
						_controlParent = elementHost?.FindForm();
					}
					catch { }
				}

				return _controlParent;
			}
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

		public void Refresh()
		{
			OnPropertyChanged(nameof(TmsCollection));
		}

		public ICommand DragEnterCommand => _dragEnterCommand ?? (_dragEnterCommand = new RelayCommand(TmDragEnter));

		public ICommand ClearTmCacheCommand => _clearTMCacheCommand ?? (_clearTMCacheCommand = new RelayCommand(ClearTmCache));

		public ICommand RemoveTmCommand => _removeTMCommand ?? (_removeTMCommand = new RelayCommand(RemoveTm));

		public void AddServerTm()
		{
			var settings = SettingsService.GetSettings();
			var loginWindow = new LoginView();

			var credentials = new Credentials
			{
				Url = settings.LastUsedServerUri,
				UserName = settings.LastUsedServerUserName
			};

			_loginWindowViewModel = new LoginViewModel(loginWindow, credentials);

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
				var selectServers = new SelectServersView();
				var model = new SelectServersViewModel(selectServers, SettingsService, _loginWindowViewModel.TranslationProviderServer);

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

		public void AddFileBasedTm()
		{
			var fileDialog = new OpenFileDialog
			{
				Multiselect = true
			};

			if (fileDialog.ShowDialog() == DialogResult.OK)
			{
				foreach (var fileName in fileDialog.FileNames)
				{
					if (!string.IsNullOrEmpty(fileName) && Path.GetExtension(fileName).Equals(".sdltm"))
					{
						AddTm(fileName);
					}
				}
			}
		}		

		public void SelectFolder()
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

		private void TmDragEnter(object parameter)
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

		private void RemoveTm(object parameter)
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

		private void ClearTmCache(object parameter)
		{
			foreach (TmFile tmFile in SelectedItems)
			{
				if (!string.IsNullOrEmpty(tmFile.CachePath) && File.Exists(tmFile.CachePath))
				{
					File.Delete(tmFile.CachePath);
					tmFile.CachePath = string.Empty;
					tmFile.IsSelected = false;
				}
			}
		}

		private void SaveSetttings()
		{
			var settings = SettingsService.GetSettings();
			settings.TmFiles = TmsCollection.ToList();
			SettingsService.SaveSettings(settings);
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
							Source = fileBasedTm.LanguageDirection.SourceLanguage.Name,
							Target = fileBasedTm.LanguageDirection.TargetLanguage.Name,
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

			if (!string.IsNullOrEmpty(tm.CachePath) && File.Exists(tm.CachePath))
			{
				File.Delete(tm.CachePath);
			}

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

						var loginWindow = new LoginView();
						_loginWindowViewModel = new LoginViewModel(loginWindow, tmFile.Credentials);

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

				if (!tmFile.IsSelected)
				{
					foreach (var languageDirection in tmFile.TmLanguageDirections)
					{
						languageDirection.TranslationUnits = null;
					}
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
