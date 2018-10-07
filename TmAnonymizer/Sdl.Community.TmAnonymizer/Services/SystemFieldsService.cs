using System.Collections.Generic;
using System.IO;
using System.Linq;
using Sdl.Community.SdlTmAnonymizer.Model;
using Sdl.LanguagePlatform.TranslationMemory;
using Sdl.LanguagePlatform.TranslationMemoryApi;

namespace Sdl.Community.SdlTmAnonymizer.Services
{
	public class SystemFieldsService
	{	
		public List<User> GetUniqueFileBasedSystemFields(TmFile tm, TmService tmService)
		{
			var translationMemory = new FileBasedTranslationMemory(tm.Path);

			var tus = tmService.LoadTranslationUnits(tm, null, new LanguageDirection
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

	
		public List<User> GetUniqueServerBasedSystemFields(TmFile tm, TranslationProviderServer translationProvideServer, TmService tmService)
		{
			var translationMemory = translationProvideServer.GetTranslationMemory(tm.Path, TranslationMemoryProperties.All);

			var translationUnits = new List<TranslationUnit>();

			foreach (var languageDirection in translationMemory.LanguageDirections)
			{
				var tus = tmService.LoadTranslationUnits(tm, translationProvideServer, new LanguageDirection
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

		public void AnonymizeFileBasedSystemFields(TmFile tm, List<User> uniqueUsers, TmService tmService)
		{
			var translationMemory = new FileBasedTranslationMemory(tm.Path);

			var translationUnits = tmService.LoadTranslationUnits(tm, null, new LanguageDirection
			{
				Source = translationMemory.LanguageDirection.SourceLanguage,
				Target = translationMemory.LanguageDirection.TargetLanguage
			});

			foreach (var userName in uniqueUsers)
			{
				if (userName.IsSelected && !string.IsNullOrEmpty(userName.Alias))
				{
					foreach (var tu in translationUnits)
					{
						if (userName.UserName == tu.SystemFields.CreationUser || userName.UserName == tu.SystemFields.UseUser)
						{
							tu.SystemFields.CreationUser = userName.Alias;
							tu.SystemFields.UseUser = userName.Alias;
							translationMemory.LanguageDirection.UpdateTranslationUnit(tu);
						}
					}
				}
			}
		}

		/// <summary>
		/// Anonymizez each unique name from the UniqueUserNames list found in a specific Server Based Translation Memory
		/// </summary>
		/// <param name="tm">Translation Memory File</param>
		/// <param name="uniqueUsers">List of UniqueUserName objects</param>
		/// /// <param name="translationProvideServer">Translation provider</param>
		public void AnonymizeServerBasedSystemFields(TmFile tm, List<User> uniqueUsers, TranslationProviderServer translationProvideServer, TmService tmService)
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
			var translationUnits = tmService.LoadTranslationUnits(tm, translationProvideServer, languageDirections);
			
			foreach (var userName in uniqueUsers)
			{
				if (userName.IsSelected && !string.IsNullOrEmpty(userName.Alias))
				{
					foreach (var tu in translationUnits)
					{
						if (userName.UserName == tu.SystemFields.CreationUser || userName.UserName == tu.SystemFields.UseUser)
						{
							tu.SystemFields.CreationUser = userName.Alias;
							tu.SystemFields.UseUser = userName.Alias;
							foreach (var languageDirection in serverBasedTm.LanguageDirections)
							{
								languageDirection.UpdateTranslationUnit(tu);
							}
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
				systemFields.AddRange(new List<string> { tu.SystemFields.CreationUser, tu.SystemFields.UseUser });
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
