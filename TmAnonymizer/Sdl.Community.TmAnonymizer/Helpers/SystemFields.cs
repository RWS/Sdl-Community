using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Sdl.Community.SdlTmAnonymizer.Model;
using Sdl.LanguagePlatform.TranslationMemory;
using Sdl.LanguagePlatform.TranslationMemoryApi;

namespace Sdl.Community.SdlTmAnonymizer.Helpers
{
	public static class SystemFields
	{
		/// <summary>
		/// Gets unique System Fields values from File Based Translation Memory
		/// </summary>
		/// <param name="tm">Translation Memory File</param>
		/// <returns>An ObservableCollection of Users</returns>
		public static ObservableCollection<User> GetUniqueFileBasedSystemFields(TmFile tm)
		{
			var users = new List<User>();
			var listOfFieldNames = new List<string>();
			var translationUnits = GetFileBasedTranslationUnits(tm);
			var uniqueUsersCollection = GetuniqueUserCollection(users, listOfFieldNames, translationUnits);
			return uniqueUsersCollection;

		}

		/// <summary>
		/// Gets unique System Fields values from Server Based Translation Memory
		/// </summary>
		/// <param name="tm">Translation Memory File</param>
		/// <param name="translationProvideServer">Translation provider</param>
		/// <returns>An ObservableCollection of UniqueUserName objects</returns>
		/// TODO: SIMPLIFY METHOD
		public static ObservableCollection<User> GetUniqueServerBasedSystemFields(TmFile tm, TranslationProviderServer translationProvideServer)
		{
			var users = new List<User>();
			var listOfFieldNames = new List<string>();

			var translationMemory = translationProvideServer.GetTranslationMemory(tm.Path, TranslationMemoryProperties.All);
			var translationUnits = GetServerBasedTranslationUnits(translationMemory.LanguageDirections);
			var uniqueUsersCollection = GetuniqueUserCollection(users, listOfFieldNames, translationUnits);
			return uniqueUsersCollection;
		}



		/// <summary>
		/// Anonymizez each unique name from the UniqueUserNames list found in a specific File Based Translation Memory
		/// </summary>
		/// <param name="tm">Translation Memory File</param>
		/// <param name="uniqueUsers">List of UniqueUserName objects</param>
		public static void AnonymizeFileBasedSystemFields(TmFile tm, List<User> uniqueUsers)
		{
			var fileBasedTm = new FileBasedTranslationMemory(tm.Path);
			var translationUnits = GetFileBasedTranslationUnits(tm);
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
							fileBasedTm.LanguageDirection.UpdateTranslationUnit(tu);
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
		public static void AnonymizeServerBasedSystemFields(TmFile tm, List<User> uniqueUsers, TranslationProviderServer translationProvideServer)
		{
			var serverBasedTm = translationProvideServer.GetTranslationMemory(tm.Path, TranslationMemoryProperties.All);
			var languageDirections = serverBasedTm.LanguageDirections;
			var translationUnits = GetServerBasedTranslationUnits(serverBasedTm.LanguageDirections);
			
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
							foreach (var languageDirection in languageDirections)
							{
								languageDirection.UpdateTranslationUnit(tu);
							}
						}
					}
				}
			}
		}

		/// <summary>
		/// Retrieves an array of Translation Units for a File Based Translation Memory
		/// </summary>
		/// <param name="tm">Translation Memory File</param>
		/// /// <returns>Array of TranslationUnits</returns>
		private static TranslationUnit[]  GetFileBasedTranslationUnits(TmFile tm)
		{
			var fileBasedTm = new FileBasedTranslationMemory(tm.Path);
			var unitsCount = fileBasedTm.LanguageDirection.GetTranslationUnitCount();
			var tmIterator = new RegularIterator(unitsCount);
			var translationUnits = fileBasedTm.LanguageDirection.GetTranslationUnits(ref tmIterator);
			return translationUnits;
		}

		/// <summary>
		/// Retrieves an array of Translation Units for a Server Based Translation Memory
		/// </summary>
		/// <param name="languageDirections">Language Directions of a Server based Translation Memory</param>
		/// /// <returns>Array of TranslationUnits</returns>
		private static TranslationUnit[] GetServerBasedTranslationUnits(ServerBasedTranslationMemoryLanguageDirectionCollection languageDirections)
		{
			var translationUnits = new TranslationUnit[] { };

			foreach (var languageDirection in languageDirections)
			{
				var unitsCount = languageDirection.GetTranslationUnitCount();
				if (unitsCount == 0) continue;
				var tmIterator = new RegularIterator(unitsCount);
				translationUnits = languageDirection.GetTranslationUnits(ref tmIterator);
			}
			return translationUnits;
		}

		private static ObservableCollection<User> GetuniqueUserCollection(List<User> users, List<string> listOfFieldNames, TranslationUnit[] translationUnits)
		{
			foreach (var tu in translationUnits)
			{
				listOfFieldNames.AddRange(new List<string>() { tu.SystemFields.CreationUser, tu.SystemFields.UseUser });
			}
			var distinctUsers = listOfFieldNames.Distinct().ToList();
			foreach (var name in distinctUsers)
			{
				users.Add(new User() { UserName = name });
			}
			var uniqueUsersCollection = new ObservableCollection<User>(users);
			return uniqueUsersCollection;
		}
	}
}
