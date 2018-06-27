using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;
using Sdl.Community.TmAnonymizer.Helpers;
using Sdl.Community.TmAnonymizer.Model;
using Sdl.Community.TmAnonymizer.Ui;
using Sdl.LanguagePlatform.TranslationMemoryApi;

namespace Sdl.Community.TmAnonymizer.ViewModel
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
		private ObservableCollection<SourceSearchResult> _sourceSearchResults;
		private readonly List<AnonymizeTranslationMemory> _anonymizeTranslationMemories;
		private IList _selectedItems;
		private WaitWindow _waitWindow;
		private bool _selectAll;


		public SystemFieldsViewModel(TranslationMemoryViewModel translationMemoryViewModel)
		{
			_uniqueUserNames = new ObservableCollection<User>();
			_selectedItems = new List<User>();
			_sourceSearchResults = new ObservableCollection<SourceSearchResult>();
			_translationMemoryViewModel = translationMemoryViewModel;
			if (_tmsCollection != null)
			{
				PopulateSystemFieldGrid(_tmsCollection, _translationMemoryViewModel);
			}

			_backgroundWorker = new BackgroundWorker();
			_backgroundWorker.DoWork += _backgroundWorker_DoWork;
			_backgroundWorker.RunWorkerCompleted += _backgroundWorker_RunWorkerCompleted;
			_tmsCollection = _translationMemoryViewModel.TmsCollection;
			_tmsCollection.CollectionChanged += _tmsCollection_CollectionChanged;
			_anonymizeTranslationMemories = new List<AnonymizeTranslationMemory>();
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
					Helpers.SystemFields.AnonymizeFileBasedSystemFields(tm, UniqueUserNames.ToList());
				}

				else if (tm.IsServerTm)
				{
					var uri = new Uri(_translationMemoryViewModel.Credentials.Url);
					var translationProvider = new TranslationProviderServer(uri, false,
						_translationMemoryViewModel.Credentials.UserName,
						_translationMemoryViewModel.Credentials.Password);

					Helpers.SystemFields.AnonymizeServerBasedSystemFields(tm, UniqueUserNames.ToList(), translationProvider);
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
						var names = Helpers.SystemFields.GetUniqueFileBasedSystemFields(fileTm);
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
						var names = Helpers.SystemFields.GetUniqueServerBasedSystemFields(serverTm, translationProvider);
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
					var userExist = UniqueUserNames.FirstOrDefault(u => u.UserName.Equals(user.UserName));
					if (userExist != null)
					{
						var index = UniqueUserNames.IndexOf(userExist);
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
					Title = @"Export selected users",
					Filter = @"Excel |*.xlsx"
				};
				var result = fileDialog.ShowDialog();
				if (result == DialogResult.OK && fileDialog.FileName != string.Empty)
				{
					foreach (User user in SelectedItems)
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
			if (e.NewItems != null)
			{
				foreach (TmFile newTm in e.NewItems)
				{
					if (!newTm.IsServerTm)
					{
						UniqueUserNames = Helpers.SystemFields.GetUniqueFileBasedSystemFields(newTm);
					}
					
					newTm.PropertyChanged += NewTm_PropertyChanged;
				}
			}
		}

		private void NewTm_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName.Equals("IsSelected"))
			{
				_backgroundWorker.RunWorkerAsync(sender);
			}

		}

		private void _backgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			if (_waitWindow != null)
			{
				_waitWindow.Close();
			}
		}

		private void _backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
		{

			var tm = e.Argument as TmFile;
			System.Windows.Application.Current.Dispatcher.Invoke(()=>
			{
				_waitWindow = new WaitWindow();
				_waitWindow.Show();
			});
			if (tm.IsSelected)
			{
				if (tm.IsServerTm)
				{
					var uri = new Uri(_translationMemoryViewModel.Credentials.Url);
					var translationProvider = new TranslationProviderServer(uri, false,
						_translationMemoryViewModel.Credentials.UserName,
						_translationMemoryViewModel.Credentials.Password);
					var names = Helpers.SystemFields.GetUniqueServerBasedSystemFields(tm, translationProvider);
					foreach (var name in names)
					{
						System.Windows.Application.Current.Dispatcher.Invoke(() =>
						{
							UniqueUserNames.Add(name);
						});
					}
				}
				else
				{
					var names = Helpers.SystemFields.GetUniqueFileBasedSystemFields(tm);
					foreach (var name in names)
					{
						System.Windows.Application.Current.Dispatcher.Invoke(() =>
						{
							UniqueUserNames.Add(name);
						});
					}
				}
			}
			else
			{
				if (tm.IsServerTm)
				{
					var uri = new Uri(_translationMemoryViewModel.Credentials.Url);
					var translationProvider = new TranslationProviderServer(uri, false,
						_translationMemoryViewModel.Credentials.UserName,
						_translationMemoryViewModel.Credentials.Password);
					var names = Helpers.SystemFields.GetUniqueServerBasedSystemFields(tm, translationProvider);
					var newList = UniqueUserNames.ToList();
					foreach (var name in names)
					{
						newList.RemoveAll(n => n.UserName.Equals( name.UserName));
					}
					UniqueUserNames = new ObservableCollection<User>(newList);
				}
				else
				{
					var names = Helpers.SystemFields.GetUniqueFileBasedSystemFields(tm);
					var newList = UniqueUserNames.ToList();
					foreach (var name in names)
					{
						newList.RemoveAll(n => n.UserName.Equals(name.UserName));
					}
					UniqueUserNames = new ObservableCollection<User>(newList);
				}
			}
		}

		private void PopulateSystemFieldGrid(ObservableCollection<TmFile> tmsCollection, TranslationMemoryViewModel translationMemoryViewModel)
		{
			var serverBasedTms = tmsCollection.Where(s => s.IsServerTm && s.IsSelected).ToList();
			var fileBasedTms = tmsCollection.Where(s => !s.IsServerTm && s.IsSelected).ToList();
			if (serverBasedTms.Any())
			{
				var uri = new Uri(translationMemoryViewModel.Credentials.Url);
				var translationProvider = new TranslationProviderServer(uri, false,
					translationMemoryViewModel.Credentials.UserName,
					translationMemoryViewModel.Credentials.Password);
				foreach (var serverTm in serverBasedTms)
				{
					UniqueUserNames = Helpers.SystemFields.GetUniqueServerBasedSystemFields(serverTm, translationProvider);
				}

			}

			if (fileBasedTms.Any())
			{
				foreach (var fileTm in fileBasedTms)
				{
					UniqueUserNames = Helpers.SystemFields.GetUniqueFileBasedSystemFields(fileTm);
				}
			}
		}
	}
}
