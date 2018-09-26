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

namespace Sdl.Community.SdlTmAnonymizer.ViewModel
{
	public class CustomFieldsViewModel : ViewModelBase, IDisposable
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
			_backgroundWorker.DoWork += BackgroundWorker_DoWork;
			_backgroundWorker.RunWorkerCompleted += BackgroundWorker_RunWorkerCompleted;

			_tmsCollection = _translationMemoryViewModel.TmsCollection;
			_tmsCollection.CollectionChanged += TmsCollection_CollectionChanged;

			InitializeComponents();
		}

		public ICommand SelectAllCommand => _selectAllCommand ?? (_selectAllCommand = new CommandHandler(SelectFields, true));
		public ICommand ImportCommand => _importCommand ?? (_importCommand = new CommandHandler(Import, true));
		public ICommand ApplyCommand => _applyCommand ?? (_applyCommand = new CommandHandler(ApplyChanges, true));
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

				var customFields = new List<CustomField>(CustomFieldsHandler.GetServerBasedCustomFields(tm, translationProvider));
				foreach (var field in customFields)
				{
					CustomFieldsCollection.Add(field);
				}
			}
			else
			{
				var customFields = new List<CustomField>(CustomFieldsHandler.GetFilebasedCustomField(tm));
				foreach (var field in customFields)
				{
					CustomFieldsCollection.Add(field);
				}
			}
		}

		private void UnselectTm(TmFile tm)
		{
			var customFieldsToBeRemoved = CustomFieldsCollection.Where(c => c.TmPath.Equals(tm.Path)).ToList();

			foreach (var customField in customFieldsToBeRemoved)
			{
				CustomFieldsCollection.Remove(customField);
			}
		}

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
					System.Windows.Application.Current.Dispatcher.Invoke(delegate
					{
						_waitWindow = new WaitWindow();
						_waitWindow.Show();
					});

					var uri = new Uri(_translationMemoryViewModel.Credentials.Url);
					var translationProvider = new TranslationProviderServer(uri, false,
						_translationMemoryViewModel.Credentials.UserName,
						_translationMemoryViewModel.Credentials.Password);

					CustomFieldsHandler.AnonymizeServerBasedCustomFields(tm, CustomFieldsCollection.ToList(), translationProvider);
					_waitWindow.Close();
				}

				Refresh();
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
					MessageBox.Show(StringResources.Export_File_was_exported_successfully_to_selected_location, Application.ProductName, MessageBoxButtons.OK,
						MessageBoxIcon.Information);
				}
			}
			else
			{
				MessageBox.Show(StringResources.Export_Please_select_at_least_one_row_to_export, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
			}
		}

		private void Import()
		{
			var confirmation = MessageBox.Show(StringResources.Import_Existing_fields_values_will_be_overwritten_with_the_values_form_the_file, StringResources.Import_Are_you_sure_you_want_to_import_an_excel_file,
				MessageBoxButtons.OKCancel, MessageBoxIcon.Question);

			if (confirmation == DialogResult.OK)
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
					var importedCustomFields = CustomFieldData.GetImportedCustomFields(fileDialog.FileNames.ToList());
					foreach (var importedField in importedCustomFields)
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

		private void TmsCollection_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			if (e.Action == NotifyCollectionChangedAction.Add)
			{
				foreach (TmFile tm in e.NewItems)
				{
					//custom fields for server based tms wil be loaded only when user check the checkbox
					//if (!tm.IsServerTm)
					//{
					//	var customFields = CustomFieldsHandler.GetFilebasedCustomField(tm);
					//	foreach (var customField in customFields)
					//	{
					//		CustomFieldsCollection.Add(customField);
					//	}
					//}

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
		}

		private void BackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
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
					SelectTm(tm);
				}
				else
				{   //remove custom fields for uncheked tm
					if (tm != null)
					{
						UnselectTm(tm);
					}
				}
			});
		}

		private void Refresh()
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
						var fields = new List<CustomField>(CustomFieldsHandler.GetFilebasedCustomField(fileTm));
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
						var fields = new List<CustomField>(CustomFieldsHandler.GetServerBasedCustomFields(serverTm, translationProvider));
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