using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;
using Sdl.Community.SdlTmAnonymizer.Helpers;
using Sdl.Community.SdlTmAnonymizer.Model;
using Sdl.Community.SdlTmAnonymizer.ViewModel;
using Sdl.Community.TmAnonymizer.Model;
using Sdl.LanguagePlatform.TranslationMemory;
using Sdl.LanguagePlatform.TranslationMemoryApi;

namespace Sdl.Community.TmAnonymizer.ViewModel
{
	public class CustomFieldsViewModel : ViewModelBase
	{
		private static TranslationMemoryViewModel _translationMemoryViewModel;
		private bool _selectAll;
		private ObservableCollection<CustomField> _customFields;
		private ICommand _selectAllCommand;
		private ICommand _applyCommand;
		private ICommand _importCommand;
		private ICommand _exportCommand;
		private IList _selectedItems;

		public CustomFieldsViewModel(TranslationMemoryViewModel translationMemoryViewModel)
		{
			_selectedItems = new List<CustomField>();
			//var tm =
			//new FileBasedTranslationMemory(@"C:\Users\aghisa\Desktop\cy-en_(Fields_and_Attributes).sdltm");

			//_customFields = new ObservableCollection<CustomField>(CustomFieldsHandler.GetCustomField(tm));
			_translationMemoryViewModel = translationMemoryViewModel;

		}

		public ICommand SelectAllCommand => _selectAllCommand ?? (_selectAllCommand = new CommandHandler(SelectFields, true));
		public ICommand ImportCommand => _importCommand ?? (_importCommand = new CommandHandler(Import, true));
		public ICommand ApplyCommand => _applyCommand ?? (_applyCommand = new CommandHandler(ApplyChanges, true));
		public ICommand ExportCommand => _exportCommand ?? (_exportCommand = new CommandHandler(Export, true));

		private void Export()
		{
			if (SelectedItems.Count > 0)
			{
				var selectedFields = new List<CustomField>();
				var fileDialog = new SaveFileDialog
				{
					Title = @"Export selected custom fields",
					Filter = @"Excel |*.xlsx"
				};
				var result = fileDialog.ShowDialog();
				if (result == DialogResult.OK && fileDialog.FileName != string.Empty)
				{
					foreach (CustomField field in SelectedItems)
					{
						selectedFields.Add(field);
					}
					//Expressions.ExportExporessions(fileDialog.FileName, selectedRules);
					MessageBox.Show(@"File was exported successfully to selected location", "", MessageBoxButtons.OK,
						MessageBoxIcon.Information);
				}
			}
			else
			{
				MessageBox.Show(@"Please select at least one row to export", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
			}
		}

		private void ApplyChanges()
		{
			
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
	}
}



