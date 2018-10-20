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
		private readonly SettingsService _settingsService;

		public CustomFieldsService(TmService tmService, SettingsService settingsService)
		{
			_tmService = tmService;
			_settingsService = settingsService;
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

		public void AnonymizeFileBasedCustomFields(ProgressDialogContext context, TmFile tmFile, List<CustomField> anonymizeFields)
		{
			var tm = new FileBasedTranslationMemory(tmFile.Path);

			var translationUnits = _tmService.LoadTranslationUnits(context, tmFile, null,
				new LanguageDirection
				{
					Source = tm.LanguageDirection.SourceLanguage,
					Target = tm.LanguageDirection.TargetLanguage
				});

			UpdateCustomFieldPickLists(context, anonymizeFields, tm);

			var units = GetUpdatableTranslationUnits(anonymizeFields, translationUnits);

			if (units.Count == 0)
			{
				return;
			}

			var settings = _settingsService.GetSettings();
			if (settings.UseSqliteApiForFileBasedTm)
			{
				UpdateCustomFieldsSqlite(context, tmFile, units);
			}
			else
			{
				UpdateCustomFields(context, translationUnits, units, tm);
			}

			ClearPreviousCustomFieldValues(translationUnits);
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

			UpdateCustomFieldPickLists(context, anonymizeFields, serverBasedTm);

			var units = GetUpdatableTranslationUnits(anonymizeFields, translationUnits);

			if (units.Count == 0)
			{
				return;
			}

			UpdateCustomFields(context, tmFile, translationUnits, units, serverBasedTm);

			ClearPreviousCustomFieldValues(translationUnits);
		}

		private static void UpdateCustomFieldsSqlite(ProgressDialogContext context, TmFile tmFile, List<TmTranslationUnit> units)
		{
			if (units.Count == 0)
			{
				return;
			}

			var service = new SqliteTmService(tmFile.Path, null, new SerializerService(), new SegmentService());

			try
			{
				service.OpenConnection();

				service.UpdateCustomFields(context, units);
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
				throw;
			}
			finally
			{
				service.CloseConnection();
			}
		}

		private static void UpdateCustomFieldPickLists(ProgressDialogContext context, IEnumerable<CustomField> anonymizeFields, ITranslationMemory tm)
		{
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
		}

		private void UpdateCustomFields(ProgressDialogContext context, List<TmTranslationUnit> translationUnits, IReadOnlyCollection<TmTranslationUnit> units, ILocalTranslationMemory tm)
		{
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

		private void UpdateCustomFields(ProgressDialogContext context, TmFile tmFile, List<TmTranslationUnit> translationUnits,
			IReadOnlyCollection<TmTranslationUnit> units, ServerBasedTranslationMemory serverBasedTm)
		{
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
				var updatedFieldValues = false;
				foreach (var anonymizedField in anonymizeFields.Where(f => f.IsSelected))
				{
					foreach (var fieldValue in tu.FieldValues.Where(n => n.Name.Equals(anonymizedField.Name)))
					{
						var updatedFieldValue = false;
						foreach (var customFieldValue in anonymizedField.FieldValues.Where(
							n => n.IsSelected && n.NewValue != null && n.Value != n.NewValue))
						{
							switch (fieldValue.ValueType)
							{
								case FieldValueType.SingleString:
									if (UpdateSingleStringFieldValue(fieldValue, customFieldValue))
									{
										updatedFieldValue = true;
									}
									break;
								case FieldValueType.MultipleString:
									if (UpdateMultipleStringFieldValue(fieldValue, customFieldValue))
									{
										updatedFieldValue = true;
									}
									break;
								case FieldValueType.DateTime:
									if (UpdateDateTimeFieldValue(fieldValue, customFieldValue))
									{
										updatedFieldValue = true;
									}
									break;
								case FieldValueType.Integer:
									if (UpdateIntFieldValue(fieldValue, customFieldValue))
									{
										updatedFieldValue = true;
									}
									break;
								case FieldValueType.MultiplePicklist:
									if (UpdateMultiplePickListFieldValue(fieldValue, customFieldValue))
									{
										updatedFieldValue = true;
									}
									break;
								case FieldValueType.SinglePicklist:
									if (UpdateSinglePicklistFieldValue(fieldValue, customFieldValue))
									{
										updatedFieldValue = true;
									}
									break;
							}
						}

						if (updatedFieldValue)
						{
							updatedFieldValues = true;
						}
					}
				}

				if (updatedFieldValues)
				{
					units.Add(tu);
				}
			}

			return units;
		}

		private static bool UpdateSinglePicklistFieldValue(Model.FieldDefinitions.FieldValue fieldValue, CustomFieldValue customFieldValue)
		{
			if (!(fieldValue is Model.FieldDefinitions.SinglePicklistFieldValue singlePicklistFieldValue))
			{
				return false;
			}

			if (!singlePicklistFieldValue.Value.Name.Equals(customFieldValue.Value))
			{
				return false;
			}

			singlePicklistFieldValue.PreviousValue = new Model.FieldDefinitions.PicklistItem
			{
				Name = singlePicklistFieldValue.Value.Name,
				ID = singlePicklistFieldValue.Value.ID,
				PreviousName = singlePicklistFieldValue.Value.Name
			};

			singlePicklistFieldValue.Value.PreviousName = singlePicklistFieldValue.Value.Name;
			singlePicklistFieldValue.Value.Name = customFieldValue.NewValue;

			var picklistFieldValue = new Model.FieldDefinitions.SinglePicklistFieldValue
			{
				ValueType = FieldValueType.SinglePicklist,
				Name = fieldValue.Name,
				Value = new Model.FieldDefinitions.PicklistItem(singlePicklistFieldValue.Value.Name)
				{
					ID = singlePicklistFieldValue.Value.ID,
					PreviousName = singlePicklistFieldValue.Value.PreviousName
				}
			};

			fieldValue.Clear();
			fieldValue.Merge(picklistFieldValue);

			return true;
		}

		private static bool UpdateMultiplePickListFieldValue(Model.FieldDefinitions.FieldValue fieldValue, CustomFieldValue customFieldValue)
		{
			if (!(fieldValue is Model.FieldDefinitions.MultiplePicklistFieldValue multiplePicklistFieldValue))
			{
				return false;
			}

			multiplePicklistFieldValue.PreviousValues = new List<Model.FieldDefinitions.PicklistItem>();
			foreach (var picklistItem in multiplePicklistFieldValue.Values)
			{
				var pickListItemClone = new Model.FieldDefinitions.PicklistItem(picklistItem.Name)
				{
					ID = picklistItem.ID,
					PreviousName = picklistItem.Name
				};
				multiplePicklistFieldValue.PreviousValues.Add(pickListItemClone);
			}

			var updated = false;
			var values = new List<Model.FieldDefinitions.PicklistItem>();
			foreach (var picklistItem in multiplePicklistFieldValue.Values)
			{
				picklistItem.PreviousName = picklistItem.Name;
				if (picklistItem.Name.Equals(customFieldValue.Value) && customFieldValue.NewValue != null)
				{
					updated = true;
					picklistItem.Name = customFieldValue.NewValue;
				}

				values.Add(new Model.FieldDefinitions.PicklistItem(picklistItem.Name)
				{
					ID = picklistItem.ID,
					PreviousName = picklistItem.PreviousName
				});
			}

			var picklistFieldValue = new Model.FieldDefinitions.MultiplePicklistFieldValue
			{
				ValueType = FieldValueType.MultiplePicklist,
				Name = fieldValue.Name,
				Values = values
			};

			fieldValue.Clear();
			fieldValue.Merge(picklistFieldValue);

			return updated;
		}

		private bool UpdateMultipleStringFieldValue(Model.FieldDefinitions.FieldValue fieldValue, CustomFieldValue customFieldValue)
		{
			if (!(fieldValue is Model.FieldDefinitions.MultipleStringFieldValue multipleStringFieldValue))
			{
				return false;
			}

			if (multipleStringFieldValue.PreviousValues == null)
			{
				multipleStringFieldValue.PreviousValues = new HashSet<string>();
				foreach (var value in multipleStringFieldValue.Values)
				{
					multipleStringFieldValue.PreviousValues.Add(value.Clone().ToString());
				}
			}

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

			var valueString = ValuesToString(multipleStringFieldValue.Values);
			var previousValueString = ValuesToString(multipleStringFieldValue.PreviousValues);

			return previousValueString != valueString;
		}		

		private bool UpdateSingleStringFieldValue(Model.FieldDefinitions.FieldValue fieldValue, CustomFieldValue customFieldValue)
		{
			if (!(fieldValue is Model.FieldDefinitions.SingleStringFieldValue singleStringFieldValue))
			{
				return false;
			}

			if (singleStringFieldValue.Value != customFieldValue.Value)
			{
				return false;
			}

			singleStringFieldValue.PreviousValue = singleStringFieldValue.Value;

			var listString = _tmService.GetMultipleStringValues(fieldValue.GetValueString(), fieldValue.ValueType).ToList();

			if (!string.IsNullOrEmpty(customFieldValue.Value))
			{
				var index = listString.IndexOf(customFieldValue.Value);
				if (index > -1)
				{
					listString[index] = customFieldValue.NewValue;
				}
			}

			var newSingleStringFieldValue = new Model.FieldDefinitions.SingleStringFieldValue
			{
				Name = fieldValue.Name,
				Value = listString.First(),
				ValueType = FieldValueType.SingleString
			};

			fieldValue.Clear();
			fieldValue.Merge(newSingleStringFieldValue);

			return singleStringFieldValue.PreviousValue != singleStringFieldValue.Value;
		}

		private bool UpdateDateTimeFieldValue(Model.FieldDefinitions.FieldValue fieldValue, CustomFieldValue customFieldValue)
		{
			if (!(fieldValue is Model.FieldDefinitions.DateTimeFieldValue dateTimeFieldValue))
			{
				return false;
			}

			dateTimeFieldValue.PreviousValue = dateTimeFieldValue.Value;

			var listString = _tmService.GetMultipleStringValues(fieldValue.GetValueString(), fieldValue.ValueType).ToList();
			if (!string.IsNullOrEmpty(customFieldValue.Value))
			{
				var index = listString.IndexOf(customFieldValue.Value);
				if (index > -1)
				{
					listString[index] = customFieldValue.NewValue;
				}
			}
			var newDateTimeFieldValue = new Model.FieldDefinitions.DateTimeFieldValue
			{
				Name = fieldValue.Name,
				Value = DateTime.Parse(listString.First()),
				ValueType = FieldValueType.DateTime
			};
			fieldValue.Clear();
			fieldValue.Add(newDateTimeFieldValue);

			return dateTimeFieldValue.PreviousValue != dateTimeFieldValue.Value;
		}

		private bool UpdateIntFieldValue(Model.FieldDefinitions.FieldValue fieldValue, CustomFieldValue customFieldValue)
		{
			if (!(fieldValue is Model.FieldDefinitions.IntFieldValue intFieldValue))
			{
				return false;
			}

			if (intFieldValue.Value.ToString() != customFieldValue.Value)
			{
				return false;
			}

			intFieldValue.PreviousValue = intFieldValue.Value;

			var listString = _tmService.GetMultipleStringValues(fieldValue.GetValueString(), fieldValue.ValueType).ToList();
			if (!string.IsNullOrEmpty(customFieldValue.Value))
			{
				var index = listString.IndexOf(customFieldValue.Value);
				if (index > -1)
				{
					listString[index] = customFieldValue.NewValue;
				}
			}
			var newIntFieldValue = new Model.FieldDefinitions.IntFieldValue
			{
				Name = fieldValue.Name,
				Value = int.Parse(listString.First()),
				ValueType = FieldValueType.Integer
			};

			fieldValue.Clear();
			fieldValue.Merge(newIntFieldValue);

			return intFieldValue.PreviousValue != intFieldValue.Value;
		}

		private static string ValuesToString(IEnumerable<string> values)
		{
			return values.Aggregate(string.Empty, (current, value) => current + (!string.IsNullOrEmpty(current) ? ", " : string.Empty) + value);
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

		private static void ClearPreviousCustomFieldValues(IEnumerable<TmTranslationUnit> translationUnits)
		{
			foreach (var unit in translationUnits)
			{
				foreach (var fieldValue in unit.FieldValues)
				{
					switch (fieldValue.ValueType)
					{
						case FieldValueType.SingleString:
							if (fieldValue is Model.FieldDefinitions.SingleStringFieldValue singleStringFieldValue)
							{
								singleStringFieldValue.PreviousValue = null;
							}
							break;
						case FieldValueType.MultipleString:
							if (fieldValue is Model.FieldDefinitions.MultipleStringFieldValue multipleStringFieldValue)
							{
								multipleStringFieldValue.PreviousValues = null;
							}
							break;
						case FieldValueType.DateTime:
							if (fieldValue is Model.FieldDefinitions.DateTimeFieldValue dateTimeFieldValue)
							{
								dateTimeFieldValue.PreviousValue = null;
							}
							break;
						case FieldValueType.Integer:
							if (fieldValue is Model.FieldDefinitions.IntFieldValue intFieldValue)
							{
								intFieldValue.PreviousValue = null;
							}
							break;
						case FieldValueType.MultiplePicklist:
							if (fieldValue is Model.FieldDefinitions.MultiplePicklistFieldValue multiplePicklistFieldValue)
							{
								multiplePicklistFieldValue.PreviousValues = null;
							}
							break;
						case FieldValueType.SinglePicklist:
							if (fieldValue is Model.FieldDefinitions.SinglePicklistFieldValue singlePicklistFieldValue)
							{
								singlePicklistFieldValue.PreviousValue = null;
							}
							break;
					}
				}
			}
		}
	}
}
