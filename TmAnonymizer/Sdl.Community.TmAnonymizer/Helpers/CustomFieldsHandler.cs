using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sdl.Community.SdlTmAnonymizer.Model;
using Sdl.Community.TmAnonymizer.Model;
using Sdl.LanguagePlatform.TranslationMemory;
using Sdl.LanguagePlatform.TranslationMemoryApi;

namespace Sdl.Community.SdlTmAnonymizer.Helpers
{
	public static class CustomFieldsHandler
	{
		private static  List<string> GetMultipleStringValues(string fieldValue)
		{
			var multipleStringValues = new List<string>();
			var trimStart = fieldValue.TrimStart('(');
			var trimEnd = trimStart.TrimEnd(')');
			var listValues = trimEnd.Split(',').ToList();
			foreach (var value in listValues)
			{
				var trimStartValue = value.TrimStart(' ','"');
				var trimEndValue = trimStartValue.TrimEnd('"');
				multipleStringValues.Add(trimEndValue);
			}
			return multipleStringValues;
		}

		public static List<CustomField> GetCustomField(FileBasedTranslationMemory fileBasedTranslationMemory)
		{
			var customFieldList = new List<CustomField>();
			

			var tm =
			new FileBasedTranslationMemory(@"C:\Users\apascariu\Desktop\cy-en_(Fields_and_Attributes).sdltm");
			var unitsCount = tm.LanguageDirection.GetTranslationUnitCount();
			var tmIterator = new RegularIterator(unitsCount);
			var tus = tm.LanguageDirection.GetTranslationUnits(ref tmIterator);

			foreach (var field in tm.FieldDefinitions)
			{
				
				if (field.IsPicklist)
				{
					var customField = new CustomField();
					customField.Id = field.Id;
					customField.IsPickList = field.IsPicklist;
					customField.Name = field.Name;
					customField.ValueType = field.ValueType.ToString();
					customField.Details = new ObservableCollection<Details>(GetPickListCustomFieldValues(field));
					customFieldList.Add(customField);
				}
				else
				{
					var customField = new CustomField();
					customField.Id = field.Id;
					customField.IsPickList = field.IsPicklist;
					customField.Name = field.Name;
					customField.ValueType = field.ValueType.ToString();
					customField.Details = new ObservableCollection<Details>(GetNonPickListCustomFieldValues(tus, field.Name));
					customFieldList.Add(customField);
				}
				
			}

			return customFieldList;
		}

		public static List<Details> GetNonPickListCustomFieldValues(TranslationUnit[] translationUnits, string name)
		{
			var details = new List<Details>();
			var detailValues = new List<string>();
			foreach (var tu in translationUnits)
			{
				
				foreach (var fieldValue in tu.FieldValues)
				{
					if (fieldValue.Name.Equals(name))
					{
						var valueList = GetMultipleStringValues(fieldValue.GetValueString());
						foreach (var value in valueList)
						{
							var detailsItem = new Details
							{
								Value = value
							};
							detailValues.Add(detailsItem.Value);
						}
					}

				}
			}

			var distinctList = detailValues.Distinct().ToList();
			foreach (var value in distinctList)
			{
				details.Add(new Details() { Value = value });
			}
			return details;
		}

		public static List<Details> GetPickListCustomFieldValues(FieldDefinition fieldDefinition)
		{
			var details = new List<Details>();
			
			foreach (var pickListItem in fieldDefinition.PicklistItems)
			{
				var detailsItem = new Details();
				detailsItem.Value = pickListItem.Name;
				details.Add(detailsItem);
			}
			return details;
		}
	}
}
