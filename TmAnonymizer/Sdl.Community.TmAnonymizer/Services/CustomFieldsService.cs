using System;
using System.Collections.Generic;
using System.Linq;
using Sdl.Community.SdlTmAnonymizer.Controls.ProgressDialog;
using Sdl.Community.SdlTmAnonymizer.Extensions;
using Sdl.Community.SdlTmAnonymizer.Model;
using Sdl.LanguagePlatform.TranslationMemory;
using Sdl.LanguagePlatform.TranslationMemoryApi;

namespace Sdl.Community.SdlTmAnonymizer.Services
{
	public class CustomFieldsService
	{
		public List<CustomField> GetFilebasedCustomField(ProgressDialogContext context, TmFile tm, TmService tmService)
		{
			var translationMemory = new FileBasedTranslationMemory(tm.Path);

			var tus = tmService.LoadTranslationUnits(context, tm, null, new LanguageDirection
			{
				Source = translationMemory.LanguageDirection.SourceLanguage,
				Target = translationMemory.LanguageDirection.TargetLanguage
			});

			if (tus != null)
			{
				var customFieldList = GetCustomFieldList(translationMemory.FieldDefinitions, tus, tm.Path);
				return customFieldList;
			}

			return null;
		}

		public List<CustomField> GetServerBasedCustomFields(ProgressDialogContext context, TmFile tm, TranslationProviderServer translationProvideServer, TmService tmService)
		{
			var translationMemory = translationProvideServer.GetTranslationMemory(tm.Path, TranslationMemoryProperties.All);

			var translationUnits = new List<TranslationUnit>();

			foreach (var languageDirection in translationMemory.LanguageDirections)
			{
				var tus = tmService.LoadTranslationUnits(context, tm, translationProvideServer, new LanguageDirection
				{
					Source = languageDirection.SourceLanguage,
					Target = languageDirection.TargetLanguage
				});

				if (tus != null)
				{
					translationUnits.AddRange(tus);
				}
			}

			var customFieldList = GetCustomFieldList(translationMemory.FieldDefinitions, translationUnits.ToArray(), tm.Path);

			return customFieldList;
		}

		private static IEnumerable<string> GetMultipleStringValues(string fieldValue, FieldValueType fieldValueType)
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
				var trimStartValue = value.TrimStart(' ', '"');
				var trimEndValue = trimStartValue.TrimEnd('"');
				multipleStringValues.Add(trimEndValue);
			}

			return multipleStringValues;
		}

		private static IEnumerable<CustomFieldValue> GetNonPickListCustomFieldValues(IEnumerable<TranslationUnit> translationUnits, string name)
		{
			var customFieldValues = new List<CustomFieldValue>();
			var distinctFieldValues = new List<string>();
			foreach (var tu in translationUnits)
			{
				foreach (var fieldValue in tu.FieldValues)
				{
					if (fieldValue.Name.Equals(name))
					{
						var valueList = GetMultipleStringValues(fieldValue.GetValueString(), fieldValue.ValueType);
						foreach (var value in valueList)
						{
							var detailsItem = new CustomFieldValue
							{
								Value = value
							};

							distinctFieldValues.Add(detailsItem.Value);
						}
					}
				}
			}

			var distinctList = distinctFieldValues.Distinct().ToList();
			foreach (var value in distinctList)
			{
				customFieldValues.Add(new CustomFieldValue { Value = value });
			}
			return customFieldValues;
		}

		private static IEnumerable<CustomFieldValue> GetPickListCustomFieldValues(FieldDefinition fieldDefinition)
		{
			var details = new List<CustomFieldValue>();

			foreach (var pickListItem in fieldDefinition.PicklistItems)
			{
				var detailsItem = new CustomFieldValue
				{
					Value = pickListItem.Name
				};

				details.Add(detailsItem);
			}
			return details;
		}

		private static List<CustomField> GetCustomFieldList(FieldDefinitionCollection fieldDefinitions, TranslationUnit[] translationUnits, string tmFilePath)
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
						FieldValues = new List<CustomFieldValue>(GetPickListCustomFieldValues(field)),
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
						FieldValues = new List<CustomFieldValue>(GetNonPickListCustomFieldValues(translationUnits, field.Name)),
						TmPath = tmFilePath
					};
					customFieldList.Add(customField);
				}

			}

			return customFieldList;
		}

		public void AnonymizeFileBasedCustomFields(ProgressDialogContext context, TmFile tmFile, List<CustomField> anonymizeFields, TmService tmService)
		{
			var fileBasedTm = new FileBasedTranslationMemory(tmFile.Path);

			var translationUnits = tmService.LoadTranslationUnits(context, tmFile, null,
				new LanguageDirection
				{
					Source = fileBasedTm.LanguageDirection.SourceLanguage,
					Target = fileBasedTm.LanguageDirection.TargetLanguage
				});

			context?.Report(0, StringResources.Updating_Multiple_PickList_fields);

			foreach (var anonymizedField in anonymizeFields.Where(f => f.IsSelected))
			{
				if (anonymizedField.IsPickList)
				{
					foreach (var fieldValue in anonymizedField.FieldValues.Where(n => n.IsSelected && n.NewValue != null))
					{
						foreach (var fieldDefinition in fileBasedTm.FieldDefinitions.Where(n => n.Name.Equals(anonymizedField.Name)))
						{
							var pickListItem = fieldDefinition.PicklistItems.FirstOrDefault(a => a.Name.Equals(fieldValue.Value));
							if (pickListItem != null)
							{
								pickListItem.Name = fieldValue.NewValue;
							}
						}
					}
				}
			}

			fileBasedTm.Save();

			var units = GetUpdatableTranslationUnits(anonymizeFields, translationUnits);

			if (units.Count == 0)
			{
				return;
			}

			decimal iCurrent = 0;
			decimal iTotalUnits = translationUnits.Length;
			var groupsOf = 200;

			var unitGroups = new List<List<TranslationUnit>> { new List<TranslationUnit>(units) };
			if (units.Count > groupsOf)
			{
				unitGroups = translationUnits.ToList().ChunkBy(groupsOf);
			}

			foreach (var tus in unitGroups)
			{
				iCurrent = iCurrent + tus.Count;
				if (context != null && context.CheckCancellationPending())
				{
					break;
				}

				var progress = iCurrent / iTotalUnits * 100;
				context?.Report(Convert.ToInt32(progress), "Updating: " + iCurrent + " of " + iTotalUnits + " Translation Units");

				var results = fileBasedTm.LanguageDirection.UpdateTranslationUnits(tus.ToArray());
			}

		}

		public void AnonymizeServerBasedCustomFields(ProgressDialogContext context, TmFile tm, List<CustomField> anonymizeFields, TranslationProviderServer translationProvideServer, TmService tmService)
		{
			var serverBasedTm = translationProvideServer.GetTranslationMemory(tm.Path, TranslationMemoryProperties.All);

			var languageDirections = new List<LanguageDirection>();
			foreach (var languageDirection in serverBasedTm.LanguageDirections)
			{
				languageDirections.Add(new LanguageDirection
				{
					Source = languageDirection.SourceLanguage,
					Target = languageDirection.TargetLanguage
				});
			}

			var translationUnits = tmService.LoadTranslationUnits(context, tm, translationProvideServer, languageDirections);

			context?.Report(0, StringResources.Updating_Multiple_PickList_fields);

			foreach (var anonymizedField in anonymizeFields.Where(f => f.IsSelected))
			{
				if (anonymizedField.IsPickList)
				{
					foreach (var fieldValue in anonymizedField.FieldValues.Where(n => n.IsSelected && n.NewValue != null))
					{
						foreach (var fieldDefinition in serverBasedTm.FieldDefinitions.Where(n => n.Name.Equals(anonymizedField.Name)))
						{
							var pickListItem = fieldDefinition.PicklistItems.FirstOrDefault(a => a.Name.Equals(fieldValue.Value));
							if (pickListItem != null)
							{
								pickListItem.Name = fieldValue.NewValue;
							}
						}
					}
				}
			}

			serverBasedTm.Save();

			var units = GetUpdatableTranslationUnits(anonymizeFields, translationUnits);

			if (units.Count == 0)
			{
				return;
			}

			decimal iCurrent = 0;
			decimal iTotalUnits = translationUnits.Length;
			var groupsOf = 100;

			var unitGroups = new List<List<TranslationUnit>> { new List<TranslationUnit>(units) };
			if (units.Count > groupsOf)
			{
				unitGroups = translationUnits.ToList().ChunkBy(groupsOf);
			}

			foreach (var tus in unitGroups)
			{
				iCurrent = iCurrent + tus.Count;
				if (context != null && context.CheckCancellationPending())
				{
					break;
				}

				var progress = iCurrent / iTotalUnits * 100;
				context?.Report(Convert.ToInt32(progress), "Updating: " + iCurrent + " of " + iTotalUnits + " Translation Units");

				foreach (var languageDirection in serverBasedTm.LanguageDirections)
				{
					var tusToUpdate = new List<TranslationUnit>();
					foreach (var tu in tus)
					{
						if (languageDirection.SourceLanguage.Equals(tu.SourceSegment.Culture) &&
							languageDirection.TargetLanguage.Equals(tu.TargetSegment.Culture))
						{
							tusToUpdate.Add(tu);
						}
					}

					if (tusToUpdate.Count > 0)
					{
						var results = languageDirection.UpdateTranslationUnits(tusToUpdate.ToArray());
					}
				}
			}
		}

		private static List<TranslationUnit> GetUpdatableTranslationUnits(List<CustomField> anonymizeFields, IEnumerable<TranslationUnit> translationUnits)
		{
			var units = new List<TranslationUnit>();

			foreach (var tu in translationUnits)
			{
				var update = false;
				foreach (var anonymizedField in anonymizeFields.Where(f => f.IsSelected))
				{
					if (!anonymizedField.IsPickList)
					{
						foreach (var fieldValue in tu.FieldValues.Where(n => n.Name.Equals(anonymizedField.Name)))
						{
							foreach (var customFieldValue in anonymizedField.FieldValues.Where(n => n.IsSelected && n.NewValue != null))
							{
								switch (fieldValue.ValueType)
								{
									case FieldValueType.SingleString:
										update = true;
										UpdateSingleStringFieldValue(fieldValue, customFieldValue);
										break;
									case FieldValueType.MultipleString:
										update = true;
										UpdateMultipleStringFieldValue(fieldValue, customFieldValue);
										break;
									case FieldValueType.DateTime:
										update = true;
										UpdateDateTimeFieldValue(fieldValue, customFieldValue);
										break;
									case FieldValueType.Integer:
										update = true;
										UpdateIntFieldValue(fieldValue, customFieldValue);
										break;
								}
							}
						}
					}
				}

				if (update)
				{
					units.Add(tu);
				}
			}

			return units;
		}

		private static void UpdateMultipleStringFieldValue(FieldValue fieldValue, CustomFieldValue customFieldValue)
		{
			var listString = GetMultipleStringValues(fieldValue.GetValueString(), fieldValue.ValueType).ToList();
			if (!string.IsNullOrEmpty(customFieldValue.Value))
			{
				var index = listString.IndexOf(customFieldValue.Value);
				if (index > -1)
				{
					listString[index] = customFieldValue.NewValue;
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
		}

		private static void UpdateSingleStringFieldValue(FieldValue fieldValue, CustomFieldValue customFieldValue)
		{
			var listString = GetMultipleStringValues(fieldValue.GetValueString(), fieldValue.ValueType).ToList();

			if (!string.IsNullOrEmpty(customFieldValue.Value))
			{
				var index = listString.IndexOf(customFieldValue.Value);
				if (index > -1)
				{
					listString[index] = customFieldValue.NewValue;
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
		}

		private static void UpdateDateTimeFieldValue(FieldValue fieldValue, CustomFieldValue customFieldValue)
		{
			var listString = GetMultipleStringValues(fieldValue.GetValueString(), fieldValue.ValueType).ToList();
			if (!string.IsNullOrEmpty(customFieldValue.Value))
			{
				var index = listString.IndexOf(customFieldValue.Value);
				if (index > -1)
				{
					listString[index] = customFieldValue.NewValue;
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
		}

		private static void UpdateIntFieldValue(FieldValue fieldValue, CustomFieldValue customFieldValue)
		{
			var listString = GetMultipleStringValues(fieldValue.GetValueString(), fieldValue.ValueType).ToList();
			if (!string.IsNullOrEmpty(customFieldValue.Value))
			{
				var index = listString.IndexOf(customFieldValue.Value);
				if (index > -1)
				{
					listString[index] = customFieldValue.NewValue;
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
		}
	}
}
