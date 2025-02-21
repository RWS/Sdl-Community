﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Threading;
using Sdl.Community.SdlDataProtectionSuite.SdlTmAnonymizer.Controls.ProgressDialog;
using Sdl.Community.SdlDataProtectionSuite.SdlTmAnonymizer.Extensions;
using Sdl.Community.SdlDataProtectionSuite.SdlTmAnonymizer.Model;
using Sdl.Community.SdlDataProtectionSuite.SdlTmAnonymizer.Model.Log;
using Sdl.Community.SdlDataProtectionSuite.SdlTmAnonymizer.Model.TM;
using Sdl.Community.SdlDataProtectionSuite.SdlTmAnonymizer.ViewModel;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.TranslationMemory;
using Sdl.LanguagePlatform.TranslationMemoryApi;

namespace Sdl.Community.SdlDataProtectionSuite.SdlTmAnonymizer.Services
{
	public class TmService
	{
        public GroupshareCredentialManager GroupshareCredentialManager { get; }
        private readonly SettingsService _settingsService;
		private readonly ContentParsingService _contentParsingService;
		private readonly SerializerService _serializerService;
		private readonly object _lockObject = new object();

		public TmService(SettingsService settingsService, ContentParsingService contentParsingService, SerializerService serializerService, GroupshareCredentialManager groupshareCredentialManager)
		{
            GroupshareCredentialManager = groupshareCredentialManager;
            _settingsService = settingsService;
			_contentParsingService = contentParsingService;
			_serializerService = serializerService;
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
					Equals(a.Source, providerLanguageDirection.Source.Name) && Equals(a.Target, providerLanguageDirection.Target.Name));

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

					context.Report(0, StringResources.Reading_data_from_cache);

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
			context.Report(0, StringResources.Saving_data_to_cache);
			if (string.IsNullOrEmpty(tmFile.CachePath) || !File.Exists(tmFile.CachePath))
			{
				var path = Path.Combine(_settingsService.PathInfo.TemporaryStorageFullPath,
					Path.GetFileName(tmFile.Name) + "." +
					languageDirection.Source + "-" +
					languageDirection.Target + ".xml");

				var index = 0;
				while (File.Exists(path) && index < 1000)
				{
					path = Path.Combine(_settingsService.PathInfo.TemporaryStorageFullPath,
						Path.GetFileName(tmFile.Name) + "." +
						languageDirection.Source + "-" +
						languageDirection.Target + "." +
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
			context.Report(0, StringResources.Parsing_content);

			decimal iTotal = tus.Count;
			decimal iCurrent = 0;

			var results = new List<ContentSearchResult>();
			var rules = _settingsService.GetRules().OrderBy(a => a.Order).ToList();

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
					context.Report(Convert.ToInt32(progress), string.Format(StringResources.Parsing_0_of_1_Translation_Units_2_3,
															iCurrent, iTotal, tu.SourceSegment.Language, tu.TargetSegment.Language));
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
						IconFilePath = tmFile.IsServerTm
							? "../Resources/ServerBasedTranslationMemory.ico"
							: "../Resources/TranslationMemory.ico"
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

		public int AnonymizeFileBasedTm(ProgressDialogContext context, List<AnonymizeTranslationMemory> anonymizeTranslationMemories)
		{
			var updatedCount = 0;

			BackupFileBasedTms(context, anonymizeTranslationMemories.Select(a => a.TmFile).ToList());

			var unitsClone = GetAnonymizeTranslationUnitsClone(anonymizeTranslationMemories);

			PrepareTranslationUnits(context, anonymizeTranslationMemories);

			decimal iCurrent = 0;
			decimal iTotalUnits = 0;
			foreach (var memory in anonymizeTranslationMemories)
			{
				iTotalUnits += memory.TranslationUnitDetails.Count;
			}

			if (iTotalUnits == 0)
			{
				return 0;
			}

			foreach (var memory in anonymizeTranslationMemories)
			{
				var report = new Report(memory.TmFile)
				{
					ReportFullPath = _settingsService.GetLogReportFullPath(memory.TmFile.Name, Report.ReportScope.Content),
					UpdatedCount = memory.TranslationUnits.Count,
					Scope = Report.ReportScope.Content
				};

				var actions = new List<Model.Log.Action>();

				var stopWatch = new Stopwatch();
				stopWatch.Start();

				var tm = new FileBasedTranslationMemory(memory.TmFile.Path);

				var groupsOf = 200;
				var tusGroups = new List<List<TmTranslationUnit>> { new List<TmTranslationUnit>(memory.TranslationUnits) };
				if (memory.TranslationUnits.Count > groupsOf)
				{
					tusGroups = memory.TranslationUnits.ChunkBy(groupsOf);
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
					context?.Report(Convert.ToInt32(progress), string.Format(StringResources.Updating_0_of_1_Translation_Units, iCurrent, iTotalUnits));

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
						var results = tm.LanguageDirection.UpdateTranslationUnits(tusToUpdate.ToArray());
						if (results != null)
						{
							actions.AddRange(GetResultActions(results, unitsClone, tusToUpdate, tus));
							updatedCount += results.Count(a => a.ErrorCode == ErrorCode.OK);
						}
					}
				}

				tm.Save();

				report.Actions.AddRange(actions);

				stopWatch.Stop();
				report.ElapsedSeconds = stopWatch.Elapsed.TotalSeconds;

				_serializerService.Save(report, report.ReportFullPath);
			}

			return updatedCount;
		}

		public int AnonymizeServerBasedTm(ProgressDialogContext context, List<AnonymizeTranslationMemory> anonymizeTranslationMemories)
		{
			var updatedCount = 0;

			BackupServerBasedTms(context, anonymizeTranslationMemories.Select(a => a.TmFile).ToList());

			var unitsClone = GetAnonymizeTranslationUnitsClone(anonymizeTranslationMemories);

			PrepareTranslationUnits(context, anonymizeTranslationMemories);

			decimal iCurrent = 0;
			decimal iTotalUnits = 0;
			foreach (var anonymizeTranslationMemory in anonymizeTranslationMemories)
			{
				iTotalUnits += anonymizeTranslationMemory.TranslationUnitDetails.Count;
			}

			if (iTotalUnits == 0)
			{
				return 0;
			}

			foreach (var memory in anonymizeTranslationMemories)
			{
				var report = new Report(memory.TmFile)
				{
					ReportFullPath = _settingsService.GetLogReportFullPath(memory.TmFile.Name, Report.ReportScope.Content),
					UpdatedCount = memory.TranslationUnits.Count,
					Scope = Report.ReportScope.Content
				};

				var actions = new List<Model.Log.Action>();

				var stopWatch = new Stopwatch();
				stopWatch.Start();

				var uri = new Uri(memory.TmFile.Credentials.Url);
				var translationProvider = GroupshareCredentialManager.TryGetProviderWithoutUserInput(memory.TmFile.Credentials);
				var tm = translationProvider.GetTranslationMemory(memory.TmFile.Path, TranslationMemoryProperties.All);

				var groupsOf = 100;
				var tusGroups = new List<List<TmTranslationUnit>> { new List<TmTranslationUnit>(memory.TranslationUnits) };
				if (memory.TranslationUnits.Count > groupsOf)
				{
					tusGroups = memory.TranslationUnits.ChunkBy(groupsOf);
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
						context?.Report(Convert.ToInt32(progress), string.Format(StringResources.Updating_0_of_1_Translation_Units, iCurrent, iTotalUnits));
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
							var results = languageDirection.UpdateTranslationUnits(tusToUpdate.ToArray());
							if (results != null)
							{
								actions.AddRange(GetResultActions(results, unitsClone, tusToUpdate, tus));
								updatedCount += results.Count(a => a.ErrorCode == ErrorCode.OK);
							}
						}
					}
				}

				tm.Save();

				foreach (var languageDirection in memory.TmFile.TmLanguageDirections)
				{
					SaveTmCacheStorage(context, memory.TmFile, languageDirection);
				}

				report.Actions.AddRange(actions);

				stopWatch.Stop();
				report.ElapsedSeconds = stopWatch.Elapsed.TotalSeconds;

				_serializerService.Save(report, report.ReportFullPath);
			}

			return updatedCount;
		}


        private static IEnumerable<Model.Log.Action> GetResultActions(IReadOnlyList<ImportResult> results, IReadOnlyList<TranslationUnitDetails> unitsClone,
			IReadOnlyList<LanguagePlatform.TranslationMemory.TranslationUnit> unitsUpdated, IReadOnlyCollection<TmTranslationUnit> unitsReference)
		{
			var details = new List<Model.Log.Action>();

			for (var i = 0; i < results.Count; i++)
			{
				// The ResourceIds don't match from the TU used to update the TM against the results received from a Server-based TM.
				// for this reason, we are using a simple index to align the result generation
				// var previousTu = unitsUpdated.FirstOrDefault(a => a.TranslationUnit.ResourceId.Id == result.TuId.Id);

				var updateTu = unitsUpdated[i]; // we need this value to recover the correct resource ID, because of issue mentioned above.
				var previousTu = unitsClone.FirstOrDefault(a => a.TranslationUnit.ResourceId.Id == updateTu.ResourceId.Id);
				var tuReference = unitsReference.FirstOrDefault(a => a.ResourceId.Id == updateTu.ResourceId.Id);

				if (tuReference != null && previousTu != null)
				{
					if (previousTu.IsSourceMatch)
					{
						var detail = new Model.Log.Action
						{
							TmId = updateTu.ResourceId,
							Name = "Source",
							Result = results[i].ErrorCode.ToString(),
							Previous = previousTu.TranslationUnit.SourceSegment.ToPlain(true),
							Value = tuReference.SourceSegment.ToPlain(true),
							Type = "Segment"
						};
						details.Add(detail);
					}

					if (previousTu.IsTargetMatch)
					{
						var detail = new Model.Log.Action
						{
							TmId = updateTu.ResourceId,
							Name = "Target",
							Result = results[i].ErrorCode.ToString(),
							Previous = previousTu.TranslationUnit.TargetSegment.ToPlain(true),
							Value = tuReference.TargetSegment.ToPlain(true),
							Type = "Segment"
						};
						details.Add(detail);
					}
				}
			}

			return details;
		}

		private static List<TranslationUnitDetails> GetAnonymizeTranslationUnitsClone(IEnumerable<AnonymizeTranslationMemory> anonymizeTranslationMemories)
		{
			var unitsClone = new List<TranslationUnitDetails>();

			foreach (var memory in anonymizeTranslationMemories)
			{
				foreach (var unit in memory.TranslationUnitDetails)
				{
					unitsClone.Add(new TranslationUnitDetails
					{
						IsSourceMatch = unit.IsSourceMatch,
						IsTargetMatch = unit.IsTargetMatch,
						TranslationUnit = new TmTranslationUnit
						{
							ResourceId = unit.TranslationUnit.ResourceId,
							SourceSegment = unit.TranslationUnit.SourceSegment.Clone() as TmSegment,
							TargetSegment = unit.TranslationUnit.TargetSegment.Clone() as TmSegment,
						}
					});
				}
			}

			return unitsClone;
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
			if (!fieldValueType.Equals(FieldValueType.DateTime) && !fieldValueType.Equals(FieldValueType.SingleString))
			{
				if (fieldValueType.Equals(FieldValueType.MultipleString))
				{
					var matches = Regex.Matches(trimEnd, "([\"'])(?:(?=(\\\\?))\\2.)*?\\1");
					listValues = matches.Cast<Match>().Select(m => m.Value).ToList();
				}
				else
				{
					listValues = trimEnd.Split(',').ToList();
				}
			}



			foreach (var value in listValues)
			{
				var trimStartValue = value.TrimStart(' ', '"');
				var trimEndValue = trimStartValue.TrimEnd('"');
				multipleStringValues.Add(trimEndValue);
			}

			return multipleStringValues;
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
				context.ProgressBarIsIndeterminate = true;

				foreach (var tm in tmsCollection)
				{
					if (tm == null)
					{
						continue;
					}

					var uri = new Uri(tm.Credentials.Url);
					var translationProvider =
						GroupshareCredentialManager.TryGetProviderWithoutUserInput(tm.Credentials);

					context.Report(0, "Backup " + tm.Path);

					var translationMemory = translationProvider.GetTranslationMemory(tm.Path, TranslationMemoryProperties.All);
					var languageDirections = translationMemory.LanguageDirections;

					foreach (var languageDirection in languageDirections)
					{
						var folderPath = Path.Combine(settings.BackupFullPath, translationMemory.Name,
							languageDirection.TargetLanguageCode);

						if (!Directory.Exists(folderPath))
						{
							Directory.CreateDirectory(folderPath);
						}

						var fileName = translationMemory.Name + languageDirection.TargetLanguageCode + "." +
									   _settingsService.GetDateTimeToString() + ".tmx.gz";
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
								MessageBox.Show(tmExporter.ErrorMessage, PluginResources.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
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
				if (exception.Message.Equals(StringResources.TmService_BackupServerBasedTms_One_or_more_errors_occurred_) && exception.InnerException != null)
				{
					MessageBox.Show(exception.InnerException.Message, PluginResources.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
				}
				else
				{
					MessageBox.Show(exception.Message, PluginResources.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
				}
			}
			finally
			{
				context.ProgressBarIsIndeterminate = false;
			}
		}

		public void BackupFileBasedTms(ProgressDialogContext context, IEnumerable<TmFile> tmsCollection)
		{
			var settings = _settingsService.GetSettings();
			if (!settings.Backup)
			{
				return;
			}
			try
			{
				context.ProgressBarIsIndeterminate = true;

				foreach (var tm in tmsCollection)
				{
					if (tm == null)
					{
						continue;
					}

					context.Report(0, string.Format(StringResources.Backup_0, tm.Path));

					var tmInfo = new FileInfo(tm.Path);

					var extension = Path.GetExtension(tm.Name);
					var tmName = tm.Name;
					if (extension?.Length > 0)
					{
						tmName = tmName.Substring(0, tmName.Length - extension.Length);
					}

					var backupFilePath = Path.Combine(settings.BackupFullPath,
						tmName + "." + _settingsService.GetDateTimeToString() + extension);

					Dispatcher.CurrentDispatcher.Invoke(DispatcherPriority.ContextIdle, new System.Action(
						delegate { tmInfo.CopyTo(backupFilePath, true); }));
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message, PluginResources.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
			finally
			{
				context.ProgressBarIsIndeterminate = false;
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
					if (tm.SourceLangauge == tmFileTmLanguageDirection.Source &&
						tm.TargetLanguage == tmFileTmLanguageDirection.Target)
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

		private void PrepareTranslationUnits(ProgressDialogContext context, IReadOnlyCollection<AnonymizeTranslationMemory> anonymizeTranslationMemories)
		{
			decimal iCurrent = 0;
			decimal iTotalUnits = 0;
			foreach (var memory in anonymizeTranslationMemories)
			{
				iTotalUnits += memory.TranslationUnitDetails.Count;
			}

			if (iTotalUnits == 0)
			{
				return;
			}

			var rules = _settingsService.GetRules();

			foreach (var memory in anonymizeTranslationMemories)
			{
				foreach (var details in memory.TranslationUnitDetails)
				{
					iCurrent++;

					if (iCurrent % 100 == 0)
					{
						if (context != null && context.CheckCancellationPending())
						{
							break;
						}

						var progress = iCurrent / iTotalUnits * 100;
						context?.Report(Convert.ToInt32(progress), string.Format(StringResources.Preparing_0_of_1_Translation_Units, iCurrent, iTotalUnits));
					}

					AnonymizeTranslationUnit(details, rules);
				}
			}
		}

		private static void AnonymizeTranslationUnit(TranslationUnitDetails details, List<Rule> rules)
		{
			var anchors = GetTranslationUnitAnchors(details.TranslationUnit);

			//check if there are selected words from the ui
			if (details.SelectedWordsDetails.Any() || details.SelectedWordsDetails.Any())
			{
				if (details.SelectedWordsDetails.Any())
				{
					AnonymizeSelectedWordsFromPreview(details, details.TranslationUnit.SourceSegment.Elements.ToList(), true, anchors);
				}

				if (details.TargetSelectedWordsDetails.Any())
				{
					AnonymizeSelectedWordsFromPreview(details, details.TranslationUnit.TargetSegment.Elements.ToList(), false, anchors);
				}

				anchors = GetTranslationUnitAnchors(details.TranslationUnit);
			}


			if (details.IsSourceMatch)
			{
				AnonymizeSegment(details, details.TranslationUnit.SourceSegment.Elements.ToList(), rules, true, anchors);
			}

			if (details.IsTargetMatch)
			{
				AnonymizeSegment(details, details.TranslationUnit.TargetSegment.Elements.ToList(), rules, false, anchors);
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
				context?.Report(Convert.ToInt32(progress), string.Format(StringResources.Reading_0_of_1_Translation_Units, iCurrent, iTotalUnits));

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

		private static IEnumerable<TmTranslationUnit> AddTranslationUnits(IEnumerable<LanguagePlatform.TranslationMemory.TranslationUnit> units)
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

		private static void AnonymizeSegment(TranslationUnitDetails tuDetails, IEnumerable<SegmentElement> elements, List<Rule> rules, bool isSource, IReadOnlyList<int> anchors)
		{
			var elementsContainsTag = elements.Any(s => s.GetType().UnderlyingSystemType.Name.Equals("Tag"));

			if (elementsContainsTag)
			{
				AnonymizeSegmentsWithTags(tuDetails, rules, isSource, anchors);
			}
			else
			{
				AnonymizeSegmentsWithoutTags(tuDetails, rules, isSource, anchors);
			}
		}

		private static void AnonymizeSelectedWordsFromPreview(TranslationUnitDetails translationUnitDetails, IEnumerable<SegmentElement> translationElements, bool isSource, IReadOnlyList<int> anchors)
		{
			if (isSource)
			{
				AnonymizeSelectedWordsFromPreview(translationUnitDetails.TranslationUnit.SourceSegment, translationElements,
					translationUnitDetails.SelectedWordsDetails, anchors);
			}
			else
			{
				AnonymizeSelectedWordsFromPreview(translationUnitDetails.TranslationUnit.TargetSegment, translationElements,
					translationUnitDetails.TargetSelectedWordsDetails, anchors);
			}
		}

		private static void AnonymizeSelectedWordsFromPreview(TmSegment segment, IEnumerable<SegmentElement> translationElements, List<WordDetails> selectedWords, IReadOnlyList<int> anchors)
		{
			segment.Elements.Clear();

			foreach (var element in translationElements.ToList())
			{
				var visitor = new SelectedWordsVisitorService(selectedWords, anchors);
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

		private static void AnonymizeSegmentsWithoutTags(TranslationUnitDetails translationUnitDetails, List<Rule> rules, bool isSource, IReadOnlyList<int> anchors)
		{
			if (isSource)
			{
				AnonymizeSegmentsWithoutTags(translationUnitDetails.TranslationUnit.SourceSegment, rules, translationUnitDetails.RemovedWordsFromMatches, anchors);
			}
			else
			{
				AnonymizeSegmentsWithoutTags(translationUnitDetails.TranslationUnit.TargetSegment, rules, translationUnitDetails.TargetRemovedWordsFromMatches, anchors);
			}
		}

		private static void AnonymizeSegmentsWithoutTags(TmSegment segment, List<Rule> rules, List<WordDetails> removeWords, IReadOnlyList<int> anchors)
		{
			var segmentElements = new List<SegmentElement>();
			foreach (var element in segment.Elements.ToList())
			{
				var visitor = new SegmentElementVisitorService(removeWords, anchors, rules);

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

		private static void AnonymizeSegmentsWithTags(TranslationUnitDetails translationUnitDetails, List<Rule> rules, bool isSource, IReadOnlyList<int> anchors)
		{
			if (isSource)
			{
				AnonymizeSegmentsWithTags(translationUnitDetails.TranslationUnit.SourceSegment, rules, translationUnitDetails.RemovedWordsFromMatches, anchors);
			}
			else
			{
				AnonymizeSegmentsWithTags(translationUnitDetails.TranslationUnit.TargetSegment, rules, translationUnitDetails.TargetRemovedWordsFromMatches, anchors);
			}
		}

		private static void AnonymizeSegmentsWithTags(TmSegment segment, List<Rule> rules, List<WordDetails> removeWords, IReadOnlyList<int> anchors)
		{
			for (var i = 0; i < segment.Elements.Count; i++)
			{
				if (!segment.Elements[i].GetType().UnderlyingSystemType.Name.Equals("Text"))
				{
					continue;
				}

				var visitor = new SegmentElementVisitorService(removeWords, anchors, rules);

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

		private static List<int> GetTranslationUnitAnchors(TmTranslationUnit unit)
		{
			var anchors = GetSegmentAnchors(unit.SourceSegment);
			foreach (var anchor in GetSegmentAnchors(unit.TargetSegment))
			{
				if (!anchors.Contains(anchor))
				{
					anchors.Add(anchor);
				}
			}

			return anchors;
		}

		private static List<int> GetSegmentAnchors(TmSegment segment)
		{
			var anchors = new List<int>();
			foreach (var element in segment.Elements)
			{
				if (element is Tag tag)
				{
					if (!anchors.Contains(tag.AlignmentAnchor))
					{
						anchors.Add(tag.AlignmentAnchor);
					}

					if (!anchors.Contains(tag.Anchor))
					{
						anchors.Add(tag.Anchor);
					}

					try
					{
						if (int.TryParse(tag.TagID, out var tagId) && !anchors.Contains(tagId))
						{
							anchors.Add(tagId);
						}
					}
					catch
					{
						// ignored
					}
				}
			}

			return anchors;
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

