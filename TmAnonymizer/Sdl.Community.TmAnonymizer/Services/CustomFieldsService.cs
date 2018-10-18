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
		private readonly TmService _tmService;

		public CustomFieldsService(TmService tmService)
		{
			_tmService = tmService;
		}

		public List<CustomField> GetFilebasedCustomField(ProgressDialogContext context, TmFile tm)
		{
			var translationMemory = new FileBasedTranslationMemory(tm.Path);

			var tus = _tmService.LoadTranslationUnits(context, tm, null, new LanguageDirection
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

		public List<CustomField> GetServerBasedCustomFields(ProgressDialogContext context, TmFile tm, TranslationProviderServer translationProvideServer)
		{
			var translationMemory = translationProvideServer.GetTranslationMemory(tm.Path, TranslationMemoryProperties.All);

			var translationUnits = new List<TmTranslationUnit>();

			foreach (var languageDirection in translationMemory.LanguageDirections)
			{
				var tus = _tmService.LoadTranslationUnits(context, tm, translationProvideServer, new LanguageDirection
				{
					Source = languageDirection.SourceLanguage,
					Target = languageDirection.TargetLanguage
				});

				if (tus != null)
				{
					translationUnits.AddRange(tus);
				}
			}

			var customFieldList = GetCustomFieldList(translationMemory.FieldDefinitions, translationUnits, tm.Path);

			return customFieldList;
		}
		
		private IEnumerable<CustomFieldValue> GetNonPickListCustomFieldValues(IEnumerable<TmTranslationUnit> translationUnits, string name)
		{
			var customFieldValues = new List<CustomFieldValue>();
			var distinctFieldValues = new List<string>();
			foreach (var tu in translationUnits)
			{
				foreach (var fieldValue in tu.FieldValues)
				{
					if (fieldValue.Name.Equals(name))
					{
						var valueList = _tmService.GetMultipleStringValues(fieldValue.GetValueString(), fieldValue.ValueType);
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

		private List<CustomField> GetCustomFieldList(FieldDefinitionCollection fieldDefinitions, List<TmTranslationUnit> translationUnits, string tmFilePath)
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

		public void AnonymizeFileBasedCustomFields(ProgressDialogContext context, TmFile tmFile, List<CustomField> anonymizeFields)
		{
			var tm = new FileBasedTranslationMemory(tmFile.Path);

			var translationUnits = _tmService.LoadTranslationUnits(context, tmFile, null,
				new LanguageDirection
				{
					Source = tm.LanguageDirection.SourceLanguage,
					Target = tm.LanguageDirection.TargetLanguage
				});

			context?.Report(0, StringResources.Updating_Multiple_PickList_fields);

			foreach (var anonymizedField in anonymizeFields.Where(f => f.IsSelected))
			{
				if (anonymizedField.IsPickList)
				{
					foreach (var fieldValue in anonymizedField.FieldValues.Where(n => n.IsSelected && n.NewValue != null))
					{
						foreach (var fieldDefinition in tm.FieldDefinitions.Where(n => n.Name.Equals(anonymizedField.Name)))
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

			tm.Save();

			var units = GetUpdatableTranslationUnits(anonymizeFields, translationUnits);

			if (units.Count == 0)
			{
				return;
			}

			decimal iCurrent = 0;
			decimal iTotalUnits = translationUnits.Count;
			var groupsOf = 200;

			var tusGroups = new List<List<TmTranslationUnit>> { new List<TmTranslationUnit>(units) };
			if (units.Count > groupsOf)
			{
				tusGroups = translationUnits.ChunkBy(groupsOf);
			}

			foreach (var tus in tusGroups)
			{
				iCurrent = iCurrent + tus.Count;
				if (context != null && context.CheckCancellationPending())
				{
					break;
				}

				var progress = iCurrent / iTotalUnits * 100;
				context?.Report(Convert.ToInt32(progress), "Updating: " + iCurrent + " of " + iTotalUnits + " Translation Units");


				var tusToUpdate = new List<TranslationUnit>();
				foreach (var tu in tus)
				{
					if (tm.LanguageDirection.SourceLanguage.Name.Equals(tu.SourceSegment.Language) &&
						tm.LanguageDirection.TargetLanguage.Name.Equals(tu.TargetSegment.Language))
					{
						var unit = _tmService.CreateTranslationUnit(tu, tm.LanguageDirection);

						tusToUpdate.Add(unit);
					}
				}

				if (tusToUpdate.Count > 0)
				{
					//TODO - output results to log
					var results = tm.LanguageDirection.UpdateTranslationUnits(tusToUpdate.ToArray());
				}
			}			
		}

		public void AnonymizeServerBasedCustomFields(ProgressDialogContext context, TmFile tmFile, List<CustomField> anonymizeFields, TranslationProviderServer translationProvideServer)
		{
			var serverBasedTm = translationProvideServer.GetTranslationMemory(tmFile.Path, TranslationMemoryProperties.All);

			var languageDirections = new List<LanguageDirection>();
			foreach (var languageDirection in serverBasedTm.LanguageDirections)
			{
				languageDirections.Add(new LanguageDirection
				{
					Source = languageDirection.SourceLanguage,
					Target = languageDirection.TargetLanguage
				});
			}

			var translationUnits = _tmService.LoadTranslationUnits(context, tmFile, translationProvideServer, languageDirections);

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
			decimal iTotalUnits = translationUnits.Count;
			var groupsOf = 100;

			var tusGroups = new List<List<TmTranslationUnit>> { new List<TmTranslationUnit>(units) };
			if (units.Count > groupsOf)
			{
				tusGroups = translationUnits.ChunkBy(groupsOf);
			}

			foreach (var tus in tusGroups)
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
						if (languageDirection.SourceLanguage.Name.Equals(tu.SourceSegment.Language) &&
							languageDirection.TargetLanguage.Name.Equals(tu.TargetSegment.Language))
						{
							var unit = _tmService.CreateTranslationUnit(tu, languageDirection);
							tusToUpdate.Add(unit);
						}
					}

					if (tusToUpdate.Count > 0)
					{
						//TODO - output results to log
						var results = languageDirection.UpdateTranslationUnits(tusToUpdate.ToArray());
					}
				}
			}

			foreach (var languageDirection in tmFile.TmLanguageDirections)
			{
				_tmService.SaveTmCacheStorage(context, tmFile, languageDirection);
			}
		}

		private List<TmTranslationUnit> GetUpdatableTranslationUnits(List<CustomField> anonymizeFields, IEnumerable<TmTranslationUnit> translationUnits)
		{
			var units = new List<TmTranslationUnit>();

			foreach (var tu in translationUnits)
			{
				var update = false;
				foreach (var anonymizedField in anonymizeFields.Where(f => f.IsSelected))
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
								case FieldValueType.MultiplePicklist:
									update = true;
									UpdateMultiplePickListFieldValue(fieldValue, customFieldValue);
									break;
								case FieldValueType.SinglePicklist:
									update = true;
									UpdateSinglePicklistFieldValue(fieldValue, customFieldValue);
									break;
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

		private static void UpdateSinglePicklistFieldValue(Model.FieldDefinitions.FieldValue fieldValue, CustomFieldValue customFieldValue)
		{
			var picklistFieldValue = new Model.FieldDefinitions.SinglePicklistFieldValue();

			if (fieldValue is Model.FieldDefinitions.SinglePicklistFieldValue multiplePicklistFieldValue &&
				multiplePicklistFieldValue.Value.Name.Equals(customFieldValue.Value))
			{
				multiplePicklistFieldValue.Value.Name = customFieldValue.NewValue;

				picklistFieldValue.ValueType = FieldValueType.SinglePicklist;
				picklistFieldValue.Name = fieldValue.Name;
				picklistFieldValue.Value = new PicklistItem(multiplePicklistFieldValue.Value.Name)
				{
					ID = multiplePicklistFieldValue.Value.ID
				};

				fieldValue.Clear();
				fieldValue.Merge(picklistFieldValue);
			}
		}

		private static void UpdateMultiplePickListFieldValue(Model.FieldDefinitions.FieldValue fieldValue, CustomFieldValue customFieldValue)
		{
			var values = new List<PicklistItem>();
			if (fieldValue is Model.FieldDefinitions.MultiplePicklistFieldValue multiplePicklistFieldValue)
			{
				foreach (var picklistItem in multiplePicklistFieldValue.Values)
				{
					if (picklistItem.Name.Equals(customFieldValue.Value))
					{
						picklistItem.Name = customFieldValue.NewValue;
					}

					values.Add(new PicklistItem(picklistItem.Name) { ID = picklistItem.ID });
				}
			}

			var picklistFieldValue = new Model.FieldDefinitions.MultiplePicklistFieldValue
			{
				ValueType = FieldValueType.MultiplePicklist,
				Name = fieldValue.Name,
				Values = values
			};

			fieldValue.Clear();
			fieldValue.Merge(picklistFieldValue);
		}

		private void UpdateMultipleStringFieldValue(Model.FieldDefinitions.FieldValue fieldValue, CustomFieldValue customFieldValue)
		{
			var listString = _tmService.GetMultipleStringValues(fieldValue.GetValueString(), fieldValue.ValueType).ToList();
			if (!string.IsNullOrEmpty(customFieldValue.Value))
			{
				var index = listString.IndexOf(customFieldValue.Value);
				if (index > -1)
				{
					listString[index] = customFieldValue.NewValue;
				}
			}

			var multiStrngFieldValue = new Model.FieldDefinitions.MultipleStringFieldValue
			{
				Name = fieldValue.Name,
				Values = new HashSet<string>(listString),
				ValueType = FieldValueType.MultipleString
			};

			fieldValue.Clear();
			fieldValue.Add(multiStrngFieldValue);
		}

		private void UpdateSingleStringFieldValue(Model.FieldDefinitions.FieldValue fieldValue, CustomFieldValue customFieldValue)
		{
			var listString = _tmService.GetMultipleStringValues(fieldValue.GetValueString(), fieldValue.ValueType).ToList();

			if (!string.IsNullOrEmpty(customFieldValue.Value))
			{
				var index = listString.IndexOf(customFieldValue.Value);
				if (index > -1)
				{
					listString[index] = customFieldValue.NewValue;
				}
			}

			var singleStringFieldValue = new Model.FieldDefinitions.SingleStringFieldValue
			{
				Name = fieldValue.Name,
				Value = listString.First(),
				ValueType = FieldValueType.SingleString
			};

			fieldValue.Clear();
			fieldValue.Merge(singleStringFieldValue);
		}

		private void UpdateDateTimeFieldValue(Model.FieldDefinitions.FieldValue fieldValue, CustomFieldValue customFieldValue)
		{
			var listString = _tmService.GetMultipleStringValues(fieldValue.GetValueString(), fieldValue.ValueType).ToList();
			if (!string.IsNullOrEmpty(customFieldValue.Value))
			{
				var index = listString.IndexOf(customFieldValue.Value);
				if (index > -1)
				{
					listString[index] = customFieldValue.NewValue;
				}
			}
			var dateTimeFieldValue = new Model.FieldDefinitions.DateTimeFieldValue
			{
				Name = fieldValue.Name,
				Value = DateTime.Parse(listString.First()),
				ValueType = FieldValueType.DateTime
			};
			fieldValue.Clear();
			fieldValue.Add(dateTimeFieldValue);
		}

		private void UpdateIntFieldValue(Model.FieldDefinitions.FieldValue fieldValue, CustomFieldValue customFieldValue)
		{
			var listString = _tmService.GetMultipleStringValues(fieldValue.GetValueString(), fieldValue.ValueType).ToList();
			if (!string.IsNullOrEmpty(customFieldValue.Value))
			{
				var index = listString.IndexOf(customFieldValue.Value);
				if (index > -1)
				{
					listString[index] = customFieldValue.NewValue;
				}
			}
			var intFieldValue = new Model.FieldDefinitions.IntFieldValue
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
