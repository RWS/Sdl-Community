using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
		private ObservableCollection<CustomField> _customFields = new ObservableCollection<CustomField>();


		public CustomFieldsViewModel(TranslationMemoryViewModel translationMemoryViewModel)
		{
			var tm =
			new FileBasedTranslationMemory(@"C:\Users\apascariu\Desktop\cy-en_(Fields_and_Attributes).sdltm");

			_customFields = new ObservableCollection<CustomField>(CustomFieldsHandler.GetCustomField(tm));

			//var unitsCount = tm.LanguageDirection.GetTranslationUnitCount();
			//var tmIterator = new RegularIterator(unitsCount);
			//var tus = tm.LanguageDirection.GetTranslationUnits(ref tmIterator);
			//var multipleString = new ObservableCollection<Details>();
			//var details = new ObservableCollection<Details>();
			//var detailItem = new Details();
			//foreach (var tu in tus)
			//{
			//	var values = tu.FieldValues.Values;
			//	foreach(var value in values)
			//	{
			//		if (value.ValueType == FieldValueType.DateTime /*|| value.ValueType == FieldValueType.Integer*/)
			//		{
			//			detailItem.Value = value.GetValueString();
			//			//details.Add(de)
			//			//var fieldValues = CustomFieldsHandler.GetMultipleStringValues(value.GetValueString());
			//		}
					
			//		//var details = new Details
			//		//{
			//		//	Value = value.v
			//		//}
			//	}
				
			//}

			//foreach (var fieldDefinition in tm.FieldDefinitions)
			//{
			//	var type = fieldDefinition.ValueType;
			//	_customFields.Add(new CustomField()
			//	{
			//		Name = fieldDefinition.Name,
			//		ValueType = type.ToString(),
			//		IsPickList = fieldDefinition.IsPicklist,
			//		Details = new ObservableCollection<Details>() {new Details
			//			{
			//				Value = "Another value",
			//				NewValue = ""
			//			} }
			//	});
			//}
			_translationMemoryViewModel = translationMemoryViewModel;
			//_customFields = new ObservableCollection<CustomField>
			//{
			//	new CustomField
			//	{
			//		Name = "Client",
			//		Type = "List",
			//		Details = new ObservableCollection<Details>
			//		{
			//			new Details
			//			{
			//				Value = "Pedigree",
			//				NewValue = ""
			//			},
			//			new Details
			//			{
			//				Value = "Another value",
			//				NewValue = ""
			//			}
			//		}
			//	},
			//	new CustomField
			//	{
			//		Name = "Client1",
			//		Type = "List",
			//		Details = new ObservableCollection<Details>
			//		{
			//			new Details
			//			{
			//				Value = "Pedigree",
			//				NewValue = ""
			//			},
			//			new Details
			//			{
			//				Value = "Another value",
			//				NewValue = ""
			//			},
			//			new Details
			//			{
			//				Value = "Another value",
			//				NewValue = ""
			//			}
			//		}
			//	}
			//};

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



