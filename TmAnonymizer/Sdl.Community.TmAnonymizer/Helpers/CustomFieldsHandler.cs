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

		public static List<CustomField> GetFilebasedCustomField(TmFile tm)
		{
			var translationMemory = new FileBasedTranslationMemory(tm.Path);
			var unitsCount = translationMemory.LanguageDirection.GetTranslationUnitCount();
			var tmIterator = new RegularIterator(unitsCount);
			var tus = translationMemory.LanguageDirection.GetTranslationUnits(ref tmIterator);
			var customFieldList = GetCustomFieldList(translationMemory.FieldDefinitions, tus);

			return customFieldList;
		}

		public static List<CustomField> GetServerBasedCustomFields(TmFile tm, TranslationProviderServer translationProvideServer)
		{
			var translationMemory = translationProvideServer.GetTranslationMemory(tm.Path, TranslationMemoryProperties.All);
			var translationUnits = GetServerBasedTranslationUnits(translationMemory.LanguageDirections);
			var customFieldList = GetCustomFieldList(translationMemory.FieldDefinitions, translationUnits);

			return customFieldList;
		}

		private static List<Details> GetNonPickListCustomFieldValues(TranslationUnit[] translationUnits, string name)
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
				details.Add(new Details { Value = value });
			}
			return details;
		}

		private static List<Details> GetPickListCustomFieldValues(FieldDefinition fieldDefinition)
		{
			var details = new List<Details>();
			
			foreach (var pickListItem in fieldDefinition.PicklistItems)
			{
				var detailsItem = new Details
				{
					Value = pickListItem.Name
				};
				details.Add(detailsItem);
			}
			return details;
		}

		private static List<CustomField> GetCustomFieldList(FieldDefinitionCollection fieldDefinitions, TranslationUnit[] translationUnits)
		{
			var customFieldList = new List<CustomField>();

			foreach (var field in fieldDefinitions)
			{

				if (field.IsPicklist)
				{
					var customField = new CustomField
					{
						Id = field.Id,
						IsPickList = field.IsPicklist,
						Name = field.Name,
						ValueType = field.ValueType.ToString(),
						Details = new ObservableCollection<Details>(GetPickListCustomFieldValues(field))
					};
					customFieldList.Add(customField);
				}
				else
				{
					var customField = new CustomField
					{
						Id = field.Id,
						IsPickList = field.IsPicklist,
						Name = field.Name,
						ValueType = field.ValueType.ToString(),
						Details = new ObservableCollection<Details>(GetNonPickListCustomFieldValues(translationUnits, field.Name))
					};
					customFieldList.Add(customField);
				}

			}

			return customFieldList;
		}

		/// <summary>
		/// Retrieves an array of Translation Units for a Server Based Translation Memory
		/// </summary>
		/// <param name="languageDirections">Language Directions of a Server based Translation Memory</param>
		/// /// <returns>Array of TranslationUnits</returns>
		private static TranslationUnit[] GetServerBasedTranslationUnits(ServerBasedTranslationMemoryLanguageDirectionCollection languageDirections)
		{
			var translationUnits = new TranslationUnit[] { };

			foreach (var languageDirection in languageDirections)
			{
				var unitsCount = languageDirection.GetTranslationUnitCount();
				if (unitsCount == 0) continue;
				var tmIterator = new RegularIterator(unitsCount);
				translationUnits = languageDirection.GetTranslationUnits(ref tmIterator);
			}
			return translationUnits;
		}
	}
}
