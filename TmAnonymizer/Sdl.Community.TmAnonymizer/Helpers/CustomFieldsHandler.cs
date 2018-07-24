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
		private static  List<string> GetMultipleStringValues(string fieldValue, FieldValueType fieldValueType)
		{
			var multipleStringValues = new List<string>();
			var trimStart = fieldValue.TrimStart('(');
			var trimEnd = trimStart.TrimEnd(')');
			var listValues = new List<string> { trimEnd };
			if (!fieldValueType.Equals(FieldValueType.DateTime))
			{
				listValues = trimEnd.Split(',').ToList();
			}
			
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
			var customFieldList = GetCustomFieldList(translationMemory.FieldDefinitions, tus,tm.Path);

			return customFieldList;
		}

		public static List<CustomField> GetServerBasedCustomFields(TmFile tm, TranslationProviderServer translationProvideServer)
		{
			var translationMemory = translationProvideServer.GetTranslationMemory(tm.Path, TranslationMemoryProperties.All);
			var translationUnits = GetServerBasedTranslationUnits(translationMemory.LanguageDirections);
			var customFieldList = GetCustomFieldList(translationMemory.FieldDefinitions, translationUnits,tm.Path);

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
						var valueList = GetMultipleStringValues(fieldValue.GetValueString(),fieldValue.ValueType);
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

		private static List<CustomField> GetCustomFieldList(FieldDefinitionCollection fieldDefinitions, TranslationUnit[] translationUnits,string tmFilePath)
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
						ValueType = field.ValueType,
						Details = new ObservableCollection<Details>(GetPickListCustomFieldValues(field)),
						TmPath = tmFilePath
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
						ValueType = field.ValueType,
						Details = new ObservableCollection<Details>(GetNonPickListCustomFieldValues(translationUnits, field.Name)),
						TmPath = tmFilePath
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

		public static void AnonymizeFileBasedCustomFields(TmFile tm, List<CustomField> anonymizeFields)
		{
			var fileBasedTm = new FileBasedTranslationMemory(tm.Path);
			var unitsCount = fileBasedTm.LanguageDirection.GetTranslationUnitCount();
			var tmIterator = new RegularIterator(unitsCount);
			var tus = fileBasedTm.LanguageDirection.GetTranslationUnits(ref tmIterator);
			
			
			foreach (var anonymizedField in anonymizeFields.Where(f => f.IsSelected))
			{
				if (anonymizedField.IsPickList)
				{
					foreach (var detail in anonymizedField.Details.Where(n => n.NewValue != null))
					{
						foreach (var fieldDefinition in fileBasedTm.FieldDefinitions.Where(n => n.Name.Equals(anonymizedField.Name)))
						{
							fieldDefinition.PicklistItems.Remove(detail.Value);
							fieldDefinition.PicklistItems.Add(detail.NewValue);
						}
					}
				}
				else
				{
					foreach (var tu in tus)
					{
						var newListFieldValues = new List<FieldValue>();
						foreach (var fieldValue in tu.FieldValues.Where(n => n.Name.Equals(anonymizedField.Name)))
						{
							foreach (var detail in anonymizedField.Details.Where(n => n.NewValue != null))
							{
								switch (fieldValue.ValueType)
								{
									case FieldValueType.SingleString:
										UpdateFileBasedSingleStringFieldValue(fileBasedTm, fieldValue, tu, detail);
										break;
									case FieldValueType.MultipleString:
										UpdateFileBasedMultipleStringFieldValue(fileBasedTm,fieldValue,tu,detail);
										break;
									case FieldValueType.DateTime:
										UpdateFileBasedDateTimeFieldValue(fileBasedTm, fieldValue, tu, detail);
										break;
									case FieldValueType.Integer:
										UpdateFileBasedIntFieldValue(fileBasedTm, fieldValue, tu, detail);
										break;
								}
							}
						}
						
					}
				}

			}
			fileBasedTm.Save();
		}

		public static void AnonymizeServerBasedCustomFields(TmFile tm, List<CustomField> anonymizeFields, TranslationProviderServer translationProvideServer)
		{
			var serverBasedTm = translationProvideServer.GetTranslationMemory(tm.Path, TranslationMemoryProperties.All);
			var translationUnits = GetServerBasedTranslationUnits(serverBasedTm.LanguageDirections);

			foreach (var anonymizedField in anonymizeFields.Where(f => f.IsSelected))
			{
				if (anonymizedField.IsPickList)
				{
					foreach (var detail in anonymizedField.Details.Where(n => n.NewValue != null))
					{
						foreach (var fieldDefinition in serverBasedTm.FieldDefinitions.Where(n => n.Name.Equals(anonymizedField.Name)))
						{
							fieldDefinition.PicklistItems.Remove(detail.Value);
							fieldDefinition.PicklistItems.Add(detail.NewValue);
						}
					}
				}
				else
				{
					foreach (var tu in translationUnits)
					{
						var newListFieldValues = new List<FieldValue>();
						foreach (var fieldValue in tu.FieldValues.Where(n => n.Name.Equals(anonymizedField.Name)))
						{
							foreach (var detail in anonymizedField.Details.Where(n => n.NewValue != null))
							{
								switch (fieldValue.ValueType)
								{
									case FieldValueType.SingleString:
										UpdateServerBasedSingleStringFieldValue(serverBasedTm, fieldValue, tu, detail);
										break;
									case FieldValueType.MultipleString:
										UpdateServerBasedMultipleStringFieldValue(serverBasedTm, fieldValue, tu, detail);
										break;
									case FieldValueType.DateTime:
										UpdateServerBasedDateTimeFieldValue(serverBasedTm, fieldValue, tu, detail);
										break;
									case FieldValueType.Integer:
										UpdateServerBasedIntFieldValue(serverBasedTm, fieldValue, tu, detail);
										break;
								}
							}
						}

					}
				}

			}
			serverBasedTm.Save();
		}

		private static void UpdateFileBasedMultipleStringFieldValue(FileBasedTranslationMemory fileBasedTm, FieldValue fieldValue, TranslationUnit tu, Details details)
		{
			var listString = GetMultipleStringValues(fieldValue.GetValueString(),fieldValue.ValueType).ToList();
			if (!string.IsNullOrEmpty(details.Value))
			{
				var index = listString.IndexOf(details.Value);
				if (index > -1)
				{
					listString[index] = details.NewValue;
				}
			}
			var multiStrngFieldValue = new MultipleStringFieldValue
			{
				Name = fieldValue.Name,
				Values = listString,
				ValueType = FieldValueType.MultipleString
			};
			fieldValue.Clear();
			fieldValue.Add(multiStrngFieldValue);
			fileBasedTm.LanguageDirection.UpdateTranslationUnit(tu);
		}

		private static void UpdateFileBasedSingleStringFieldValue(FileBasedTranslationMemory fileBasedTm, FieldValue fieldValue, TranslationUnit tu, Details details)
		{
			var listString = GetMultipleStringValues(fieldValue.GetValueString(), fieldValue.ValueType).ToList();
			if (!string.IsNullOrEmpty(details.Value))
			{
				var index = listString.IndexOf(details.Value);
				if (index > -1)
				{
					listString[index] = details.NewValue;
				}
			}
			var singleStringFieldValue = new SingleStringFieldValue
			{
				Name = fieldValue.Name,
				Value = listString.First(),
				ValueType = FieldValueType.SingleString
			};
			fieldValue.Clear();
			fieldValue.Merge(singleStringFieldValue);
			fileBasedTm.LanguageDirection.UpdateTranslationUnit(tu);
		}

		private static void UpdateFileBasedDateTimeFieldValue(FileBasedTranslationMemory fileBasedTm, FieldValue fieldValue, TranslationUnit tu, Details details)
		{
			var listString = GetMultipleStringValues(fieldValue.GetValueString(), fieldValue.ValueType).ToList();
			if (!string.IsNullOrEmpty(details.Value))
			{
				var index = listString.IndexOf(details.Value);
				if (index > -1)
				{
					listString[index] = details.NewValue;
				}
			}
			var dateTimeFieldValue = new DateTimeFieldValue
			{
				Name = fieldValue.Name,
				Value = DateTime.Parse(listString.First()),
				ValueType = FieldValueType.DateTime
			};
			fieldValue.Clear();
			fieldValue.Add(dateTimeFieldValue);
			fileBasedTm.LanguageDirection.UpdateTranslationUnit(tu);
		}

		private static void UpdateFileBasedIntFieldValue(FileBasedTranslationMemory fileBasedTm, FieldValue fieldValue, TranslationUnit tu, Details details)
		{
			var listString = GetMultipleStringValues(fieldValue.GetValueString(), fieldValue.ValueType).ToList();
			if (!string.IsNullOrEmpty(details.Value))
			{
				var index = listString.IndexOf(details.Value);
				if (index > -1)
				{
					listString[index] = details.NewValue;
				}
			}
			var intFieldValue = new IntFieldValue
			{
				Name = fieldValue.Name,
				Value = int.Parse(listString.First()),
				ValueType = FieldValueType.Integer
			};
			fieldValue.Clear();
			fieldValue.Merge(intFieldValue);
			fileBasedTm.LanguageDirection.UpdateTranslationUnit(tu);
		}

		//serve-based

		private static void UpdateServerBasedMultipleStringFieldValue(ServerBasedTranslationMemory serverBasedTm, FieldValue fieldValue, TranslationUnit tu, Details details)
		{
			var listString = GetMultipleStringValues(fieldValue.GetValueString(), fieldValue.ValueType).ToList();
			if (!string.IsNullOrEmpty(details.Value))
			{
				var index = listString.IndexOf(details.Value);
				if (index > -1)
				{
					listString[index] = details.NewValue;
				}
			}
			var multiStringFieldValue = new MultipleStringFieldValue
			{
				Name = fieldValue.Name,
				Values = listString,
				ValueType = FieldValueType.MultipleString
			};
			fieldValue.Clear();
			fieldValue.Add(multiStringFieldValue);
			foreach (var languageDirection in serverBasedTm.LanguageDirections)
			{
				languageDirection.UpdateTranslationUnit(tu);
			}
		}

		private static void UpdateServerBasedSingleStringFieldValue(ServerBasedTranslationMemory serverBasedTm, FieldValue fieldValue, TranslationUnit tu, Details details)
		{
			var listString = GetMultipleStringValues(fieldValue.GetValueString(), fieldValue.ValueType).ToList();
			if (!string.IsNullOrEmpty(details.Value))
			{
				var index = listString.IndexOf(details.Value);
				if (index > -1)
				{
					listString[index] = details.NewValue;
				}
			}
			var singleStringFieldValue = new SingleStringFieldValue
			{
				Name = fieldValue.Name,
				Value = listString.First(),
				ValueType = FieldValueType.SingleString
			};
			fieldValue.Clear();
			fieldValue.Merge(singleStringFieldValue);
			foreach (var languageDirection in serverBasedTm.LanguageDirections)
			{
				languageDirection.UpdateTranslationUnit(tu);
			}
		}

		private static void UpdateServerBasedDateTimeFieldValue(ServerBasedTranslationMemory serverBasedTm, FieldValue fieldValue, TranslationUnit tu, Details details)
		{
			var listString = GetMultipleStringValues(fieldValue.GetValueString(), fieldValue.ValueType).ToList();
			if (!string.IsNullOrEmpty(details.Value))
			{
				var index = listString.IndexOf(details.Value);
				if (index > -1)
				{
					listString[index] = details.NewValue;
				}
			}
			var dateTimeFieldValue = new DateTimeFieldValue
			{
				Name = fieldValue.Name,
				Value = DateTime.Parse(listString.First()),
				ValueType = FieldValueType.DateTime
			};
			fieldValue.Clear();
			fieldValue.Add(dateTimeFieldValue);
			foreach (var languageDirection in serverBasedTm.LanguageDirections)
			{
				languageDirection.UpdateTranslationUnit(tu);
			}
		}

		private static void UpdateServerBasedIntFieldValue(ServerBasedTranslationMemory serverBasedTm, FieldValue fieldValue, TranslationUnit tu, Details details)
		{
			var listString = GetMultipleStringValues(fieldValue.GetValueString(), fieldValue.ValueType).ToList();
			if (!string.IsNullOrEmpty(details.Value))
			{
				var index = listString.IndexOf(details.Value);
				if (index > -1)
				{
					listString[index] = details.NewValue;
				}
			}
			var intFieldValue = new IntFieldValue
			{
				Name = fieldValue.Name,
				Value = int.Parse(listString.First()),
				ValueType = FieldValueType.Integer
			};
			fieldValue.Clear();
			fieldValue.Merge(intFieldValue);
			foreach (var languageDirection in serverBasedTm.LanguageDirections)
			{
				languageDirection.UpdateTranslationUnit(tu);
			}
		}
	}
}
