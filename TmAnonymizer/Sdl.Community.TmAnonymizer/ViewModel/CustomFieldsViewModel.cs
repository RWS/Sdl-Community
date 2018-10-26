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
using Sdl.Community.SdlTmAnonymizer.Model.Log;
using Sdl.Community.SdlTmAnonymizer.Services;
using Sdl.LanguagePlatform.TranslationMemoryApi;

namespace Sdl.Community.SdlTmAnonymizer.ViewModel
{
	public class CustomFieldsViewModel : ViewModelBase, IDisposable
	{
		private readonly ObservableCollection<TmFile> _tmsCollection;
		private static TranslationMemoryViewModel _model;
		private bool _selectAll;
		private ObservableCollection<CustomField> _customFields;
		private ObservableCollection<CustomFieldValue> _customFieldValues;
		private CustomField _selectedItem;
		private ICommand _selectAllCommand;
		private ICommand _applyCommand;
		private ICommand _importCommand;
		private ICommand _exportCommand;
		private IList _selectedItems;
		private readonly CustomFieldsService _customFieldsService;
		private readonly ExcelImportExportService _excelImportExportService;
		private readonly SettingsService _settingsService;
		private readonly SerializerService _serializerService;

		public CustomFieldsViewModel(TranslationMemoryViewModel model, CustomFieldsService customFieldsService, 
			ExcelImportExportService excelImportExportService, SerializerService serializerService)
		{
			_customFieldsService = customFieldsService;
			_excelImportExportService = excelImportExportService;

			_serializerService = serializerService;

			_model = model;
			_settingsService = model.SettingsService;

			_tmsCollection = _model.TmsCollection;
			_tmsCollection.CollectionChanged += TmsCollection_CollectionChanged;

			InitializeComponents();
		}

		public ICommand SelectAllCommand => _selectAllCommand ?? (_selectAllCommand = new CommandHandler(SelectFields, true));
		public ICommand ImportCommand => _importCommand ?? (_importCommand = new CommandHandler(Import, true));
		public ICommand ApplyCommand => _applyCommand ?? (_applyCommand = new CommandHandler(ApplyChanges, true));
		public ICommand ExportCommand => _exportCommand ?? (_exportCommand = new CommandHandler(Export, true));

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
			get => _selectedItems ?? (_selectedItems = new List<CustomField>());
			set
			{
				_selectedItems = value;
				OnPropertyChanged(nameof(SelectedItems));
			}
		}

		public CustomField SelectedItem
		{
			get => _selectedItem;
			set
			{
				if (Equals(value, _selectedItem))
				{
					return;
				}

				_selectedItem = value;

				CustomFieldsValues = new ObservableCollection<CustomFieldValue>(_selectedItem?.FieldValues);

				UpdateCheckedAllState();
				OnPropertyChanged(nameof(SelectedItem));
			}
		}		

		public ObservableCollection<CustomField> CustomFields
		{
			get => _customFields ?? (_customFields = new ObservableCollection<CustomField>());
			set
			{
				if (Equals(value, _customFields))
				{
					return;
				}

				if (value != null)
				{
					foreach (var customField in value)
					{
						foreach (var customFieldValue in customField.FieldValues)
						{
							customFieldValue.PropertyChanged -= FieldValue_PropertyChanged;
						}
					}
				}

				_customFields = value;

				if (_customFields != null)
				{
					foreach (var customField in _customFields)
					{
						foreach (var customFieldValue in customField.FieldValues)
						{
							customFieldValue.PropertyChanged += FieldValue_PropertyChanged;
						}
					}
				}

				OnPropertyChanged(nameof(CustomFields));
			}
		}

		public ObservableCollection<CustomFieldValue> CustomFieldsValues
		{
			get => _customFieldValues ?? (_customFieldValues = new ObservableCollection<CustomFieldValue>());
			set
			{
				if (Equals(value, _customFieldValues))
				{
					return;
				}

				_customFieldValues = value;

				OnPropertyChanged(nameof(CustomFieldsValues));
			}
		}

		private void FieldValue_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			foreach (var customField in CustomFields)
			{
				customField.IsSelected = customField.FieldValues.Count(a => a.IsSelected) > 0;
			}

			OnPropertyChanged(nameof(CustomFields));

			UpdateCheckedAllState();
		}

		private void UpdateCheckedAllState()
		{
			if (SelectedItem is CustomField selectedFeield)
			{
				if (selectedFeield.FieldValues.Count > 0)
				{
					SelectAll = selectedFeield.FieldValues.Count(a => !a.IsSelected) <= 0;
				}
				else
				{
					SelectAll = false;
				}
			}
		}

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
			if (!tm.IsSelected)
			{
				return;
			}

			var customFields = new List<CustomField>();
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

					customFields.AddRange(_customFieldsService.GetServerBasedCustomFields(ProgressDialog.Current, tm,
						translationProvider));
				}
				else
				{
					customFields.AddRange(_customFieldsService.GetFilebasedCustomField(ProgressDialog.Current, tm));
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
				foreach (var field in customFields)
				{
					AddCustomFieldValue(field);
				}

				if (CustomFields.Count > 0 && SelectedItem == null)
				{
					SelectedItem = customFields[0];
				}
			}
		}

		private void UnselectTm(TmFile tm)
		{
			var customFieldsToBeRemoved = CustomFields.Where(c => c.TmPath.Equals(tm.Path)).ToList();

			foreach (var field in customFieldsToBeRemoved)
			{
				RemoveCustomFieldValue(field);
			}

			if (SelectedItem == null)
			{
				if (CustomFields?.Count > 0)
				{
					SelectedItem = CustomFields[0];
				}
				else
				{
					CustomFieldsValues.Clear();
				}
			}
		}

		private void ApplyChanges()
		{
			var settings = new ProgressDialogSettings(_model.ControlParent, true, true, false);
			var result = ProgressDialog.Execute(StringResources.Applying_changes, () =>
			{
				foreach (var tm in _tmsCollection.Where(t => t.IsSelected))
				{
					ProgressDialog.Current.Report(0, tm.Path);

					Report report;
					if (!tm.IsServerTm)
					{
						report = _customFieldsService.AnonymizeFileBasedCustomFields(ProgressDialog.Current, tm, CustomFields.ToList());
					}
					else
					{
						var uri = new Uri(tm.Credentials.Url);
						var translationProvider = new TranslationProviderServer(uri, false,
							tm.Credentials.UserName,
							tm.Credentials.Password);

						report = _customFieldsService.AnonymizeServerBasedCustomFields(ProgressDialog.Current, tm, CustomFields.ToList(), translationProvider);
					}

					_serializerService.Save<Model.Log.Report>(report, report.ReportFullPath);				
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

		private void Export()
		{
			if (SelectedItems.Count > 0)
			{
				var fileDialog = new SaveFileDialog
				{
					Title = StringResources.Export_Export_selected_custom_fields,
					Filter = @"Excel |*.xlsx"
				};
				var result = fileDialog.ShowDialog();
				var valuesToBeAnonymized = new List<CustomField>();
				foreach (var item in SelectedItems)
				{
					valuesToBeAnonymized.Add(item as CustomField);
				}

				if (result == DialogResult.OK && fileDialog.FileName != string.Empty)
				{
					_excelImportExportService.ExportCustomFields(fileDialog.FileName, valuesToBeAnonymized);
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
					var importedFields = _excelImportExportService.ImportCustomFields(fileDialog.FileNames.ToList());

					foreach (var importedField in importedFields)
					{
						var existingField = CustomFields.FirstOrDefault(c => c.Name.Equals(importedField.Name));

						if (existingField != null)
						{
							foreach (var importedFieldValue in importedField.FieldValues)
							{
								if (importedFieldValue == null)
								{
									continue;
								}

								var existingFieldValue = existingField.FieldValues.FirstOrDefault(a => a.Value == importedFieldValue.Value);
								if (existingFieldValue != null && FieldValuesAreEqual(existingFieldValue.NewValue, importedFieldValue.NewValue))
								{
									existingFieldValue.IsSelected = true;
									existingFieldValue.NewValue = importedFieldValue.NewValue;
								}
							}
						}
					}
				}
			}
		}

		private void Refresh()
		{
			if (_tmsCollection != null)
			{
				var selectedItem = SelectedItem;

				CustomFields.Clear();

				if (_model.ControlParent == null)
				{
					return;
				}				

				var customFields = new List<CustomField>();

				var serverTms = _tmsCollection.Where(s => s.IsServerTm && s.IsSelected).ToList();
				var fileBasedTms = _tmsCollection.Where(s => !s.IsServerTm && s.IsSelected).ToList();

				if (fileBasedTms.Any())
				{
					foreach (var tm in fileBasedTms)
					{						
						customFields.AddRange(_customFieldsService.GetFilebasedCustomField(ProgressDialog.Current, tm));
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

						customFields.AddRange(_customFieldsService.GetServerBasedCustomFields(ProgressDialog.Current, tm, translationProvider));
					}
				}

				foreach (var field in customFields)
				{
					AddCustomFieldValue(field);
				}
			
				if (CustomFields.Count > 0 && SelectedItem == null)
				{
					var selected = customFields.FirstOrDefault(
						a => a.Name.Equals(selectedItem.Name) && a.TmPath.Equals(selectedItem.TmPath));

					if (selected != null)
					{
						SelectedItem = selected;
					}
					else
					{
						SelectedItem = CustomFields[0];
					}
				}

				_model.Refresh();
			}
		}

		private void AddCustomFieldValue(CustomField field)
		{
			if (CustomFields.Contains(field))
			{
				return;
			}

			foreach (var customFieldValue in field.FieldValues)
			{
				customFieldValue.PropertyChanged += FieldValue_PropertyChanged;
			}

			CustomFields.Add(field);
		}

		private void RemoveCustomFieldValue(CustomField field)
		{
			foreach (var customFieldValue in field.FieldValues)
			{
				customFieldValue.PropertyChanged -= FieldValue_PropertyChanged;
			}

			CustomFields.Remove(field);
		}

		private void SelectFields()
		{
			var value = SelectAll;
			foreach (var fieldValue in CustomFieldsValues)
			{
				fieldValue.IsSelected = value;
			}
		}

		private static bool FieldValuesAreEqual(string existingFieldValue, string importedFieldValue)
		{
			if (existingFieldValue == null || importedFieldValue == null)
			{
				return !string.IsNullOrEmpty(existingFieldValue) || !string.IsNullOrEmpty(importedFieldValue);
			}

			return existingFieldValue != importedFieldValue;
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

			if (CustomFields != null)
			{
				foreach (var customField in CustomFields)
				{
					foreach (var customFieldValue in customField.FieldValues)
					{
						customFieldValue.PropertyChanged -= FieldValue_PropertyChanged;
					}
				}
			}
		}
	}
}