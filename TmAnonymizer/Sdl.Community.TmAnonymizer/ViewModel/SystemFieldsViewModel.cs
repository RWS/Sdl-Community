using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Input;
using Sdl.Community.SdlTmAnonymizer.Helpers;
using Sdl.Community.SdlTmAnonymizer.Model;
using Sdl.Community.SdlTmAnonymizer.Ui;
using Sdl.LanguagePlatform.TranslationMemoryApi;
using SystemFields = Sdl.Community.SdlTmAnonymizer.Helpers.SystemFields;


namespace Sdl.Community.SdlTmAnonymizer.ViewModel
{
	public class SystemFieldsViewModel:ViewModelBase
	{
		private readonly ObservableCollection<TmFile> _tmsCollection;
		private ObservableCollection<User> _uniqueUserNames;
		private static TranslationMemoryViewModel _translationMemoryViewModel;
		private readonly BackgroundWorker _backgroundWorker;
		private ICommand _selectAllCommand;
		private ICommand _applyChangesCommand;
		private ICommand _importCommand;
		private ICommand _exportCommand;
		private IList _selectedItems;
		private WaitWindow _waitWindow;
		private bool _selectAll;


		public SystemFieldsViewModel(TranslationMemoryViewModel translationMemoryViewModel)
		{
			_uniqueUserNames = new ObservableCollection<User>();
			_selectedItems = new List<User>();
			_translationMemoryViewModel = translationMemoryViewModel;
			_backgroundWorker = new BackgroundWorker();
			_backgroundWorker.DoWork += _backgroundWorker_DoWork;
			_backgroundWorker.RunWorkerCompleted += _backgroundWorker_RunWorkerCompleted;
			_tmsCollection = _translationMemoryViewModel.TmsCollection;
			_tmsCollection.CollectionChanged += _tmsCollection_CollectionChanged;
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
					SystemFields.AnonymizeFileBasedSystemFields(tm, UniqueUserNames.ToList());
				}

				else if (tm.IsServerTm)
				{
					var uri = new Uri(_translationMemoryViewModel.Credentials.Url);
					var translationProvider = new TranslationProviderServer(uri, false,
						_translationMemoryViewModel.Credentials.UserName,
						_translationMemoryViewModel.Credentials.Password);

					SystemFields.AnonymizeServerBasedSystemFields(tm, UniqueUserNames.ToList(), translationProvider);
				}
			}
			RefreshSystemFields();
		}

		private void RefreshSystemFields()
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
						var names = SystemFields.GetUniqueFileBasedSystemFields(fileTm);
						foreach (var name in names)
						{
							System.Windows.Application.Current.Dispatcher.Invoke(()=>
							{
								UniqueUserNames.Add(name);
							});
						}
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
						var names = SystemFields.GetUniqueServerBasedSystemFields(serverTm, translationProvider);
						foreach (var name in names)
						{
							System.Windows.Application.Current.Dispatcher.Invoke(()=>
							{
								UniqueUserNames.Add(name);
							});
						}
					}
				}
			}
		}

		private void Import()
		{
			var fileDialog = new OpenFileDialog
			{
				Title = @"Please select the files you want to import",
				Filter = @"Excel |*.xlsx",
				CheckFileExists = true,
				CheckPathExists = true,
				DefaultExt = "xlsx",
				Multiselect = true
			};
			var result = fileDialog.ShowDialog();
			if (result == DialogResult.OK && fileDialog.FileNames.Length > 0)
			{
				var importedUsers = Users.GetImportedUsers(fileDialog.FileNames.ToList());
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
					Title = @"Export selected system fields",
					Filter = @"Excel |*.xlsx"
				};
				var result = fileDialog.ShowDialog();
				if (result == DialogResult.OK && fileDialog.FileName != string.Empty)
				{
					foreach (User user in SelectedItems.OfType<User>())
					{
						selectedUsers.Add(user);
					}
					Users.ExportUsers(fileDialog.FileName, selectedUsers);
					MessageBox.Show(@"File was exported successfully to selected location", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
				}
			}
			else
			{
				MessageBox.Show(@"Please select at least one row to export", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
			}
		}

		private void _tmsCollection_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			if (e.Action == NotifyCollectionChangedAction.Add)
			{
				foreach (TmFile newTm in e.NewItems)
				{
					if (!newTm.IsServerTm)
					{
						var fields = SystemFields.GetUniqueFileBasedSystemFields(newTm);
						foreach (var user in fields)
						{
							UniqueUserNames.Add(user);
						}
					}
					newTm.PropertyChanged += NewTm_PropertyChanged;
				}
			}
			if (e.Action == NotifyCollectionChangedAction.Remove)
			{
				if (e.OldItems == null) return;
				foreach (TmFile removedTm in e.OldItems)
				{
					RemoveFieldsForTm(removedTm.Path);
				}
			}
		}

		private void RemoveFieldsForTm(string tmPath)
		{
			var userNamesToBeRemoved = UniqueUserNames.Where(t => t.TmFilePath.Equals(tmPath)).ToList();
			foreach (var userName in userNamesToBeRemoved)
			{
				UniqueUserNames.Remove(userName);
			}
		}

		private void NewTm_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName.Equals("IsSelected"))
			{
				if (!_backgroundWorker.IsBusy)
				{
					_backgroundWorker.RunWorkerAsync(sender);
				}
			}
		}

		private void _backgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			_waitWindow?.Close();
			_translationMemoryViewModel.IsEnabled = true;
		}

		private void _backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
		{
			_translationMemoryViewModel.IsEnabled = false;
			var tm = e.Argument as TmFile;
			System.Windows.Application.Current.Dispatcher.Invoke(()=>
			{
				_waitWindow = new WaitWindow();
				_waitWindow.Show();
			});
			System.Windows.Application.Current.Dispatcher.Invoke(() =>
			{
				// add system fields for checked tms
				if (tm != null && tm.IsSelected)
				{
					if (tm.IsServerTm)
					{
						var uri = new Uri(_translationMemoryViewModel.Credentials.Url);
						var translationProvider = new TranslationProviderServer(uri, false,
							_translationMemoryViewModel.Credentials.UserName,
							_translationMemoryViewModel.Credentials.Password);
						var names = SystemFields.GetUniqueServerBasedSystemFields(tm, translationProvider);
						foreach (var name in names)
						{
							UniqueUserNames.Add(name);
						}
					}
					else
					{
						var names = SystemFields.GetUniqueFileBasedSystemFields(tm);
						foreach (var name in names)
						{
							UniqueUserNames.Add(name);
						}
					}
				}
				else
				{
					//remove system fields for unchecked tms
					if (tm != null)
					{
						RemoveFieldsForTm(tm.Path);
					}
				}
			});
		}
	}
}
