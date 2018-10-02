using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Input;
using Sdl.Community.SdlTmAnonymizer.Commands;
using Sdl.Community.SdlTmAnonymizer.Model;
using Sdl.Community.SdlTmAnonymizer.Services;
using Sdl.LanguagePlatform.TranslationMemoryApi;
using WaitWindow = Sdl.Community.SdlTmAnonymizer.View.WaitWindow;

namespace Sdl.Community.SdlTmAnonymizer.ViewModel
{
	public class SystemFieldsViewModel : ViewModelBase, IDisposable
	{
		private readonly ObservableCollection<TmFile> _tmsCollection;
		private ObservableCollection<User> _uniqueUserNames;
		private readonly TranslationMemoryViewModel _translationMemoryViewModel;
		private readonly BackgroundWorker _backgroundWorker;
		private User _selectedItem;
		private ICommand _selectAllCommand;
		private ICommand _applyChangesCommand;
		private ICommand _importCommand;
		private ICommand _exportCommand;
		private IList _selectedItems;
		private WaitWindow _waitWindow;
		private bool _selectAll;
		private readonly SystemFieldsService _systemFieldsService;
		private readonly UsersService _usersService;

		public SystemFieldsViewModel(TranslationMemoryViewModel translationMemoryViewModel, SystemFieldsService systemFieldsService, UsersService usersService)
		{
			_systemFieldsService = systemFieldsService;
			_usersService = usersService;

			_uniqueUserNames = new ObservableCollection<User>();
			_selectedItems = new List<User>();
			_translationMemoryViewModel = translationMemoryViewModel;

			_backgroundWorker = new BackgroundWorker();
			_backgroundWorker.DoWork += BackgroundWorker_DoWork;
			_backgroundWorker.RunWorkerCompleted += BackgroundWorker_RunWorkerCompleted;

			_tmsCollection = _translationMemoryViewModel.TmsCollection;
			_tmsCollection.CollectionChanged += TmsCollection_CollectionChanged;

			InitializeComponents();
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

		public User SelectedItem
		{
			get => _selectedItem;
			set
			{
				if (Equals(value, _selectedItem))
				{
					return;
				}

				_selectedItem = value;
				OnPropertyChanged(nameof(SelectedItem));
			}
		}

		public ObservableCollection<User> UniqueUserNames
		{
			get => _uniqueUserNames;
			set
			{
				if (Equals(value, _uniqueUserNames))
				{
					return;
				}
				_uniqueUserNames = value;
				OnPropertyChanged(nameof(UniqueUserNames));
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

		public ICommand SelectAllCommand => _selectAllCommand ?? (_selectAllCommand = new CommandHandler(SelectAllUserNames, true));

		public ICommand ApplyChangesCommand => _applyChangesCommand ?? (_applyChangesCommand = new CommandHandler(ApplyChanges, true));

		public ICommand ImportCommand => _importCommand ?? (_importCommand = new CommandHandler(Import, true));

		public ICommand ExportCommand => _exportCommand ?? (_exportCommand = new CommandHandler(Export, true));

		private void InitializeComponents()
		{
			foreach (var tm in _tmsCollection)
			{
				AddTm(tm);
			}

			Refresh();
		}

		private void AddTm(TmFile tm)
		{
			tm.PropertyChanged -= Tm_PropertyChanged;
			tm.PropertyChanged += Tm_PropertyChanged;
			SelectTm(tm);
		}

		private void RemoveTm(TmFile tm)
		{
			tm.PropertyChanged -= Tm_PropertyChanged;
			UnselectTm(tm);
		}

		private void SelectTm(TmFile tm)
		{
			if (tm.IsServerTm)
			{
				var uri = new Uri(_translationMemoryViewModel.Credentials.Url);
				var translationProvider = new TranslationProviderServer(uri, false,
					_translationMemoryViewModel.Credentials.UserName,
					_translationMemoryViewModel.Credentials.Password);

				var names = _systemFieldsService.GetUniqueServerBasedSystemFields(tm, translationProvider);
				AddUniqueUserNames(names);
			}
			else
			{
				var names = _systemFieldsService.GetUniqueFileBasedSystemFields(tm);
				AddUniqueUserNames(names);
			}
		}

		private void AddUniqueUserNames(IEnumerable<User> names)
		{
			foreach (var name in names)
			{
				if (!UniqueUserNames.Contains(name))
				{
					UniqueUserNames.Add(name);
				}
			}
		}

		private void UnselectTm(TmFile tm)
		{
			var userNamesToBeRemoved = UniqueUserNames.Where(t => t.TmFilePath.Equals(tm.Path)).ToList();
			foreach (var userName in userNamesToBeRemoved)
			{
				UniqueUserNames.Remove(userName);
			}
		}

		private void SelectAllUserNames()
		{
			foreach (var userName in UniqueUserNames)
			{
				userName.IsSelected = SelectAll;
			}
		}

		private void ApplyChanges()
		{
			foreach (var tm in _tmsCollection.Where(t => t.IsSelected))
			{
				if (!tm.IsServerTm)
				{
					_systemFieldsService.AnonymizeFileBasedSystemFields(tm, UniqueUserNames.ToList());
				}

				else if (tm.IsServerTm)
				{
					var uri = new Uri(_translationMemoryViewModel.Credentials.Url);
					var translationProvider = new TranslationProviderServer(uri, false,
						_translationMemoryViewModel.Credentials.UserName,
						_translationMemoryViewModel.Credentials.Password);

					_systemFieldsService.AnonymizeServerBasedSystemFields(tm, UniqueUserNames.ToList(), translationProvider);
				}
			}

			Refresh();
		}

		private void Refresh()
		{
			if (_tmsCollection != null)
			{
				UniqueUserNames = new ObservableCollection<User>();
				var serverTms = _tmsCollection.Where(s => s.IsServerTm && s.IsSelected).ToList();
				var fileBasedTms = _tmsCollection.Where(s => !s.IsServerTm && s.IsSelected).ToList();
				if (fileBasedTms.Any())
				{
					foreach (var fileTm in fileBasedTms)
					{
						var names = _systemFieldsService.GetUniqueFileBasedSystemFields(fileTm);
						AddUniqueUserNames(names);
					}
				}

				if (serverTms.Any())
				{
					var uri = new Uri(_translationMemoryViewModel.Credentials.Url);
					var translationProvider = new TranslationProviderServer(uri, false,
						_translationMemoryViewModel.Credentials.UserName,
						_translationMemoryViewModel.Credentials.Password);

					foreach (var serverTm in serverTms)
					{
						var names = _systemFieldsService.GetUniqueServerBasedSystemFields(serverTm, translationProvider);
						AddUniqueUserNames(names);
					}
				}

				if (UniqueUserNames.Count > 0 && SelectedItem == null)
				{
					SelectedItem = UniqueUserNames[0];
				}
			}
		}

		private void Import()
		{
			var fileDialog = new OpenFileDialog
			{
				Title = StringResources.Import_Please_select_the_files_you_want_to_import,
				Filter = @"Excel |*.xlsx",
				CheckFileExists = true,
				CheckPathExists = true,
				DefaultExt = "xlsx",
				Multiselect = true
			};

			var result = fileDialog.ShowDialog();
			if (result == DialogResult.OK && fileDialog.FileNames.Length > 0)
			{
				var importedUsers = _usersService.GetImportedUsers(fileDialog.FileNames.ToList());
				foreach (var user in importedUsers)
				{
					var existingUser = UniqueUserNames.FirstOrDefault(u => u.UserName.Equals(user.UserName));
					if (existingUser != null)
					{
						var index = UniqueUserNames.IndexOf(existingUser);
						if (index != -1)
						{
							UniqueUserNames[index] = user;
						}
					}
				}
			}
		}

		private void Export()
		{
			if (SelectedItems.Count > 0)
			{
				var selectedUsers = new List<User>();
				var fileDialog = new SaveFileDialog
				{
					Title = StringResources.Export_Export_selected_system_fields,
					Filter = @"Excel |*.xlsx"
				};

				var result = fileDialog.ShowDialog();
				if (result == DialogResult.OK && fileDialog.FileName != string.Empty)
				{
					foreach (var user in SelectedItems.OfType<User>())
					{
						selectedUsers.Add(user);
					}

					_usersService.ExportUsers(fileDialog.FileName, selectedUsers);
					MessageBox.Show(StringResources.Export_File_was_exported_successfully_to_selected_location, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
				}
			}
			else
			{
				MessageBox.Show(StringResources.Export_Please_select_at_least_one_row_to_export, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
			}
		}

		private void TmsCollection_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			if (e.Action == NotifyCollectionChangedAction.Add)
			{
				foreach (TmFile tm in e.NewItems)
				{
					AddTm(tm);
				}
			}

			if (e.Action == NotifyCollectionChangedAction.Remove)
			{
				if (e.OldItems == null) return;
				foreach (TmFile tm in e.OldItems)
				{
					RemoveTm(tm);
				}
			}
		}

		private void Tm_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName.Equals("IsSelected"))
			{
				if (!_backgroundWorker.IsBusy)
				{
					_backgroundWorker.RunWorkerAsync(sender);
				}
			}
		}

		private void BackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			_waitWindow?.Close();
			_translationMemoryViewModel.IsEnabled = true;
		}

		private void BackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
		{
			_translationMemoryViewModel.IsEnabled = false;
			var tm = e.Argument as TmFile;
			System.Windows.Application.Current.Dispatcher.Invoke(() =>
			{
				_waitWindow = new WaitWindow();
				_waitWindow.Show();
			});

			System.Windows.Application.Current.Dispatcher.Invoke(() =>
			{
				// add system fields for checked tms
				if (tm != null && tm.IsSelected)
				{
					SelectTm(tm);
				}
				else
				{
					//remove system fields for unchecked tms
					if (tm != null)
					{
						UnselectTm(tm);
					}
				}
			});
		}

		public void Dispose()
		{
			if (_backgroundWorker != null)
			{
				_backgroundWorker.DoWork -= BackgroundWorker_DoWork;
				_backgroundWorker.RunWorkerCompleted -= BackgroundWorker_RunWorkerCompleted;
				_backgroundWorker?.Dispose();
			}

			if (_tmsCollection != null)
			{
				foreach (var tm in _tmsCollection)
				{
					tm.PropertyChanged -= Tm_PropertyChanged;
				}

				_tmsCollection.CollectionChanged -= TmsCollection_CollectionChanged;
			}
		}
	}
}
