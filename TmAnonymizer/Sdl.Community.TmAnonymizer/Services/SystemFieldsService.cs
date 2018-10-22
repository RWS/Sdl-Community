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

		public void AnonymizeFileBasedSystemFields(ProgressDialogContext context, TmFile tmFile, List<User> uniqueUsers)
		{
			var tm = new FileBasedTranslationMemory(tmFile.Path);

			var translationUnits = _tmService.LoadTranslationUnits(context, tmFile, null, new LanguageDirection
			{
				Source = tm.LanguageDirection.SourceLanguage,
				Target = tm.LanguageDirection.TargetLanguage
			});

			_tmService.BackupFileBasedTms(context, new List<TmFile> { tmFile });

			var settings = _settingsService.GetSettings();
			if (settings.UseSqliteApiForFileBasedTm)
			{
				UpdateSystemFieldsSqlite(context, tmFile, translationUnits, uniqueUsers);
				return;
			}

			UpdateSystemFields(context, translationUnits, tm, uniqueUsers);
		}

		public void AnonymizeServerBasedSystemFields(ProgressDialogContext context, TmFile tmFile, List<User> uniqueUsers, TranslationProviderServer translationProvideServer)
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

			_tmService.BackupServerBasedTm(context, new List<TmFile> { tmFile });

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

				var updateSystemFields = false;

				foreach (var tu in tus)
				{
					foreach (var userName in uniqueUsers)
					{
						var updateCreationUser = false;
						var updateUseUser = false;

						if (userName.IsSelected && !string.IsNullOrEmpty(userName.Alias))
						{
							var systemFields = tu.SystemFields;

							if (!string.IsNullOrEmpty(systemFields.CreationUser) &&
								userName.UserName == systemFields.CreationUser)
							{
								updateCreationUser = true;
							}

							if (!string.IsNullOrEmpty(systemFields.UseUser) &&
								userName.UserName == systemFields.UseUser)
							{
								updateUseUser = true;
							}

							if (updateCreationUser || updateUseUser)
							{
								updateSystemFields = true;

								if (updateCreationUser)
								{
									systemFields.CreationUser = userName.Alias;
								}

								if (updateUseUser)
								{
									systemFields.UseUser = userName.Alias;
								}
							}
						}
					}
				}

				if (updateSystemFields)
				{
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
			}

			serverBasedTm.Save();

			foreach (var languageDirection in tmFile.TmLanguageDirections)
			{
				_tmService.SaveTmCacheStorage(context, tmFile, languageDirection);
			}
		}

		private static void UpdateSystemFieldsSqlite(ProgressDialogContext context, TmFile tmFile, IEnumerable<TmTranslationUnit> units, List<User> uniqueUsers)
		{
			var service = new SqliteTmService(tmFile.Path, null, new SerializerService(), new SegmentService());

			try
			{
				service.OpenConnection();

				var updateList = new List<TmTranslationUnit>();
				foreach (var unit in units)
				{
					if (UpdateSystemFields(uniqueUsers, unit))
					{
						updateList.Add(unit);
					}
				}

				if (updateList.Count > 0)
				{
					service.UpdateSystemFields(context, updateList);
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
		}

		private void UpdateSystemFields(ProgressDialogContext context, List<TmTranslationUnit> translationUnits, IFileBasedTranslationMemory tm, List<User> uniqueUsers)
		{
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

				var updateSystemFields = false;

				foreach (var tu in tus)
				{
					updateSystemFields = UpdateSystemFields(uniqueUsers, tu);
				}

				if (updateSystemFields)
				{
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

			tm.Save();
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
