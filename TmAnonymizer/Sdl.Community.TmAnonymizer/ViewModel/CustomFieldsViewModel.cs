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
using Sdl.Community.TmAnonymizer.Model;
using Sdl.LanguagePlatform.TranslationMemoryApi;

namespace Sdl.Community.SdlTmAnonymizer.ViewModel
{
	public class CustomFieldsViewModel : ViewModelBase
	{
		private readonly ObservableCollection<TmFile> _tmsCollection;
		private static TranslationMemoryViewModel _translationMemoryViewModel;
		private bool _selectAll;
		private ObservableCollection<CustomField> _customFields;
		private ICommand _selectAllCommand;
		private ICommand _applyCommand;
		private ICommand _importCommand;
		private ICommand _exportCommand;
		private IList _selectedItems;
		private readonly BackgroundWorker _backgroundWorker;
		private WaitWindow _waitWindow;

		public CustomFieldsViewModel(TranslationMemoryViewModel translationMemoryViewModel)
		{
			_customFields = new ObservableCollection<CustomField>();
			_selectedItems = new List<CustomField>();
			_translationMemoryViewModel = translationMemoryViewModel;
			_backgroundWorker = new BackgroundWorker();
			_backgroundWorker.DoWork += _backgroundWorker_DoWork;
			_backgroundWorker.RunWorkerCompleted += _backgroundWorker_RunWorkerCompleted;
			_tmsCollection = _translationMemoryViewModel.TmsCollection;
			_tmsCollection.CollectionChanged += _tmsCollection_CollectionChanged;

		}

		public ICommand SelectAllCommand => _selectAllCommand ?? (_selectAllCommand = new CommandHandler(SelectFields, true));
		public ICommand ImportCommand => _importCommand ?? (_importCommand = new CommandHandler(Import, true));
		public ICommand ApplyCommand => _applyCommand ?? (_applyCommand = new CommandHandler(ApplyChanges, true));
		public ICommand ExportCommand => _exportCommand ?? (_exportCommand = new CommandHandler(Export, true));
		private void ApplyChanges()
		{
			foreach (var tm in _tmsCollection.Where(t => t.IsSelected))
			{
				if (!tm.IsServerTm)
				{
					CustomFieldsHandler.AnonymizeFileBasedCustomFields(tm, CustomFieldsCollection.ToList());
				}
				else
				{
					var uri = new Uri(_translationMemoryViewModel.Credentials.Url);
					var translationProvider = new TranslationProviderServer(uri, false,
						_translationMemoryViewModel.Credentials.UserName,
						_translationMemoryViewModel.Credentials.Password);
					CustomFieldsHandler.AnonymizeServerBasedCustomFields(tm, CustomFieldsCollection.ToList(), translationProvider);
				}
				RefreshCustomFields();
			}
		}

		private void Export()
		{
			if (SelectedItems.Count > 0)
			{
				var fileDialog = new SaveFileDialog
				{
					Title = @"Export selected custom fields",
					Filter = @"Excel |*.xlsx"
				};
				var result = fileDialog.ShowDialog();
				var valuesToBeAnonymized = CustomFieldsCollection.Where(f => f.IsSelected).ToList();
				
				if (result == DialogResult.OK && fileDialog.FileName != string.Empty)
				{
					CustomFieldData.ExportCustomFields(fileDialog.FileName, valuesToBeAnonymized);
					MessageBox.Show(@"File was exported successfully to selected location", "", MessageBoxButtons.OK,
						MessageBoxIcon.Information);
				}
			}
			else
			{
				MessageBox.Show(@"Please select at least one row to export", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
			}
		}

		private void Import()
		{
			var confirmation = MessageBox.Show(@"Existing fields values will be overwritten with the values form the file", @"Are you sure you want to import an excel file?", 
				MessageBoxButtons.OKCancel,MessageBoxIcon.Question);
			if (confirmation == DialogResult.OK)
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
					var importedCustomFields = CustomFieldData.GetImportedCustomFields(fileDialog.FileNames.ToList());
					foreach (var  importedField in importedCustomFields)
					{
						var customFieldToBeAnonymized = CustomFieldsCollection.FirstOrDefault(c => c.Name.Equals(importedField.Name));
						if (customFieldToBeAnonymized != null)
						{
							var index = CustomFieldsCollection.IndexOf(customFieldToBeAnonymized);
							customFieldToBeAnonymized.IsSelected = true;
							customFieldToBeAnonymized.Details = importedField.Details;
							CustomFieldsCollection.RemoveAt(index);
							CustomFieldsCollection.Insert(index, customFieldToBeAnonymized);
						}
					}
				}
			}
		}

		private void SelectFields()
		{
			foreach (var field in CustomFieldsCollection)
			{
				field.IsSelected = SelectAll;
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
		public IList SelectedItems
		{
			get => _selectedItems;
			set
			{
				_selectedItems = value;
				OnPropertyChanged(nameof(SelectedItems));
			}
		}

		

		public ObservableCollection<CustomField> CustomFieldsCollection
		{
			get => _customFields;

			set
			{
				if (Equals(value, _customFields))
				{
					return;
				}
				_customFields = value;
				OnPropertyChanged(nameof(CustomFieldsCollection));

			}
		}
		

		private void _tmsCollection_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			if (e.Action==NotifyCollectionChangedAction.Add)
			{
				foreach (TmFile newTm in e.NewItems)
				{
					//custom fields for server based tms wil be loaded only when user check the checkbox
					if (!newTm.IsServerTm)
					{
						var customFields = CustomFieldsHandler.GetFilebasedCustomField(newTm);
						foreach (var customField in customFields)
						{
							CustomFieldsCollection.Add(customField);
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
					RemoveCustomFieldsForTm(removedTm.Path);
				}
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
		}

		private void _backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
		{
			var tm = e.Argument as TmFile;
			System.Windows.Application.Current.Dispatcher.Invoke(() =>
			{
				_waitWindow = new WaitWindow();
				_waitWindow.Show();
			});
			System.Windows.Application.Current.Dispatcher.Invoke(() =>
			{
				if (tm != null && tm.IsSelected)
				{

					if (tm.IsServerTm)
					{
						var uri = new Uri(_translationMemoryViewModel.Credentials.Url);
						var translationProvider = new TranslationProviderServer(uri, false,
							_translationMemoryViewModel.Credentials.UserName,
							_translationMemoryViewModel.Credentials.Password);
						var customFields =
							new ObservableCollection<CustomField>(CustomFieldsHandler.GetServerBasedCustomFields(tm, translationProvider));
						foreach (var field in customFields)
						{
							CustomFieldsCollection.Add(field);
						}
					}
					else
					{
						var customFields = new ObservableCollection<CustomField>(CustomFieldsHandler.GetFilebasedCustomField(tm));
						foreach (var field in customFields)
						{
							CustomFieldsCollection.Add(field);
						}
					}
				}
				else
				{   //remove custom fields for uncheked tm
					if (tm != null)
					{
						RemoveCustomFieldsForTm(tm.Path);
					}
				}
			});
		}

		private void RemoveCustomFieldsForTm(string tmFilePath)
		{
			var customFieldsToBeRemoved = CustomFieldsCollection.Where(c => c.TmPath.Equals(tmFilePath)).ToList();
			foreach (var customField in customFieldsToBeRemoved)
			{
				CustomFieldsCollection.Remove(customField);
			}
		}

		private void RefreshCustomFields()
		{
			if (_tmsCollection != null)
			{
				CustomFieldsCollection = new ObservableCollection<CustomField>();
				var serverTms = _tmsCollection.Where(s => s.IsServerTm && s.IsSelected).ToList();
				var fileBasedTms = _tmsCollection.Where(s => !s.IsServerTm && s.IsSelected).ToList();
				if (fileBasedTms.Any())
				{
					foreach (var fileTm in fileBasedTms)
					{
						var fields = new ObservableCollection<CustomField>(CustomFieldsHandler.GetFilebasedCustomField(fileTm));
						foreach (var field in fields)
						{
							System.Windows.Application.Current.Dispatcher.Invoke(() =>
							{
								CustomFieldsCollection.Add(field);
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
						var fields = new ObservableCollection<CustomField>(CustomFieldsHandler.GetServerBasedCustomFields(serverTm, translationProvider));
						foreach (var field in fields)
						{
							System.Windows.Application.Current.Dispatcher.Invoke(() =>
							{
								CustomFieldsCollection.Add(field);
							});
						}
					}
				}
			}
		}
	}
}



