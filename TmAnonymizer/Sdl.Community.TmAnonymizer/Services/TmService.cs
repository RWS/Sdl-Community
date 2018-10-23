using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Sdl.Community.SdlTmAnonymizer.Controls.ProgressDialog;
using Sdl.Community.SdlTmAnonymizer.Extensions;
using Sdl.Community.SdlTmAnonymizer.Model;
using Sdl.Community.SdlTmAnonymizer.Model.TM;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.TranslationMemory;
using Sdl.LanguagePlatform.TranslationMemoryApi;

namespace Sdl.Community.SdlTmAnonymizer.Services
{
	public class TmService
	{
		private readonly SettingsService _settingsService;
		private readonly ContentParsingService _contentParsingService;
		private readonly object _lockObject = new object();

		public TmService(SettingsService settingsService, ContentParsingService contentParsingService)
		{
			_settingsService = settingsService;
			_contentParsingService = contentParsingService;
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

				if (!tmFile.IsServerTm)
				{
					languageDirection.TranslationUnits = GetTranslationUnitsFromLocalTm(context, tmFile);
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

						SaveTmCacheStorage(context, tmFile, languageDirection);
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

		public AnonymizeTranslationMemory GetAnonymizeTmFileBased(ProgressDialogContext context, TmFile tmFile, out List<ContentSearchResult> results)
		{
			var tm = new FileBasedTranslationMemory(tmFile.Path);
			var tus = LoadTranslationUnits(context, tmFile, null,
				new LanguageDirection
				{
					Source = tm.LanguageDirection.SourceLanguage,
					Target = tm.LanguageDirection.TargetLanguage
				});

			results = new List<ContentSearchResult>();

			results.AddRange(ParseTranslationUnits(context, tmFile, tus));

			return new AnonymizeTranslationMemory
			{
				TmFile = tmFile,
				TranslationUnits = tus,
				TranslationUnitDetails = new List<TranslationUnitDetails>()
			};
		}

		public AnonymizeTranslationMemory GetAnonymizeTmServerBased(ProgressDialogContext context, TmFile tmFile, TranslationProviderServer translationProvider, out List<ContentSearchResult> results)
		{
			results = new List<ContentSearchResult>();

			var tm = translationProvider.GetTranslationMemory(tmFile.Path, TranslationMemoryProperties.All);
			var allTus = new List<TmTranslationUnit>();

			foreach (var languageDirection in tm.LanguageDirections)
			{
				var tus = LoadTranslationUnits(context, tmFile, translationProvider, new LanguageDirection
				{
					Source = languageDirection.SourceLanguage,
					Target = languageDirection.TargetLanguage
				});

				if (tus != null)
				{
					allTus.AddRange(tus);

					results.AddRange(ParseTranslationUnits(context, tmFile, tus));
				}
			}

			return new AnonymizeTranslationMemory
			{
				TmFile = tmFile,
				TranslationUnits = allTus,
				TranslationUnitDetails = new List<TranslationUnitDetails>()
			};
		}

		private IEnumerable<ContentSearchResult> ParseTranslationUnits(ProgressDialogContext context, TmFile tmFile, IReadOnlyCollection<TmTranslationUnit> tus)
		{
			context.Report(0, "Parsing content...");

			decimal iTotal = tus.Count;
			decimal iCurrent = 0;

			var results = new List<ContentSearchResult>();
			var rules = _settingsService.GetRules();
			foreach (var tu in tus)
			{
				iCurrent++;
				if (iCurrent % 1000 == 0)
				{
					if (context.CheckCancellationPending())
					{
						break;
					}

					var progress = iCurrent / iTotal * 100;
					context.Report(Convert.ToInt32(progress), "Parsing: " + iCurrent + " of " + iTotal + " Translation Units " +
															  "(" + tu.SourceSegment.Language + "-" + tu.TargetSegment.Language + ")");
				}

				var sourceText = tu.SourceSegment.ToPlain();
				var targetText = tu.TargetSegment.ToPlain();

				var sourcePositions = _contentParsingService.GetMatchPositions(sourceText, rules);
				var targetPositions = _contentParsingService.GetMatchPositions(targetText, rules);

				if (sourcePositions?.Count > 0 || targetPositions?.Count > 0)
				{
					var searchResult = new ContentSearchResult
					{
						TranslationUnit = tu,
						SourceText = tu.SourceSegment.ToPlain(),
						TargetText = tu.TargetSegment.ToPlain(),
						TmFilePath = tmFile.Path,
						IsServer = tmFile.IsServerTm,
						IconFilePath = "../Resources/ServerBasedTranslationMemory.ico",
						SelectedWordsDetails = new List<WordDetails>(),
						DeSelectedWordsDetails = new List<WordDetails>(),
						TargetDeSelectedWordsDetails = new List<WordDetails>(),
						TargetSelectedWordsDetails = new List<WordDetails>()
					};

					if (sourcePositions?.Count > 0)
					{
						searchResult.IsSourceMatch = true;
						searchResult.MatchResult = new MatchResult
						{
							Positions = sourcePositions
						};
					}

					if (targetPositions?.Count > 0)
					{
						searchResult.IsTargetMatch = true;
						searchResult.TargetMatchResult = new MatchResult
						{
							Positions = targetPositions
						};
					}

					results.Add(searchResult);
				}
			}

			return results;
		}

		public void AnonymizeServerBasedTm(ProgressDialogContext context, List<AnonymizeTranslationMemory> anonymizeTranslationMemories)
		{
			BackupServerBasedTms(context, anonymizeTranslationMemories.Select(a => a.TmFile).ToList());

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
					if (iCurrent % 10 == 0)
					{
						if (context != null && context.CheckCancellationPending())
						{
							break;
						}

						var progress = iCurrent / iTotalUnits * 100;
						context?.Report(Convert.ToInt32(progress), "Updating: " + iCurrent + " of " + iTotalUnits + " Translation Units");
					}

					foreach (var languageDirection in tm.LanguageDirections)
					{
						var tusToUpdate = new List<LanguagePlatform.TranslationMemory.TranslationUnit>();
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

		public void AnonymizeFileBasedTm(ProgressDialogContext context, List<AnonymizeTranslationMemory> anonymizeTranslationMemories)
		{
			BackupFileBasedTms(context, anonymizeTranslationMemories.Select(a => a.TmFile).ToList());

			PrepareTranslationUnits(context, anonymizeTranslationMemories);

			//var settings = _settingsService.GetSettings();
			//if (settings.UseSqliteApiForFileBasedTm)
			//{
			//	foreach (var translationMemory in anonymizeTranslationMemories)
			//	{
			//		UpdateTranslationUnitsContentSqlite(context, translationMemory.TmFile, translationMemory.TranslationUnits);
			//		return;
			//	}
			//}

			UpdateTranslationUnitsContent(context, anonymizeTranslationMemories);
		}

		public LanguagePlatform.TranslationMemory.TranslationUnit CreateTranslationUnit(TmTranslationUnit tu, ITranslationProviderLanguageDirection languageDirection)
		{
			var unit = new LanguagePlatform.TranslationMemory.TranslationUnit
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

		private static string GetDateTimeString()
		{
			var dt = DateTime.Now;
			return dt.Year +
				   dt.Month.ToString().PadLeft(2, '0') +
				   dt.Day.ToString().PadLeft(2, '0') +
				   "T" +
				   dt.Hour.ToString().PadLeft(2, '0') +
				   dt.Minute.ToString().PadLeft(2, '0') +
				   dt.Second.ToString().PadLeft(2, '0');
		}

		public void BackupServerBasedTms(ProgressDialogContext context, IEnumerable<TmFile> tmsCollection)
		{
			var settings = _settingsService.GetSettings();
			if (!settings.Backup)
			{
				return;
			}

			var backupTms = new List<ServerTmBackUp>();

			try
			{
				foreach (var tm in tmsCollection)
				{					
					if (tm == null)
					{
						continue;
					}

					var uri = new Uri(tm.Credentials.Url);
					var translationProvider = new TranslationProviderServer(uri, false, tm.Credentials.UserName, tm.Credentials.Password);

					context.Report(0, "Backup " + tm.Path);

					var translationMemory = translationProvider.GetTranslationMemory(tm.Path, TranslationMemoryProperties.All);
					var languageDirections = translationMemory.LanguageDirections;

					foreach (var languageDirection in languageDirections)
					{
						var folderPath = Path.Combine(settings.BackupFullPath, translationMemory.Name, languageDirection.TargetLanguageCode);

						if (!Directory.Exists(folderPath))
						{
							Directory.CreateDirectory(folderPath);
						}

						var fileName = translationMemory.Name + languageDirection.TargetLanguageCode + "." + GetDateTimeString() + ".tmx.gz";
						var filePath = Path.Combine(folderPath, fileName);

						//if tm does not exist download it
						if (!File.Exists(filePath))
						{
							var tmExporter = new ScheduledServerTranslationMemoryExport(languageDirection)
							{
								ContinueOnError = true
							};

							tmExporter.Queue();
							tmExporter.Refresh();

							var continueWaiting = true;
							while (continueWaiting)
							{
								switch (tmExporter.Status)
								{
									case ScheduledOperationStatus.Abort:
									case ScheduledOperationStatus.Aborted:
									case ScheduledOperationStatus.Cancel:
									case ScheduledOperationStatus.Cancelled:
									case ScheduledOperationStatus.Completed:
									case ScheduledOperationStatus.Error:
										continueWaiting = false;
										break;
									case ScheduledOperationStatus.Aborting:
									case ScheduledOperationStatus.Allocated:
									case ScheduledOperationStatus.Cancelling:
									case ScheduledOperationStatus.NotSet:
									case ScheduledOperationStatus.Queued:
									case ScheduledOperationStatus.Recovered:
									case ScheduledOperationStatus.Recovering:
									case ScheduledOperationStatus.Recovery:
										tmExporter.Refresh();
										break;
									default:
										continueWaiting = false;
										break;
								}
							}

							if (tmExporter.Status == ScheduledOperationStatus.Completed)
							{
								var backup = new ServerTmBackUp
								{
									ScheduledExport = tmExporter,
									FilePath = filePath
								};

								backupTms.Add(backup);
							}
							else if (tmExporter.Status == ScheduledOperationStatus.Error)
							{
								MessageBox.Show(tmExporter.ErrorMessage, Application.ProductName,
									MessageBoxButtons.OK, MessageBoxIcon.Error);
							}
						}
					}

					Task.WhenAll(Task.Run(() => Parallel.ForEach(backupTms, memory =>
					{
						using (Stream outputStream = new FileStream(memory.FilePath, FileMode.Create))
						{
							memory.ScheduledExport.DownloadExport(outputStream);
						}
					})));
				}

				
			}
			catch (Exception exception)
			{
				if (exception.Message.Equals("One or more errors occurred."))
				{
					if (exception.InnerException != null)
					{
						System.Windows.Forms.MessageBox.Show(exception.InnerException.Message, System.Windows.Forms.Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
					}
				}
				else
				{
					System.Windows.Forms.MessageBox.Show(exception.Message, System.Windows.Forms.Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
				}
			}
		}

		public void BackupFileBasedTms(ProgressDialogContext context, IEnumerable<TmFile> tmsCollection)
		{
			var settings = _settingsService.GetSettings();
			if (!settings.Backup)
			{
				return;
			}

			foreach (var tm in tmsCollection)
			{		
				if (tm == null)
				{
					continue;
				}

				context.Report(0, "Backup " + tm.Path);

				var tmInfo = new FileInfo(tm.Path);

				var extension = Path.GetExtension(tm.Name);
				var tmName = tm.Name;
				if (extension?.Length > 0)
				{
					tmName = tmName.Substring(0, tmName.Length - extension.Length);
				}

				var backupFilePath = Path.Combine(settings.BackupFullPath, tmName + "." + GetDateTimeString() + extension);

				if (!File.Exists(backupFilePath))
				{
					tmInfo.CopyTo(backupFilePath, false);
				}
			}
		}

		private void UpdateTranslationUnitsContent(ProgressDialogContext context, List<AnonymizeTranslationMemory> anonymizeTranslationMemories)
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

					var tusToUpdate = new List<LanguagePlatform.TranslationMemory.TranslationUnit>();
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
			}
		}

		private static List<TmTranslationUnit> GetTranslationUnitsFromLocalTm(ProgressDialogContext context, TmFile tmFile)
		{
			List<TmTranslationUnit> values;

			var service = new SqliteTmService(tmFile.Path, null, new SerializerService(), new SegmentService());

			try
			{
				service.OpenConnection();

				values = service.GetTranslationUnits(context, GetTmIds(tmFile, service.GeTranslationMemories()));
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

			return values;
		}

		private static void UpdateTranslationUnitsContentSqlite(ProgressDialogContext context, TmFile tmFile, List<TmTranslationUnit> units)
		{
			var service = new SqliteTmService(tmFile.Path, null, new SerializerService(), new SegmentService());

			try
			{
				service.OpenConnection();

				service.UpdateTranslationUnitContent(context, units);
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

		private static List<int> GetTmIds(TmFile tmFile, IEnumerable<TranslationMemory> tms)
		{
			var ids = new List<int>();
			foreach (var tm in tms)
			{
				foreach (var tmFileTmLanguageDirection in tmFile.TmLanguageDirections)
				{
					if (tm.SourceLangauge == tmFileTmLanguageDirection.Source.Name &&
						tm.TargetLanguage == tmFileTmLanguageDirection.Target.Name)
					{
						if (!ids.Contains(tm.Id))
						{
							ids.Add(tm.Id);
						}

						break;
					}
				}
			}

			return ids;
		}

		private static List<Model.FieldDefinitions.FieldValue> SetFieldValues(FieldValues fieldValues)
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
						singleStringValue.Value = ((SingleStringFieldValue)fieldValue).Value;

						result.Add(singleStringValue);
						break;
					case FieldValueType.MultipleString:
						var multipleStringValue = new Model.FieldDefinitions.MultipleStringFieldValue();
						multipleStringValue.Name = fieldValue.Name;
						multipleStringValue.Values = ((MultipleStringFieldValue)fieldValue).Values as HashSet<string>;

						result.Add(multipleStringValue);
						break;
					case FieldValueType.DateTime:
						var dateTimeValue = new Model.FieldDefinitions.DateTimeFieldValue();
						dateTimeValue.Name = fieldValue.Name;
						dateTimeValue.Value = ((DateTimeFieldValue)fieldValue).Value;

						result.Add(dateTimeValue);
						break;
					case FieldValueType.SinglePicklist:
						var singlePickValue = new Model.FieldDefinitions.SinglePicklistFieldValue();
						singlePickValue.Name = fieldValue.Name;
						singlePickValue.Value = new Model.FieldDefinitions.PicklistItem();

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
						multiplePickValue.Values = new List<Model.FieldDefinitions.PicklistItem>();

						if (fieldValue is MultiplePicklistFieldValue multiplePicklistFieldValue)
						{
							foreach (var value in multiplePicklistFieldValue.Values)
							{
								var picklist = new Model.FieldDefinitions.PicklistItem
								{
									ID = value.ID,
									Name = value.Name
								};

								multiplePickValue.Values.Add(picklist);
							}

							result.Add(multiplePickValue);
						}
						break;
					case FieldValueType.Integer:
						var intValue = new Model.FieldDefinitions.IntFieldValue();
						intValue.Name = fieldValue.Name;
						intValue.Value = ((IntFieldValue)fieldValue).Value;

						result.Add(intValue);
						break;
					default:
						throw new ArgumentOutOfRangeException();
				}
			}

			return result;
		}

		private static FieldValues GetFieldValues(IEnumerable<Model.FieldDefinitions.FieldValue> fieldValues)
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
						singleStringValue.Value = ((Model.FieldDefinitions.SingleStringFieldValue)fieldValue).Value;

						result.Add(singleStringValue);
						break;
					case FieldValueType.MultipleString:
						var multipleStringValue = new MultipleStringFieldValue();
						multipleStringValue.Name = fieldValue.Name;
						multipleStringValue.Values = ((Model.FieldDefinitions.MultipleStringFieldValue)fieldValue).Values;

						result.Add(multipleStringValue);
						break;
					case FieldValueType.DateTime:
						var dateTimeValue = new DateTimeFieldValue();
						dateTimeValue.Name = fieldValue.Name;
						dateTimeValue.Value = ((Model.FieldDefinitions.DateTimeFieldValue)fieldValue).Value;

						result.Add(dateTimeValue);
						break;
					case FieldValueType.SinglePicklist:
						var singlePickValue = new SinglePicklistFieldValue();
						singlePickValue.Name = fieldValue.Name;


						if (fieldValue is Model.FieldDefinitions.SinglePicklistFieldValue singlePicklistFieldValue)
						{
							singlePickValue.Value = new PicklistItem
							{
								ID = singlePicklistFieldValue.Value.ID,
								Name = singlePicklistFieldValue.Value.Name
							};

							result.Add(singlePickValue);
						}
						break;
					case FieldValueType.MultiplePicklist:
						var multiplePickValue = new MultiplePicklistFieldValue();
						multiplePickValue.Name = fieldValue.Name;
						multiplePickValue.Values = new List<PicklistItem>();

						if (fieldValue is Model.FieldDefinitions.MultiplePicklistFieldValue multiplePicklistFieldValue)
						{
							foreach (var value in multiplePicklistFieldValue.Values)
							{
								var pickList = new PicklistItem
								{
									ID = value.ID,
									Name = value.Name
								};

								multiplePickValue.Add(pickList);
							}
							result.Add(multiplePickValue);
						}
						break;
					case FieldValueType.Integer:
						var intValue = new IntFieldValue();
						intValue.Name = fieldValue.Name;
						intValue.Value = ((Model.FieldDefinitions.IntFieldValue)fieldValue).Value;

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

			var rules = _settingsService.GetRules();

			foreach (var anonymizeTranslationMemory in anonymizeTranslationMemories)
			{
				foreach (var tuDetails in anonymizeTranslationMemory.TranslationUnitDetails)
				{
					iCurrent++;

					if (iCurrent % 100 == 0)
					{
						if (context != null && context.CheckCancellationPending())
						{
							break;
						}

						var progress = iCurrent / iTotalUnits * 100;
						context?.Report(Convert.ToInt32(progress),
							"Preparing: " + iCurrent + " of " + iTotalUnits + " Translation Units");
					}

					if (tuDetails.IsSourceMatch)
					{
						AnonymizeSegment(tuDetails, tuDetails.TranslationUnit.SourceSegment.Elements.ToList(), rules, true);
					}

					if (tuDetails.IsTargetMatch)
					{
						AnonymizeSegment(tuDetails, tuDetails.TranslationUnit.TargetSegment.Elements.ToList(), rules, false);
					}
				}
			}
		}

		private static void ReadTranslationUnits(ProgressDialogContext context, ITranslationMemoryLanguageDirection languageDirection, TmLanguageDirection localLanguageDirection)
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

		private static IEnumerable<TmTranslationUnit> AddTranslationUnits(IEnumerable<Sdl.LanguagePlatform.TranslationMemory.TranslationUnit> units)
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

		private static void AnonymizeSegment(TranslationUnitDetails tuDetails, List<SegmentElement> elements, List<Rule> rules, bool isSource)
		{
			var elementsContainsTag = elements.Any(s => s.GetType().UnderlyingSystemType.Name.Equals("Tag"));
			if (elementsContainsTag)
			{
				//check if there are selected words from the ui
				if (tuDetails.SelectedWordsDetails.Any() || tuDetails.TargetSelectedWordsDetails.Any())
				{
					AnonymizeSelectedWordsFromPreview(tuDetails, elements, isSource);
				}

				AnonymizeSegmentsWithTags(tuDetails, rules, isSource);
			}
			else
			{
				if (tuDetails.SelectedWordsDetails.Any() || tuDetails.TargetSelectedWordsDetails.Any())
				{
					AnonymizeSelectedWordsFromPreview(tuDetails, elements, isSource);
				}

				AnonymizeSegmentsWithoutTags(tuDetails, rules, isSource);
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
			var anchorIds = GetAnchorIds(segment);
			foreach (var element in translationElements.ToList())
			{
				var visitor = new SelectedWordsVisitorService(selectedWords, anchorIds);
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

		private static void AnonymizeSegmentsWithoutTags(TranslationUnitDetails translationUnitDetails, List<Rule> rules, bool isSource)
		{
			if (isSource)
			{
				AnonymizeSegmentsWithoutTags(translationUnitDetails.TranslationUnit.SourceSegment, rules, translationUnitDetails.RemovedWordsFromMatches);
			}
			else
			{
				AnonymizeSegmentsWithoutTags(translationUnitDetails.TranslationUnit.TargetSegment, rules, translationUnitDetails.TargetRemovedWordsFromMatches);
			}
		}

		private static void AnonymizeSegmentsWithoutTags(TmSegment segment, List<Rule> rules, List<WordDetails> removeWords)
		{
			var segmentElements = new List<SegmentElement>();
			var anchorIds = GetAnchorIds(segment);
			foreach (var element in segment.Elements.ToList())
			{
				var visitor = new SegmentElementVisitorService(removeWords, anchorIds, rules);

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

		private static void AnonymizeSegmentsWithTags(TranslationUnitDetails translationUnitDetails, List<Rule> rules, bool isSource)
		{
			if (isSource)
			{
				AnonymizeSegmentsWithTags(translationUnitDetails.TranslationUnit.SourceSegment, rules, translationUnitDetails.RemovedWordsFromMatches);
			}
			else
			{
				AnonymizeSegmentsWithTags(translationUnitDetails.TranslationUnit.TargetSegment, rules, translationUnitDetails.TargetRemovedWordsFromMatches);
			}
		}

		private static void AnonymizeSegmentsWithTags(TmSegment segment, List<Rule> rules, List<WordDetails> removeWords)
		{
			var anchorIds = GetAnchorIds(segment);
			for (var i = 0; i < segment.Elements.Count; i++)
			{
				if (!segment.Elements[i].GetType().UnderlyingSystemType.Name.Equals("Text"))
				{
					continue;
				}

				var visitor = new SegmentElementVisitorService(removeWords, anchorIds, rules);

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

		private static List<int> GetAnchorIds(TmSegment segment)
		{
			var anchorIds = new List<int>();
			foreach (var element in segment.Elements)
			{
				if (element is Tag tag)
				{
					if (!anchorIds.Contains(tag.AlignmentAnchor))
					{
						anchorIds.Add(tag.AlignmentAnchor);
					}
					if (!anchorIds.Contains(tag.Anchor))
					{
						anchorIds.Add(tag.Anchor);
					}
				}
			}

			return anchorIds;
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

