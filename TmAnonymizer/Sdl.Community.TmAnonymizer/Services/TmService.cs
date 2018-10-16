using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Sdl.Community.SdlTmAnonymizer.Controls.ProgressDialog;
using Sdl.Community.SdlTmAnonymizer.Extensions;
using Sdl.Community.SdlTmAnonymizer.Model;
using Sdl.Community.SdlTmAnonymizer.Studio;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.TranslationMemory;
using Sdl.LanguagePlatform.TranslationMemoryApi;

namespace Sdl.Community.SdlTmAnonymizer.Services
{
	public class TmService
	{
		private readonly SettingsService _settingsService;
		private readonly object _lockObject = new object();

		public TmService(SettingsService settingsService)
		{
			_settingsService = settingsService;
		}

		public List<TmTranslationUnit> LoadTranslationUnits(ProgressDialogContext context, TmFile tmFile, TranslationProviderServer translationProvider, LanguageDirection providerLanguageDirection)
		{
			if (tmFile == null)
			{
				return null;
			}

			lock (_lockObject)
			{
				var languageDirection = tmFile.TmLanguageDirections?.FirstOrDefault(a =>
					Equals(a.Source, providerLanguageDirection.Source) && Equals(a.Target, providerLanguageDirection.Target));

				if (languageDirection == null)
				{
					return null;
				}

				if (languageDirection.TranslationUnits != null)
				{
					return languageDirection.TranslationUnits;
				}

				if (File.Exists(tmFile.CachePath))
				{
					var serializer = new SerializerService();
					context.Report(0, "Reading data from cache...");
					languageDirection.TranslationUnits = serializer.Read<List<TmTranslationUnit>>(tmFile.CachePath);
					return languageDirection.TranslationUnits;
				}

				if (tmFile.IsServerTm)
				{
					var translationMemory = translationProvider.GetTranslationMemory(tmFile.Path, TranslationMemoryProperties.All);

					var serverBasedLanguageDirection = translationMemory.LanguageDirections.FirstOrDefault(a =>
						a.SourceLanguage.Equals(providerLanguageDirection.Source) && a.TargetLanguage.Equals(providerLanguageDirection.Target));

					if (serverBasedLanguageDirection != null)
					{
						ReadTranslationUnits(context, serverBasedLanguageDirection, languageDirection);
					}
				}
				else
				{
					var translationMemory = new FileBasedTranslationMemory(tmFile.Path);

					if (translationMemory.LanguageDirection.SourceLanguage.Equals(providerLanguageDirection.Source) &&
						translationMemory.LanguageDirection.TargetLanguage.Equals(providerLanguageDirection.Target))
					{
						ReadTranslationUnits(context, translationMemory.LanguageDirection, languageDirection);
					}
				}

				SaveTmCacheStorage(context, tmFile, languageDirection);

				return languageDirection.TranslationUnits;
			}
		}
	
		public List<TmTranslationUnit> LoadTranslationUnits(ProgressDialogContext context, TmFile tmFile, TranslationProviderServer translationProvider, List<LanguageDirection> languageDirections)
		{
			var translationUnits = new List<TmTranslationUnit>();
			foreach (var languageDirection in languageDirections)
			{
				var tus = LoadTranslationUnits(context, tmFile, translationProvider, languageDirection);
				if (tus != null)
				{
					translationUnits.AddRange(tus);
				}
			}

			return translationUnits;
		}

		public void SaveTmCacheStorage(ProgressDialogContext context, TmFile tmFile, TmLanguageDirection languageDirection)
		{
			context.Report(0, "Saving data to cache...");
			if (string.IsNullOrEmpty(tmFile.CachePath) || !File.Exists(tmFile.CachePath))
			{
				var path = Path.Combine(_settingsService.PathInfo.TemporaryStorageFullPath,
					Path.GetFileName(tmFile.Name) + "." +
					languageDirection.Source.Name + "-" +
					languageDirection.Target.Name + ".xml");

				var index = 0;
				while (File.Exists(path) && index < 1000)
				{
					path = Path.Combine(_settingsService.PathInfo.TemporaryStorageFullPath,
						Path.GetFileName(tmFile.Name) + "." +
						languageDirection.Source.Name + "-" +
						languageDirection.Target.Name + "." +
						(index++).ToString().PadLeft(4, '0') + ".xml");
				}

				tmFile.CachePath = path;
			}

			var serializer = new SerializerService();
			serializer.Save(languageDirection.TranslationUnits, tmFile.CachePath);

			var settings = _settingsService.GetSettings();
			_settingsService.SaveSettings(settings);
		}

		public AnonymizeTranslationMemory FileBaseTmGetTranslationUnits(ProgressDialogContext context,
			TmFile tmFile, PersonalDataParsingService personalDataParsingService, out List<SourceSearchResult> sourceSearchResult)
		{
			var translationMemory = new FileBasedTranslationMemory(tmFile.Path);
			var tus = LoadTranslationUnits(context, tmFile, null,
				new LanguageDirection
				{
					Source = translationMemory.LanguageDirection.SourceLanguage,
					Target = translationMemory.LanguageDirection.TargetLanguage
				});

			sourceSearchResult = new List<SourceSearchResult>();

			decimal iTotal = tmFile.TranslationUnits;
			decimal iCurrent = 0;
			foreach (var translationUnit in tus)
			{
				if (context.CheckCancellationPending())
				{
					break;
				}

				iCurrent++;
				if (iCurrent == 1 || iCurrent % 100 == 0)
				{
					var progress = iCurrent / iTotal * 100;
					context.Report(Convert.ToInt32(progress), "Parsing: " + iCurrent + " of " + iTotal + " Translation Units");
				}

				var sourceText = translationUnit.SourceSegment.ToPlain();
				var targetText = translationUnit.TargetSegment.ToPlain();
				var sourceContainsPi = personalDataParsingService.ContainsPi(sourceText);
				var targetContainsPi = personalDataParsingService.ContainsPi(targetText);

				if (sourceContainsPi || targetContainsPi)
				{
					var searchResult = new SourceSearchResult
					{
						Id = translationUnit.ResourceId.Guid.ToString(),
						SourceText = sourceText,
						TargetText = targetText,
						TmFilePath = tmFile.Path,
						IconFilePath = "../Resources/TranslationMemory.ico",
						IsServer = false,
						SegmentNumber = translationUnit.ResourceId.Id.ToString(),
						SelectedWordsDetails = new List<WordDetails>(),
						DeSelectedWordsDetails = new List<WordDetails>(),
						TargetDeSelectedWordsDetails = new List<WordDetails>(),
						TargetSelectedWordsDetails = new List<WordDetails>()

					};
					if (sourceContainsPi)
					{
						searchResult.IsSourceMatch = true;
						searchResult.MatchResult = new MatchResult
						{
							Positions = personalDataParsingService.GetPersonalDataPositions(sourceText)
						};
					}
					if (targetContainsPi)
					{
						searchResult.IsTargetMatch = true;
						searchResult.TargetMatchResult = new MatchResult
						{
							Positions = personalDataParsingService.GetPersonalDataPositions(targetText)
						};
					}

					sourceSearchResult.Add(searchResult);
				}
			}

			return new AnonymizeTranslationMemory
			{
				TmFile = tmFile,
				TranslationUnits = tus,
				TranslationUnitDetails = new List<TranslationUnitDetails>()
			};
		}

		public AnonymizeTranslationMemory ServerBasedTmGetTranslationUnits(ProgressDialogContext context, TmFile tmFile, TranslationProviderServer translationProvider,
			PersonalDataParsingService personalDataParsingService, out List<SourceSearchResult> sourceSearchResult)
		{
			var allTusForLanguageDirections = new List<TmTranslationUnit>();
			var searchResults = new List<SourceSearchResult>();

			var translationMemory = translationProvider.GetTranslationMemory(tmFile.Path, TranslationMemoryProperties.All);

			var languageDirections = translationMemory.LanguageDirections;

			decimal iTotal = tmFile.TranslationUnits;
			decimal iCurrent = 0;

			foreach (var languageDirection in languageDirections)
			{
				var translationUnits = LoadTranslationUnits(context, tmFile, translationProvider, new LanguageDirection
				{
					Source = languageDirection.SourceLanguage,
					Target = languageDirection.TargetLanguage
				});

				if (translationUnits != null)
				{
					allTusForLanguageDirections.AddRange(translationUnits);
					foreach (var translationUnit in translationUnits)
					{
						if (context.CheckCancellationPending())
						{
							break;
						}

						iCurrent++;
						if (iCurrent == 1 || iCurrent % 100 == 0)
						{
							var progress = iCurrent / iTotal * 100;
							context.Report(Convert.ToInt32(progress), "Parsing: " + iCurrent + " of " + iTotal + " Translation Units");
						}

						var sourceText = translationUnit.SourceSegment.ToPlain();
						var targetText = translationUnit.TargetSegment.ToPlain();
						var sourceContainsPi = personalDataParsingService.ContainsPi(sourceText);
						var targetContainsPi = personalDataParsingService.ContainsPi(targetText);

						if (sourceContainsPi || targetContainsPi)
						{
							var searchResult = new SourceSearchResult
							{
								Id = translationUnit.ResourceId.Guid.ToString(),
								SourceText = sourceText,
								TargetText = targetText,
								TmFilePath = tmFile.Path,
								IsServer = true,
								IconFilePath = "../Resources/ServerBasedTranslationMemory.ico",
								SegmentNumber = translationUnit.ResourceId.Id.ToString(),
								SelectedWordsDetails = new List<WordDetails>(),
								DeSelectedWordsDetails = new List<WordDetails>(),
								TargetDeSelectedWordsDetails = new List<WordDetails>(),
								TargetSelectedWordsDetails = new List<WordDetails>()
							};

							if (sourceContainsPi)
							{
								searchResult.IsSourceMatch = true;
								searchResult.MatchResult = new MatchResult
								{
									Positions = personalDataParsingService.GetPersonalDataPositions(sourceText)
								};
							}
							if (targetContainsPi)
							{
								searchResult.IsTargetMatch = true;
								searchResult.TargetMatchResult = new MatchResult
								{
									Positions = personalDataParsingService.GetPersonalDataPositions(targetText)
								};
							}

							searchResults.Add(searchResult);
						}
					}
				}
			}

			sourceSearchResult = searchResults;

			return new AnonymizeTranslationMemory
			{
				TmFile = tmFile,
				TranslationUnits = allTusForLanguageDirections,
				TranslationUnitDetails = new List<TranslationUnitDetails>()
			};
		}

		public void AnonymizeServerBasedTu(ProgressDialogContext context, List<AnonymizeTranslationMemory> anonymizeTranslationMemories)
		{
			PrepareTranslationUnits(context, anonymizeTranslationMemories);

			decimal iCurrent = 0;
			decimal iTotalUnits = 0;
			foreach (var anonymizeTranslationMemory in anonymizeTranslationMemories)
			{
				iTotalUnits += anonymizeTranslationMemory.TranslationUnitDetails.Count;
			}

			if (iTotalUnits == 0)
			{
				return;
			}

			foreach (var translationMemory in anonymizeTranslationMemories)
			{
				var uri = new Uri(translationMemory.TmFile.Credentials.Url);
				var translationProvider = new TranslationProviderServer(uri, false, translationMemory.TmFile.Credentials.UserName, translationMemory.TmFile.Credentials.Password);
				var tm = translationProvider.GetTranslationMemory(translationMemory.TmFile.Path, TranslationMemoryProperties.All);

				var groupsOf = 100;
				var tusGroups = new List<List<TmTranslationUnit>> { new List<TmTranslationUnit>(translationMemory.TranslationUnits) };
				if (translationMemory.TranslationUnits.Count > groupsOf)
				{
					tusGroups = translationMemory.TranslationUnits.ChunkBy(groupsOf);
				}

				if (tusGroups.Count == 0)
				{
					continue;
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

					foreach (var languageDirection in tm.LanguageDirections)
					{
						var tusToUpdate = new List<TranslationUnit>();
						foreach (var tu in tus)
						{
							if (languageDirection.SourceLanguage.Name.Equals(tu.SourceSegment.Language) &&
								languageDirection.TargetLanguage.Name.Equals(tu.TargetSegment.Language))
							{
								var unit = CreateTranslationUnit(tu, languageDirection);

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

				tm.Save();

				foreach (var languageDirection in translationMemory.TmFile.TmLanguageDirections)
				{
					SaveTmCacheStorage(context, translationMemory.TmFile, languageDirection);
				}
			}
		}

		public void AnonymizeFileBasedTu(ProgressDialogContext context, List<AnonymizeTranslationMemory> anonymizeTranslationMemories)
		{
			PrepareTranslationUnits(context, anonymizeTranslationMemories);

			decimal iCurrent = 0;
			decimal iTotalUnits = 0;
			foreach (var anonymizeTranslationMemory in anonymizeTranslationMemories)
			{
				iTotalUnits += anonymizeTranslationMemory.TranslationUnitDetails.Count;
			}

			if (iTotalUnits == 0)
			{
				return;
			}

			foreach (var translationMemory in anonymizeTranslationMemories)
			{
				var tm = new FileBasedTranslationMemory(translationMemory.TmFile.Path);

				var groupsOf = 200;
				var tusGroups = new List<List<TmTranslationUnit>> { new List<TmTranslationUnit>(translationMemory.TranslationUnits) };
				if (translationMemory.TranslationUnits.Count > groupsOf)
				{
					tusGroups = translationMemory.TranslationUnits.ChunkBy(groupsOf);
				}

				if (tusGroups.Count == 0)
				{
					continue;
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
							var unit = CreateTranslationUnit(tu, tm.LanguageDirection);

							tusToUpdate.Add(unit);
						}
					}

					if (tusToUpdate.Count > 0)
					{
						//TODO - output results to log
						var results = tm.LanguageDirection.UpdateTranslationUnits(tusToUpdate.ToArray());
					}
				}

				tm.Save();

				foreach (var languageDirection in translationMemory.TmFile.TmLanguageDirections)
				{
					SaveTmCacheStorage(context, translationMemory.TmFile, languageDirection);
				}
			}
		}

		public TranslationUnit CreateTranslationUnit(TmTranslationUnit tu, ITranslationProviderLanguageDirection languageDirection)
		{
			var unit = new TranslationUnit
			{
				ResourceId = tu.ResourceId,
				FieldValues = GetFieldValues(tu.FieldValues),
				SystemFields = tu.SystemFields,
				SourceSegment = new Segment
				{
					Culture = languageDirection.SourceLanguage,
					Elements = tu.SourceSegment.Elements
				},
				TargetSegment = new Segment
				{
					Culture = languageDirection.TargetLanguage,
					Elements = tu.TargetSegment.Elements
				}
			};

			return unit;
		}

		public List<string> GetMultipleStringValues(string fieldValue, FieldValueType fieldValueType)
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

		private List<Model.FieldDefinitions.FieldValue> SetFieldValues(FieldValues fieldValues)
		{
			var result = new List<Model.FieldDefinitions.FieldValue>();

			foreach (var fieldValue in fieldValues)
			{
				switch (fieldValue.ValueType)
				{
					case FieldValueType.Unknown:
						break;
					case FieldValueType.SingleString:
						var singleStringValue = new Model.FieldDefinitions.SingleStringFieldValue();
						singleStringValue.Name = fieldValue.Name;
						singleStringValue.Value = GetMultipleStringValues(fieldValue.GetValueString(), fieldValue.ValueType)[0];

						result.Add(singleStringValue);
						break;
					case FieldValueType.MultipleString:
						var multipleStringValue = new Model.FieldDefinitions.MultipleStringFieldValue();
						multipleStringValue.Name = fieldValue.Name;
						multipleStringValue.Values =
							new HashSet<string>(GetMultipleStringValues(fieldValue.GetValueString(), fieldValue.ValueType));

						result.Add(multipleStringValue);
						break;
					case FieldValueType.DateTime:
						var dateTimeValue = new Model.FieldDefinitions.DateTimeFieldValue();
						dateTimeValue.Name = fieldValue.Name;
						dateTimeValue.Value = DateTime.Parse(GetMultipleStringValues(fieldValue.GetValueString(), fieldValue.ValueType)[0]);

						result.Add(dateTimeValue);
						break;
					case FieldValueType.SinglePicklist:
						var singlePickValue = new Model.FieldDefinitions.SinglePicklistFieldValue();
						singlePickValue.Name = fieldValue.Name;
						singlePickValue.Value = new PicklistItem();

						if (fieldValue is SinglePicklistFieldValue singlePicklistFieldValue)
						{
							singlePickValue.Value.ID = singlePicklistFieldValue.Value.ID;
							singlePickValue.Value.Name = singlePicklistFieldValue.Value.Name;

							result.Add(singlePickValue);
						}
						break;
					case FieldValueType.MultiplePicklist:
						var multiplePickValue = new Model.FieldDefinitions.MultiplePicklistFieldValue();
						multiplePickValue.Name = fieldValue.Name;
						multiplePickValue.Values = new List<PicklistItem>();

						if (fieldValue is MultiplePicklistFieldValue multiplePicklistFieldValue)
						{
							multiplePickValue.Values = multiplePicklistFieldValue.Values;
							result.Add(multiplePickValue);
						}
						break;
					case FieldValueType.Integer:
						var intValue = new Model.FieldDefinitions.IntFieldValue();
						intValue.Name = fieldValue.Name;
						intValue.Value = int.Parse(GetMultipleStringValues(fieldValue.GetValueString(), fieldValue.ValueType)[0]);

						result.Add(intValue);
						break;
					default:
						throw new ArgumentOutOfRangeException();
				}
			}

			return result;
		}

		private FieldValues GetFieldValues(IEnumerable<Model.FieldDefinitions.FieldValue> fieldValues)
		{
			var result = new FieldValues();
			foreach (var fieldValue in fieldValues)
			{
				switch (fieldValue.ValueType)
				{
					case FieldValueType.Unknown:
						break;
					case FieldValueType.SingleString:
						var singleStringValue = new SingleStringFieldValue();
						singleStringValue.Name = fieldValue.Name;
						singleStringValue.Value = GetMultipleStringValues(fieldValue.GetValueString(), fieldValue.ValueType)[0];

						result.Add(singleStringValue);
						break;
					case FieldValueType.MultipleString:
						var multipleStringValue = new MultipleStringFieldValue();
						multipleStringValue.Name = fieldValue.Name;
						multipleStringValue.Values =
							new HashSet<string>(GetMultipleStringValues(fieldValue.GetValueString(), fieldValue.ValueType));

						result.Add(multipleStringValue);
						break;
					case FieldValueType.DateTime:
						var dateTimeValue = new DateTimeFieldValue();
						dateTimeValue.Name = fieldValue.Name;
						dateTimeValue.Value = DateTime.Parse(GetMultipleStringValues(fieldValue.GetValueString(), fieldValue.ValueType)[0]);

						result.Add(dateTimeValue);
						break;
					case FieldValueType.SinglePicklist:
						var singlePickValue = new SinglePicklistFieldValue();
						singlePickValue.Name = fieldValue.Name;
						singlePickValue.Value = new PicklistItem();

						if (fieldValue is Model.FieldDefinitions.SinglePicklistFieldValue singlePicklistFieldValue)
						{
							singlePickValue.Value.ID = singlePicklistFieldValue.Value.ID;
							singlePickValue.Value.Name = singlePicklistFieldValue.Value.Name;

							result.Add(singlePickValue);
						}
						break;
					case FieldValueType.MultiplePicklist:
						var multiplePickValue = new MultiplePicklistFieldValue();
						multiplePickValue.Name = fieldValue.Name;
						multiplePickValue.Values = new List<PicklistItem>();

						if (fieldValue is Model.FieldDefinitions.MultiplePicklistFieldValue multiplePicklistFieldValue)
						{
							multiplePickValue.Values = multiplePicklistFieldValue.Values;
							result.Add(multiplePickValue);
						}
						break;
					case FieldValueType.Integer:
						var intValue = new IntFieldValue();
						intValue.Name = fieldValue.Name;
						intValue.Value = int.Parse(GetMultipleStringValues(fieldValue.GetValueString(), fieldValue.ValueType)[0]);

						result.Add(intValue);
						break;
					default:
						throw new ArgumentOutOfRangeException();
				}
			}

			return result;
		}

		
		private void PrepareTranslationUnits(ProgressDialogContext context, List<AnonymizeTranslationMemory> anonymizeTranslationMemories)
		{
			decimal iCurrent = 0;
			decimal iTotalUnits = 0;
			foreach (var anonymizeTranslationMemory in anonymizeTranslationMemories)
			{
				iTotalUnits += anonymizeTranslationMemory.TranslationUnitDetails.Count;
			}

			if (iTotalUnits == 0)
			{
				return;
			}

			foreach (var anonymizeTranslationMemory in anonymizeTranslationMemories)
			{
				foreach (var tuDetails in anonymizeTranslationMemory.TranslationUnitDetails)
				{
					iCurrent++;
					if (context != null && context.CheckCancellationPending())
					{
						break;
					}

					var progress = iCurrent / iTotalUnits * 100;
					context?.Report(Convert.ToInt32(progress), "Preparing: " + iCurrent + " of " + iTotalUnits + " Translation Units");

					if (tuDetails.IsSourceMatch)
					{
						AnonymizeSegment(tuDetails, tuDetails.TranslationUnit.SourceSegment.Elements.ToList(), true);
					}

					if (tuDetails.IsTargetMatch)
					{
						AnonymizeSegment(tuDetails, tuDetails.TranslationUnit.TargetSegment.Elements.ToList(), false);
					}
				}
			}
		}

		private void ReadTranslationUnits(ProgressDialogContext context, ITranslationMemoryLanguageDirection languageDirection, TmLanguageDirection localLanguageDirection)
		{
			decimal iTotalUnits = languageDirection.GetTranslationUnitCount();

			if (iTotalUnits == 0)
			{
				return;
			}

			var tus = new List<TmTranslationUnit>();

			decimal groups = 1;
			var unitCount = (int)iTotalUnits;
			decimal threshold = 1000;

			if (iTotalUnits > threshold)
			{
				groups = iTotalUnits / threshold;
				unitCount = Convert.ToInt32(iTotalUnits / groups);
			}

			var tmIterator = new RegularIterator
			{
				Forward = true,
				MaxCount = unitCount
			};

			for (var i = 1; i <= groups; i++)
			{
				if (context != null && context.CheckCancellationPending())
				{
					break;
				}

				var iCurrent = i * unitCount;
				var progress = iCurrent / iTotalUnits * 100;
				context?.Report(Convert.ToInt32(progress), "Reading: " + iCurrent + " of " + iTotalUnits + " Translation Units");

				tus.AddRange(AddTranslationUnits(languageDirection.GetTranslationUnits(ref tmIterator)));
			}

			if (context != null && context.CheckCancellationPending())
			{
				return;
			}

			if (tmIterator.ProcessedTranslationUnits < iTotalUnits)
			{
				tmIterator.MaxCount = (int)iTotalUnits - tmIterator.ProcessedTranslationUnits;

				tus.AddRange(AddTranslationUnits(languageDirection.GetTranslationUnits(ref tmIterator)));
			}

			if (context != null && context.CheckCancellationPending())
			{
				return;
			}

			localLanguageDirection.TranslationUnits = tus;
			localLanguageDirection.TranslationUnitsCount = tus.Count;
		}

		private IEnumerable<TmTranslationUnit> AddTranslationUnits(IEnumerable<TranslationUnit> units)
		{
			var tus = new List<TmTranslationUnit>();
			tus.AddRange(units.Select(unit => new TmTranslationUnit
			{
				ResourceId = unit.ResourceId,
				FieldValues = SetFieldValues(unit.FieldValues),
				SystemFields = unit.SystemFields,
				SourceSegment = new TmSegment
				{
					Elements = unit.SourceSegment.Elements,
					Language = unit.SourceSegment.Culture.Name
				},
				TargetSegment = new TmSegment
				{
					Elements = unit.TargetSegment.Elements,
					Language = unit.TargetSegment.Culture.Name
				}
			}));

			return tus;
		}

		private void AnonymizeSegment(TranslationUnitDetails tuDetails, List<SegmentElement> elements, bool isSource)
		{
			var elementsContainsTag = elements.Any(s => s.GetType().UnderlyingSystemType.Name.Equals("Tag"));
			if (elementsContainsTag)
			{
				//check if there are selected words from the ui
				if (tuDetails.SelectedWordsDetails.Any() || tuDetails.TargetSelectedWordsDetails.Any())
				{
					AnonymizeSelectedWordsFromPreview(tuDetails, elements, isSource);
				}

				AnonymizeSegmentsWithTags(tuDetails, isSource);
			}
			else
			{
				if (tuDetails.SelectedWordsDetails.Any() || tuDetails.TargetSelectedWordsDetails.Any())
				{
					AnonymizeSelectedWordsFromPreview(tuDetails, elements, isSource);
				}

				AnonymizeSegmentsWithoutTags(tuDetails, isSource);
			}
		}

		private static void AnonymizeSelectedWordsFromPreview(TranslationUnitDetails translationUnitDetails, IEnumerable<SegmentElement> translationElements, bool isSource)
		{
			if (isSource)
			{
				AnonymizeSelectedWordsFromPreview(translationUnitDetails.TranslationUnit.SourceSegment, translationElements,
					translationUnitDetails.SelectedWordsDetails);
			}
			else
			{
				AnonymizeSelectedWordsFromPreview(translationUnitDetails.TranslationUnit.TargetSegment, translationElements,
					translationUnitDetails.TargetSelectedWordsDetails);
			}
		}

		private static void AnonymizeSelectedWordsFromPreview(TmSegment segment, IEnumerable<SegmentElement> translationElements, List<WordDetails> selectedWords)
		{
			segment.Elements.Clear();

			foreach (var element in translationElements.ToList())
			{
				var visitor = new SelectedWordsFromUiElementVisitor(selectedWords);
				element.AcceptSegmentElementVisitor(visitor);

				//new elements after splited the text for selected words
				var newElements = visitor.SegmentColection;
				if (newElements?.Count > 0)
				{
					foreach (var seg in newElements.Cast<SegmentElement>())
					{
						//add segments back Source Segment
						AddElement(seg, segment.Elements);
					}
				}
				else
				{
					//add remaining words				
					AddElement(element, segment.Elements);
				}
			}
		}

		private void AnonymizeSegmentsWithoutTags(TranslationUnitDetails translationUnitDetails, bool isSource)
		{
			if (isSource)
			{
				AnonymizeSegmentsWithoutTags(translationUnitDetails.TranslationUnit.SourceSegment, translationUnitDetails.RemovedWordsFromMatches);
			}
			else
			{
				AnonymizeSegmentsWithoutTags(translationUnitDetails.TranslationUnit.TargetSegment, translationUnitDetails.TargetRemovedWordsFromMatches);
			}
		}

		private void AnonymizeSegmentsWithoutTags(TmSegment segment, List<WordDetails> removeWords)
		{
			var segmentElements = new List<SegmentElement>();

			foreach (var element in segment.Elements.ToList())
			{
				var visitor = new SegmentElementVisitor(removeWords, _settingsService);

				element.AcceptSegmentElementVisitor(visitor);
				var segmentColection = visitor.SegmentColection;

				if (segmentColection?.Count > 0)
				{
					foreach (var seg in segmentColection.Cast<SegmentElement>())
					{
						AddElement(seg, segmentElements);
					}
				}
				else
				{
					//add remaining words
					AddElement(element, segmentElements);
				}
			}

			//clear initial list
			segment.Elements.Clear();

			//add new elements list to Translation Unit
			segment.Elements = segmentElements;
		}

		private void AnonymizeSegmentsWithTags(TranslationUnitDetails translationUnitDetails, bool isSource)
		{
			if (isSource)
			{
				AnonymizeSegmentsWithTags(translationUnitDetails.TranslationUnit.SourceSegment, translationUnitDetails.RemovedWordsFromMatches);
			}
			else
			{
				AnonymizeSegmentsWithTags(translationUnitDetails.TranslationUnit.TargetSegment, translationUnitDetails.TargetRemovedWordsFromMatches);
			}
		}

		private void AnonymizeSegmentsWithTags(TmSegment segment, List<WordDetails> removeWords)
		{
			for (var i = 0; i < segment.Elements.Count; i++)
			{
				if (!segment.Elements[i].GetType().UnderlyingSystemType.Name.Equals("Text"))
				{
					continue;
				}

				var visitor = new SegmentElementVisitor(removeWords, _settingsService);
				//check for PI in each element from the list
				segment.Elements[i].AcceptSegmentElementVisitor(visitor);
				var segmentColection = visitor.SegmentColection;

				if (segmentColection?.Count > 0)
				{
					var segmentElements = new List<SegmentElement>();
					//if element contains PI add it to a list of Segment Elements
					foreach (var seg in segmentColection.Cast<SegmentElement>())
					{
						AddElement(seg, segmentElements);
					}

					//remove from the list original element at position
					segment.Elements.RemoveAt(i);

					//to the same position add the new list with elements (Text + Tag)
					segment.Elements.InsertRange(i, segmentElements);
				}
			}
		}

		private static void AddElement(SegmentElement element, ICollection<SegmentElement> elements)
		{
			var text = element as Text;
			var tag = element as Tag;
			if (text != null)
			{
				elements.Add(text);
			}

			if (tag != null)
			{
				elements.Add(tag);
			}
		}
	}
}

