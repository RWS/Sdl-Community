﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Input;
using Sdl.Community.SdlDataProtectionSuite.SdlTmAnonymizer.Commands;
using Sdl.Community.SdlDataProtectionSuite.SdlTmAnonymizer.Controls.ProgressDialog;
using Sdl.Community.SdlDataProtectionSuite.SdlTmAnonymizer.Model;
using Sdl.Community.SdlDataProtectionSuite.SdlTmAnonymizer.Model.Log;
using Sdl.Community.SdlDataProtectionSuite.SdlTmAnonymizer.Services;
using Sdl.LanguagePlatform.TranslationMemoryApi;

namespace Sdl.Community.SdlDataProtectionSuite.SdlTmAnonymizer.ViewModel
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
		private readonly SerializerService _serializerService;

		public SystemFieldsViewModel(TranslationMemoryViewModel model, SystemFieldsService systemFieldsService,
			ExcelImportExportService excelImportExportService, SerializerService serializerService)
		{
			_systemFieldsService = systemFieldsService;
			_excelImportExportService = excelImportExportService;
			_serializerService = serializerService;

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

			var userNames = new List<User>();
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

					userNames.AddRange(_systemFieldsService.GetUniqueServerBasedSystemFields(ProgressDialog.Current, tm,
						translationProvider));
				}
				else
				{
					userNames.AddRange(_systemFieldsService.GetUniqueFileBasedSystemFields(ProgressDialog.Current, tm));
				}

			}, settings);

			if (result.Cancelled)
			{
				tm.IsSelected = false;
				MessageBox.Show(StringResources.Process_cancelled_by_user, PluginResources.ProductName);
			}
			if (result.OperationFailed)
			{
				tm.IsSelected = false;
				MessageBox.Show(StringResources.Process_failed + Environment.NewLine + Environment.NewLine + result.Error.Message, PluginResources.ProductName);
			}
			else
			{
				AddUniqueUserNames(userNames);

				if (userNames.Count > 0 && SelectedItem == null)
				{
					SelectedItem = userNames[0];
				}
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
					Report report;
					if (!tm.IsServerTm)
					{
						report = _systemFieldsService.AnonymizeFileBasedSystemFields(ProgressDialog.Current, tm, UniqueUserNames.ToList());
					}
					else
					{
						var uri = new Uri(tm.Credentials.Url);
						var translationProvider = new TranslationProviderServer(uri, false,
							tm.Credentials.UserName,
							tm.Credentials.Password);

						report = _systemFieldsService.AnonymizeServerBasedSystemFields(ProgressDialog.Current, tm, UniqueUserNames.ToList(), translationProvider);
					}

					_serializerService.Save<Model.Log.Report>(report, report.ReportFullPath);
				}
			}, settings);

			if (result.Cancelled)
			{
				MessageBox.Show(StringResources.Process_cancelled_by_user, PluginResources.ProductName);
			}
			if (result.OperationFailed)
			{
				MessageBox.Show(StringResources.Process_failed + Environment.NewLine + Environment.NewLine + result.Error.Message, PluginResources.ProductName);
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

				_model.Refresh();
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
							UniqueUserNames[index].Alias = user.Alias;
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

					if (!fileDialog.FileName.ToLower().EndsWith(".xlsx"))
					{
						fileDialog.FileName += ".xlsx";
					}

					_excelImportExportService.ExportUsers(fileDialog.FileName, selectedUsers);
					MessageBox.Show(StringResources.Export_File_was_exported_successfully_to_selected_location, PluginResources.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);

					if (SelectedItem != null && File.Exists(fileDialog.FileName))
					{
						System.Diagnostics.Process.Start("\"" + fileDialog.FileName + "\"");
					}
				}
			}
			else
			{
				MessageBox.Show(StringResources.Export_Please_select_at_least_one_row_to_export, PluginResources.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
