using System;
using System.Collections.Generic;
using System.Linq;
using Sdl.Community.SdlTmAnonymizer.Controls.ProgressDialog;
using Sdl.Community.SdlTmAnonymizer.Model;
using Sdl.LanguagePlatform.TranslationMemory;
using Sdl.LanguagePlatform.TranslationMemoryApi;

namespace Sdl.Community.SdlTmAnonymizer.Services
{
	public class SystemFieldsService
	{
		public List<User> GetUniqueFileBasedSystemFields(ProgressDialogContext context, TmFile tm, TmService tmService)
		{
			var translationMemory = new FileBasedTranslationMemory(tm.Path);

			var tus = tmService.LoadTranslationUnits(context, tm, null, new LanguageDirection
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

		public List<User> GetUniqueServerBasedSystemFields(ProgressDialogContext context, TmFile tm, TranslationProviderServer translationProvideServer, TmService tmService)
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

			var uniqueUsersCollection = GetUniqueUserCollection(tm.Path, translationUnits.ToArray());

			return uniqueUsersCollection;
		}

		public void AnonymizeFileBasedSystemFields(ProgressDialogContext context, TmFile tm, List<User> uniqueUsers, TmService tmService)
		{
			var translationMemory = new FileBasedTranslationMemory(tm.Path);

			var translationUnits = tmService.LoadTranslationUnits(context, tm, null, new LanguageDirection
			{
				Source = translationMemory.LanguageDirection.SourceLanguage,
				Target = translationMemory.LanguageDirection.TargetLanguage
			});

			decimal iCurrent = 0;
			decimal iTotalUnits = translationUnits.Length;
			foreach (var tu in translationUnits)
			{
				iCurrent++;
				var progress = iCurrent / iTotalUnits * 100;
				context.Report(Convert.ToInt32(progress), "Updating: " + iCurrent + " of " + iTotalUnits + " Translation Units");

				var updateSystemFields = false;

				foreach (var userName in uniqueUsers)
				{
					var updateCreationUser = false;
					//var updateChangeUser = false;
					var updateUseUser = false;

					if (userName.IsSelected && !string.IsNullOrEmpty(userName.Alias))
					{
						var systemFields = tu.SystemFields;

						if (!string.IsNullOrEmpty(systemFields.CreationUser) &&
						    userName.UserName == systemFields.CreationUser)
						{
							updateCreationUser = true;
						}

						//if (!string.IsNullOrEmpty(systemFields.ChangeUser) &&
						//    userName.UserName == systemFields.ChangeUser)
						//{
						//	updateChangeUser = true;
						//}

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

							//if (updateChangeUser)
							//{
							//	systemFields.ChangeUser = userName.Alias;
							//}

							if (updateUseUser)
							{
								systemFields.UseUser = userName.Alias;
							}
						}
					}
				}

				if (updateSystemFields)
				{
					translationMemory.LanguageDirection.UpdateTranslationUnit(tu);
				}
				
			}
		}
	
		public void AnonymizeServerBasedSystemFields(ProgressDialogContext context, TmFile tm, List<User> uniqueUsers, TranslationProviderServer translationProvideServer, TmService tmService)
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


			decimal iCurrent = 0;
			decimal iTotalUnits = translationUnits.Length;
			foreach (var tu in translationUnits)
			{
				iCurrent++;
				if (context.CheckCancellationPending())
				{
					break;
				}

				var progress = iCurrent / iTotalUnits * 100;
				context.Report(Convert.ToInt32(progress), "Updating: " + iCurrent + " of " + iTotalUnits + " Translation Units");

				var updateSystemFields = false;
				
				foreach (var userName in uniqueUsers)
				{
					var updateCreationUser = false;
					//var updateChangeUser = false;
					var updateUseUser = false;

					if (userName.IsSelected && !string.IsNullOrEmpty(userName.Alias))
					{										
						var systemFields = tu.SystemFields;

						if (!string.IsNullOrEmpty(systemFields.CreationUser) &&
						    userName.UserName == systemFields.CreationUser)
						{
							updateCreationUser = true;
						}

						//if (!string.IsNullOrEmpty(systemFields.ChangeUser) &&
						//    userName.UserName == systemFields.ChangeUser)
						//{
						//	updateChangeUser = true;
						//}

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

							//if (updateChangeUser)
							//{
							//	systemFields.ChangeUser = userName.Alias;
							//}

							if (updateUseUser)
							{
								systemFields.UseUser = userName.Alias;
							}							
						}
					}
				}

				if (updateSystemFields)
				{
					foreach (var languageDirection in serverBasedTm.LanguageDirections)
					{
						if (languageDirection.SourceLanguage.Equals(tu.SourceSegment.Culture) &&
						    languageDirection.TargetLanguage.Equals(tu.TargetSegment.Culture))
						{
							languageDirection.UpdateTranslationUnit(tu);
						}
					}
				}
			}		
		}

		private static List<User> GetUniqueUserCollection(string tmFilePath, IEnumerable<TranslationUnit> translationUnits)
		{
			var systemFields = new List<string>();
			var distinctUsersCollection = new List<User>();
			foreach (var tu in translationUnits)
			{
				systemFields.AddRange(new List<string> {
					tu.SystemFields.CreationUser,
					//tu.SystemFields.ChangeUser,
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
