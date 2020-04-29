using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Sdl.Community.SdlTmAnonymizer.Controls.ProgressDialog;
using Sdl.Community.SdlTmAnonymizer.Extensions;
using Sdl.Community.SdlTmAnonymizer.Model;
using Sdl.Community.SdlTmAnonymizer.Model.Log;
using Sdl.LanguagePlatform.TranslationMemory;
using Sdl.LanguagePlatform.TranslationMemoryApi;

namespace Sdl.Community.SdlTmAnonymizer.Services
{
	public class SystemFieldsService
	{
		private readonly TmService _tmService;
		private readonly SettingsService _settingsService;

		public SystemFieldsService(TmService tmService, SettingsService settingsService)
		{
			_tmService = tmService;
			_settingsService = settingsService;
		}

		public List<User> GetUniqueFileBasedSystemFields(ProgressDialogContext context, TmFile tm)
		{
			var translationMemory = new FileBasedTranslationMemory(tm.Path);

			var tus = _tmService.LoadTranslationUnits(context, tm, null, new LanguageDirection
			{
				Source = translationMemory.LanguageDirection.SourceLanguage,
				Target = translationMemory.LanguageDirection.TargetLanguage
			});

			if (tus != null)
			{
				var uniqueUsersCollection = GetUniqueUserCollection(tm.Path, tus);

				return uniqueUsersCollection;
			}

			return null;
		}

		public List<User> GetUniqueServerBasedSystemFields(ProgressDialogContext context, TmFile tm, TranslationProviderServer translationProvideServer)
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

			var uniqueUsersCollection = GetUniqueUserCollection(tm.Path, translationUnits.ToArray());

			return uniqueUsersCollection;
		}

		public Report AnonymizeFileBasedSystemFields(ProgressDialogContext context, TmFile tmFile, List<User> uniqueUsers)
		{
			_tmService.BackupFileBasedTms(context, new List<TmFile> { tmFile });

			var tm = new FileBasedTranslationMemory(tmFile.Path);

			var translationUnits = _tmService.LoadTranslationUnits(context, tmFile, null, new LanguageDirection
			{
				Source = tm.LanguageDirection.SourceLanguage,
				Target = tm.LanguageDirection.TargetLanguage
			});

			var report = new Report(tmFile)
			{
				ReportFullPath = _settingsService.GetLogReportFullPath(tmFile.Name, Report.ReportScope.SystemFields),
				UpdatedCount = translationUnits.Count,
				Scope = Report.ReportScope.SystemFields,
			};

			var stopWatch = new Stopwatch();
			stopWatch.Start();

			report.Actions.AddRange(GetSystemFieldChangesReport(uniqueUsers));

			var settings = _settingsService.GetSettings();
			report.UpdatedCount = settings.UseSqliteApiForFileBasedTm
				? UpdateSystemFieldsSqlite(context, tmFile, translationUnits, uniqueUsers)
				: UpdateSystemFields(context, translationUnits, tm, uniqueUsers);

			stopWatch.Stop();
			report.ElapsedSeconds = stopWatch.Elapsed.TotalSeconds;

			return report;
		}

		public Report AnonymizeServerBasedSystemFields(ProgressDialogContext context, TmFile tmFile, List<User> uniqueUsers, TranslationProviderServer translationProvideServer)
		{
			_tmService.BackupServerBasedTms(context, new List<TmFile> { tmFile });

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

			var report = new Report(tmFile)
			{
				ReportFullPath = _settingsService.GetLogReportFullPath(tmFile.Name, Report.ReportScope.SystemFields),
				UpdatedCount = translationUnits.Count,
				Scope = Report.ReportScope.SystemFields,
			};


			report.Actions.AddRange(GetSystemFieldChangesReport(uniqueUsers));

			var stopWatch = new Stopwatch();
			stopWatch.Start();

			report.UpdatedCount = UpdateSystemFields(context, tmFile, uniqueUsers, translationUnits, serverBasedTm);

			stopWatch.Stop();
			report.ElapsedSeconds = stopWatch.Elapsed.TotalSeconds;

			return report;
		}

		private int UpdateSystemFields(ProgressDialogContext context, TmFile tmFile, List<User> uniqueUsers,
			List<TmTranslationUnit> translationUnits, ServerBasedTranslationMemory serverBasedTm)
		{
			var updatedCount = 0;
			decimal iCurrent = 0;
			decimal iTotalUnits = translationUnits.Count;
			var groupsOf = 100;

			var tusGroups = new List<List<TmTranslationUnit>> { new List<TmTranslationUnit>(translationUnits) };
			if (translationUnits.Count > groupsOf)
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

				var filteredTusToUpdate = new List<TmTranslationUnit>();
				foreach (var tu in tus)
				{
					if (UpdateSystemFields(uniqueUsers, tu))
					{
						filteredTusToUpdate.Add(tu);
					}					
				}

				if (filteredTusToUpdate.Count > 0)
				{
					foreach (var languageDirection in serverBasedTm.LanguageDirections)
					{
						var tusToUpdate = new List<TranslationUnit>();
						foreach (var tu in filteredTusToUpdate)
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
							var results = languageDirection.UpdateTranslationUnits(tusToUpdate.ToArray());
							updatedCount += results.Count(result => result.Action != LanguagePlatform.TranslationMemory.Action.Error);
						}
					}
				}
			}

			serverBasedTm.Save();

			foreach (var languageDirection in tmFile.TmLanguageDirections)
			{
				_tmService.SaveTmCacheStorage(context, tmFile, languageDirection);
			}

			return updatedCount;
		}

		private static int UpdateSystemFieldsSqlite(ProgressDialogContext context, TmFile tmFile, IEnumerable<TmTranslationUnit> units, List<User> uniqueUsers)
		{
			var updatedCount = 0;

			var service = new SqliteTmService(tmFile.Path, null, new SerializerService(), new SegmentService());

			try
			{
				service.OpenConnection();

				var updateList = units.Where(unit => UpdateSystemFields(uniqueUsers, unit)).ToList();

				if (updateList.Count > 0)
				{
					updatedCount = service.UpdateSystemFields(context, updateList);
				}
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

			return updatedCount;
		}

		private int UpdateSystemFields(ProgressDialogContext context, List<TmTranslationUnit> translationUnits, IFileBasedTranslationMemory tm, List<User> uniqueUsers)
		{
			var updatedCount = 0;

			decimal iCurrent = 0;
			decimal iTotalUnits = translationUnits.Count;
			var groupsOf = 200;

			var tusGroups = new List<List<TmTranslationUnit>> { new List<TmTranslationUnit>(translationUnits) };
			if (translationUnits.Count > groupsOf)
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

				var filteredTusToUpdate = new List<TmTranslationUnit>();
				foreach (var tu in tus)
				{
					if (UpdateSystemFields(uniqueUsers, tu))
					{
						filteredTusToUpdate.Add(tu);
					}
				}

				if (filteredTusToUpdate.Count > 0)
				{
					var tusToUpdate = new List<TranslationUnit>();
					foreach (var tu in filteredTusToUpdate)
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
						var results = tm.LanguageDirection.UpdateTranslationUnits(tusToUpdate.ToArray());
						updatedCount += results.Count(result => result.Action != LanguagePlatform.TranslationMemory.Action.Error);
					}
				}
			}

			tm.Save();

			return updatedCount;
		}

		private static bool UpdateSystemFields(IEnumerable<User> uniqueUsers, TmTranslationUnit tu)
		{
			var updateSystemFields = false;

			foreach (var userName in uniqueUsers)
			{
				var updateCreationUser = false;
				var updateChangeUser = false;
				var updateUseUser = false;

				if (userName.IsSelected && !string.IsNullOrEmpty(userName.Alias))
				{
					var systemFields = tu.SystemFields;

					if (!string.IsNullOrEmpty(systemFields.CreationUser) &&
						userName.UserName == systemFields.CreationUser)
					{
						updateCreationUser = true;
					}

					if (!string.IsNullOrEmpty(systemFields.ChangeUser) &&
						userName.UserName == systemFields.ChangeUser)
					{
						updateChangeUser = true;
					}

					if (!string.IsNullOrEmpty(systemFields.UseUser) &&
						userName.UserName == systemFields.UseUser)
					{
						updateUseUser = true;
					}

					if (updateCreationUser || updateChangeUser || updateUseUser)
					{
						updateSystemFields = true;

						if (updateCreationUser)
						{
							systemFields.CreationUser = userName.Alias;
						}

						if (updateChangeUser)
						{
							systemFields.ChangeUser = userName.Alias;
						}

						if (updateUseUser)
						{
							systemFields.UseUser = userName.Alias;
						}
					}
				}
			}

			return updateSystemFields;
		}

		private static IEnumerable<Model.Log.Action> GetSystemFieldChangesReport(IEnumerable<User> uniqueUsers)
		{
			var details = new List<Model.Log.Action>();

			foreach (var userName in uniqueUsers)
			{
				if (userName.IsSelected && !string.IsNullOrEmpty(userName.Alias))
				{
					var detailCreationUser = details.FirstOrDefault(a => a.Value == userName.Alias && a.Previous == userName.UserName);
					if (detailCreationUser == null)
					{
						var detail = new Model.Log.Action
						{
							Name = "UserName",
							Type = "SystemField",
							Previous = userName.UserName,
							Value = userName.Alias
						};
						details.Add(detail);
					}
				}
			}

			return details;
		}

		private static List<User> GetUniqueUserCollection(string tmFilePath, IEnumerable<TmTranslationUnit> translationUnits)
		{
			var systemFields = new List<string>();
			var distinctUsersCollection = new List<User>();
			foreach (var tu in translationUnits)
			{
				systemFields.AddRange(new List<string> {
					tu.SystemFields.CreationUser,
					tu.SystemFields.UseUser });
			}

			foreach (var name in systemFields.Distinct().ToList())
			{
				distinctUsersCollection.Add(new User
				{
					UserName = name,
					TmFilePath = tmFilePath
				});
			}
			return distinctUsersCollection;
		}
	}
}
