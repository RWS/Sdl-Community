using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using Sdl.LanguagePlatform.TranslationMemoryApi;

namespace SDLTM.Import.Model
{
	public class TmDetails : BaseModel
	{
		private string _segmentIdCustomFieldName;
		private string _fileNameCustomField;
		private CustomFieldDetails _selectedSegmentField;
		private CustomFieldDetails _selectedFileNameField;
		private ImportSummary _importSummary;
		private bool _isImportSummaryVisible;
		private bool _customXliffFieldAlreadyExists;
		private bool _customFileNameFieldAlreadyExists;

		public string Name { get; set; }
		public string Path { get; set; }
		public Image SourceFlag { get; set; }
		public Image TargetFlag { get; set; }
		public string Id { get; set; }
		public FileBasedTranslationMemory TranslationMemory { get; set; }
		public CultureInfo SourceLanguage { get; set; }
		public CultureInfo TargetLanguage { get; set; }
		public FieldDefinitionCollection FieldsCollection { get; set; }
		public List<CustomFieldDetails> AvailableCustomFields { get; set; }

		public delegate void EditXliffCustomFiled(TmDetails tm);
		public event EditXliffCustomFiled EditXliffFieldEventRaised;
		public delegate void EditFileNameCustomField(TmDetails tm);
		public event EditFileNameCustomField EditFileNameFieldEventRaised;

		public ImportSummary ImportSummary
		{
			get => _importSummary;
			set
			{
				_importSummary = value;
				OnPropertyChanged(nameof(ImportSummary));
			}
		}

		public bool CustomXliffFieldAlreadyExists
		{
			get => _customXliffFieldAlreadyExists;
			set
			{
				if(_customXliffFieldAlreadyExists == value)return;
				_customXliffFieldAlreadyExists = value;
				OnPropertyChanged(nameof(CustomXliffFieldAlreadyExists));
			} 
		}

		public CustomFieldDetails SelectedSegmentField
		{
			get => _selectedSegmentField;
			set
			{
				_selectedSegmentField = value;
				OnPropertyChanged(nameof(SelectedSegmentField));
			}
		}

		public CustomFieldDetails SelectedFileNameField
		{
			get => _selectedFileNameField;
			set
			{
				_selectedFileNameField = value;
				OnPropertyChanged(nameof(SelectedFileNameField));
			}
		}

		public string SegmentIdCustomFieldName
		{
			get => _segmentIdCustomFieldName;
			set
			{
				if (_segmentIdCustomFieldName == value) return;
				_segmentIdCustomFieldName = value;

				CustomXliffFieldAlreadyExists =CheckIfFieldExists(_segmentIdCustomFieldName);
				EditXliffFieldEventRaised?.Invoke(this);
				if (string.IsNullOrEmpty(value) && AvailableCustomFields.Count > 0)
				{
					SelectedSegmentField = AvailableCustomFields[0];
				}
				else
				{
					SelectedSegmentField = null;
				}
				OnPropertyChanged(nameof(SegmentIdCustomFieldName));
			}
		}

		public bool CustomFileNameFieldAlreadyExists
		{
			get => _customFileNameFieldAlreadyExists;
			set
			{
				if (_customFileNameFieldAlreadyExists == value) return;
				_customFileNameFieldAlreadyExists = value;
				OnPropertyChanged(nameof(CustomFileNameFieldAlreadyExists));
			}
		}

		public string FileNameCustomField
		{
			get => _fileNameCustomField;
			set
			{
				if (_fileNameCustomField == value) return;
				_fileNameCustomField = value;
				CustomFileNameFieldAlreadyExists = CheckIfFieldExists(_fileNameCustomField);
				EditFileNameFieldEventRaised?.Invoke(this);
				if (string.IsNullOrEmpty(value) && AvailableCustomFields.Count > 0)
				{
					SelectedFileNameField = AvailableCustomFields[0];
				}
				else
				{
					SelectedFileNameField = null;
				}
				OnPropertyChanged(nameof(FileNameCustomField));
			}
		}

		public bool IsImportSummaryVisible
		{
			get => _isImportSummaryVisible;
			set
			{
				if (_isImportSummaryVisible == value)
				{
					return;
				}
				_isImportSummaryVisible = value;
				OnPropertyChanged(nameof(IsImportSummaryVisible));
			}
		}

		public void SetCustomFields()
		{
			AvailableCustomFields = new List<CustomFieldDetails>();
			foreach (var fieldCollection in FieldsCollection)
			{
				var filed = new CustomFieldDetails
				{
					Name = fieldCollection.Name
				};
				AvailableCustomFields.Add(filed);
			}
		}

		public string GetFileNameField()
		{
			var fileNameField = SelectedFileNameField != null
				? SelectedFileNameField.Name
				: FileNameCustomField;
			return fileNameField;
		}

		public string GetSegmentFieldName()
		{
			var segmentField = SelectedSegmentField != null
				? SelectedSegmentField.Name
				: SegmentIdCustomFieldName;
			return segmentField;
		}

		public bool HasCustomFiledsSelected()
		{
			var fileNameField = GetFileNameField();
			var segmentField = GetSegmentFieldName();
			return !string.IsNullOrEmpty(fileNameField) || !string.IsNullOrEmpty(segmentField);
		}
		private bool CheckIfFieldExists(string fieldValue)
		{
			return AvailableCustomFields.Any(c => c.Name.Equals(fieldValue));
		}
	}
}
