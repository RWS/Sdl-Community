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
using Sdl.Community.SdlTmAnonymizer.Controls.ProgressDialog;
using Sdl.Community.SdlTmAnonymizer.Model;
using Sdl.Community.SdlTmAnonymizer.Services;
using Sdl.LanguagePlatform.TranslationMemoryApi;

namespace Sdl.Community.SdlTmAnonymizer.ViewModel
{
	public class SystemFieldsViewModel : ViewModelBase, IDisposable
	{
		private readonly ObservableCollection<TmFile> _tmsCollection;
		private ObservableCollection<User> _uniqueUserNames;
		private readonly TranslationMemoryViewModel _model;
		private User _selectedItem;
		private ICommand _selectAllCommand;
		private ICommand _applyChangesCommand;
		private ICommand _importCommand;
		private ICommand _exportCommand;
		private IList _selectedItems;
		private bool _selectAll;
		private readonly SystemFieldsService _systemFieldsService;
		private readonly ExcelImportExportService _excelImportExportService;

		public SystemFieldsViewModel(TranslationMemoryViewModel model, SystemFieldsService systemFieldsService, ExcelImportExportService excelImportExportService)
		{
			_systemFieldsService = systemFieldsService;
			_excelImportExportService = excelImportExportService;

			_model = model;

			_tmsCollection = _model.TmsCollection;
			_tmsCollection.CollectionChanged += TmsCollection_CollectionChanged;

			InitializeComponents();
		}

		public IList SelectedItems
		{
			get => _selectedItems ?? (_selectedItems = new List<User>());
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
			get => _uniqueUserNames ?? (_uniqueUserNames = new ObservableCollection<User>());
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

			UpdateCheckedAllState();
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
			if (!tm.IsSelected)
			{
				return;
			}
			
			var users = new List<User>();
			var settings = new ProgressDialogSettings(_model.ControlParent, true, true, false);
			var result = ProgressDialog.Execute(StringResources.Loading_data, () =>
			{
				ProgressDialog.Current.Report(0, tm.Path);

				if (tm.IsServerTm)
				{
					var uri = new Uri(tm.Credentials.Url);
					var translationProvider = new TranslationProviderServer(uri, false,
						tm.Credentials.UserName,
						tm.Credentials.Password);

					users.AddRange(_systemFieldsService.GetUniqueServerBasedSystemFields(ProgressDialog.Current, tm,
						translationProvider));
				}
				else
				{
					users.AddRange(_systemFieldsService.GetUniqueFileBasedSystemFields(ProgressDialog.Current, tm));
				}

			}, settings);

			if (result.Cancelled)
			{
				tm.IsSelected = false;
				MessageBox.Show(StringResources.Process_cancelled_by_user, Application.ProductName);
			}
			if (result.OperationFailed)
			{
				tm.IsSelected = false;
				MessageBox.Show(StringResources.Process_failed + "\r\n\r\n" + result.Error.Message, Application.ProductName);
			}
			else
			{				
				AddUniqueUserNames(users);
			}
		}

		private void AddUniqueUserNames(IEnumerable<User> userNames)
		{
			foreach (var name in userNames)
			{
				if (!UniqueUserNames.Contains(name))
				{
					name.PropertyChanged += Name_PropertyChanged;
					UniqueUserNames.Add(name);
				}
			}
		}

		private void Name_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			UpdateCheckedAllState();
		}

		private void UpdateCheckedAllState()
		{
			if (UniqueUserNames.Count > 0)
			{
				SelectAll = UniqueUserNames.Count(a => !a.IsSelected) <= 0;
			}
			else
			{
				SelectAll = false;
			}
		}

		private void UnselectTm(TmFile tm)
		{
			var userNames = UniqueUserNames.Where(t => t.TmFilePath.Equals(tm.Path)).ToList();
			foreach (var name in userNames)
			{
				name.PropertyChanged -= Name_PropertyChanged;
				UniqueUserNames.Remove(name);
			}
		}

		private void SelectAllUserNames()
		{
			var value = SelectAll;
			foreach (var userName in UniqueUserNames)
			{
				userName.IsSelected = value;
			}
		}

		private void ApplyChanges()
		{
			var settings = new ProgressDialogSettings(_model.ControlParent, true, true, false);
			var result = ProgressDialog.Execute(StringResources.Applying_changes, () =>
			{
				foreach (var tm in _tmsCollection.Where(t => t.IsSelected))
				{
					if (!tm.IsServerTm)
					{
						_systemFieldsService.AnonymizeFileBasedSystemFields(ProgressDialog.Current, tm, UniqueUserNames.ToList());
					}

					else if (tm.IsServerTm)
					{
						var uri = new Uri(tm.Credentials.Url);
						var translationProvider = new TranslationProviderServer(uri, false,
							tm.Credentials.UserName,
							tm.Credentials.Password);

						_systemFieldsService.AnonymizeServerBasedSystemFields(ProgressDialog.Current, tm, UniqueUserNames.ToList(), translationProvider);
					}
				}
			}, settings);

			if (result.Cancelled)
			{
				MessageBox.Show(StringResources.Process_cancelled_by_user, Application.ProductName);
			}
			if (result.OperationFailed)
			{
				MessageBox.Show(StringResources.Process_failed + "\r\n\r\n" + result.Error.Message, Application.ProductName);
			}

			Refresh();
		}

		private void Refresh()
		{
			if (_tmsCollection != null)
			{
				UniqueUserNames.Clear();

				if (_model.ControlParent == null)
				{
					return;
				}

				var users = new List<User>();

				var serverTms = _tmsCollection.Where(s => s.IsServerTm && s.IsSelected).ToList();
				var fileBasedTms = _tmsCollection.Where(s => !s.IsServerTm && s.IsSelected).ToList();
				if (fileBasedTms.Any())
				{
					foreach (var tm in fileBasedTms)
					{
						users.AddRange(_systemFieldsService.GetUniqueFileBasedSystemFields(ProgressDialog.Current, tm));
					}
				}

				if (serverTms.Any())
				{
					foreach (var tm in serverTms)
					{
						var uri = new Uri(tm.Credentials.Url);
						var translationProvider = new TranslationProviderServer(uri, false,
							tm.Credentials.UserName,
							tm.Credentials.Password);

						users.AddRange(_systemFieldsService.GetUniqueServerBasedSystemFields(ProgressDialog.Current, tm, translationProvider));
					}
				}

				AddUniqueUserNames(users);

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
				var importedUsers = _excelImportExportService.ImportUsers(fileDialog.FileNames.ToList());
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

					_excelImportExportService.ExportUsers(fileDialog.FileName, selectedUsers);
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
			if (e.PropertyName.Equals(nameof(TmFile.IsSelected)))
			{
				if (!(sender is TmFile tm))
				{
					return;
				}

				if (ProgressDialog.Current == null)
				{
					if (tm.IsSelected)
					{
						SelectTm(tm);
					}
					else
					{
						UnselectTm(tm);
					}
				}
			}
		}

		public void Dispose()
		{
			if (_tmsCollection != null)
			{
				foreach (var tm in _tmsCollection)
				{
					tm.PropertyChanged -= Tm_PropertyChanged;
				}

				_tmsCollection.CollectionChanged -= TmsCollection_CollectionChanged;
			}

			foreach (var name in UniqueUserNames)
			{
				name.PropertyChanged -= Name_PropertyChanged;
			}
		}
	}
}
