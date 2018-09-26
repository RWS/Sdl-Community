using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.Integration;
using System.Windows.Input;
using System.Windows.Threading;
using Sdl.Community.SdlTmAnonymizer.Commands;
using Sdl.Community.SdlTmAnonymizer.Helpers;
using Sdl.Community.SdlTmAnonymizer.Model;
using Sdl.Community.SdlTmAnonymizer.Services;
using Sdl.Community.SdlTmAnonymizer.Ui;
using DataFormats = System.Windows.Forms.DataFormats;
using MessageBox = System.Windows.Forms.MessageBox;

namespace Sdl.Community.SdlTmAnonymizer.ViewModel
{
	public class TranslationMemoryViewModel : ViewModelBase, IDisposable
	{
		private bool _selectAll;
		private ICommand _selectFoldersCommand;
		private ICommand _removeCommand;
		private ICommand _selectTmCommand;
		private ICommand _selectAllCommand;
		private ICommand _dragEnterCommand;
		private ICommand _loadServerTmCommand;
		private IList _selectedItems;
		private ObservableCollection<TmFile> _tmsCollection;
		private Login _credentials;
		private bool _isEnabled;
		private LoginWindowViewModel _loginWindowViewModel;		

		public TranslationMemoryViewModel(SettingsService settingsService)
		{
			SettingsService = settingsService;
			TmService = new TmService(settingsService);

			_isEnabled = true;
			_tmsCollection = new ObservableCollection<TmFile>(SettingsService.GetTmFiles());
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

				_tmsCollection = value;
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

		public Login Credentials
		{
			get => _credentials;

			set
			{
				if (Equals(value, _credentials))
				{
					return;
				}
				_credentials = value;
				OnPropertyChanged(nameof(Credentials));
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

		public ICommand SelectTmCommand => _selectTmCommand ?? (_selectTmCommand = new CommandHandler(SelectTm, true));

		public ICommand SelectAllCommand => _selectAllCommand ?? (_selectAllCommand = new CommandHandler(SelectAllTms, true));

		public ICommand DragEnterCommand => _dragEnterCommand ?? (_dragEnterCommand = new RelayCommand(HandlePreviewDrop));

		public ICommand LoadServerTmCommand => _loadServerTmCommand ?? (_loadServerTmCommand = new CommandHandler(ShowLogInWindow, true));

		private void ShowLogInWindow()
		{
			var loginWindow = new LoginWindow();

			_loginWindowViewModel = new LoginWindowViewModel(loginWindow, TmsCollection);
			_loginWindowViewModel.PropertyChanged += ViewModel_PropertyChanged;
			loginWindow.DataContext = _loginWindowViewModel;

			//if we don't set element host we are not able to type in text box
			ElementHost.EnableModelessKeyboardInterop(loginWindow);
			loginWindow.Show();
			Dispatcher.Run();
		}

		private void ViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			var viewModel = (LoginWindowViewModel)sender;

			if (!string.IsNullOrEmpty(viewModel?.Credentials.Password) &&
			   !string.IsNullOrEmpty(viewModel.Credentials.Url) &&
			   !string.IsNullOrEmpty(viewModel.Credentials.UserName))
			{
				Credentials = viewModel.Credentials;
			}
		}

		/// <summary>
		/// Handle drop file event
		/// </summary>
		/// <param name="dropedFile"></param>
		private void HandlePreviewDrop(object dropedFile)
		{
			var file = dropedFile as System.Windows.DataObject;
			var tmsPath = (string[])file?.GetData(DataFormats.FileDrop);
			if (tmsPath != null)
			{
				foreach (var tm in tmsPath)
				{
					if (!string.IsNullOrEmpty(tm) && Path.GetExtension(tm).Equals(".sdltm")) AddTm(tm);
				}
			}
		}		

		private void SaveSetttings()
		{
			var settings = SettingsService.GetSettings();
			settings.TmFiles = TmsCollection.ToList();
			SettingsService.SaveSettings(settings);			
		}

		private void SelectTm()
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

		private void SelectAllTms()
		{
			foreach (var tm in TmsCollection)
			{
				tm.IsSelected = SelectAll;
			}
		}

		private void AddTm(string tmPath)
		{
			if (!TmAlreadyExist(tmPath))
			{
				var tmFileInfo = new FileInfo(tmPath);
				var tm = new TmFile
				{
					Name = tmFileInfo.Name,
					Path = tmFileInfo.FullName,
					IsSelected = true,
					IsServerTm = false
				};

				AddTm(tm);
			}
		}

		private void AddTm(TmFile tm)
		{
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

		/// <summary>
		/// Raise property change event for TM Collection
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Tm_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			OnPropertyChanged(nameof(TmsCollection));
		}

		public void Dispose()
		{
			if (_loginWindowViewModel != null)
			{
				_loginWindowViewModel.PropertyChanged -= ViewModel_PropertyChanged;
			}

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
