using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sdl.Community.TmAnonymizer.Model;
using Sdl.LanguagePlatform.TranslationMemory;
using Sdl.LanguagePlatform.TranslationMemoryApi;

namespace Sdl.Community.TmAnonymizer.Helpers
{
	public static class SystemFields
	{
		public static ObservableCollection<UniqueUsername> GetUniqueFileBasedSystemFields(TmFile tm)
		{
			var uniqueUsernames = new List<UniqueUsername>();
			var listOfFieldNames = new List<string>();
			var tus = GetFileBasedTranslationUnits(tm);
			foreach (var tu in tus)
			{
				listOfFieldNames.AddRange(new List<string>() { tu.SystemFields.CreationUser, tu.SystemFields.UseUser });
			}
			var uniqueUsers = listOfFieldNames.Distinct().ToList();
			foreach (var name in uniqueUsers)
			{
				uniqueUsernames.Add(new UniqueUsername() { Username = name });
			}
			var observableCollection = new ObservableCollection<UniqueUsername>(uniqueUsernames);
			return observableCollection;

		}
		public static ObservableCollection<UniqueUsername> GetUniqueServerBasedSystemFields(TmFile tm, TranslationProviderServer translationProvideServer)
		{
			var uniqueUsernames = new List<UniqueUsername>();
			var listOfFieldNames = new List<string>();

			var translationMemory = translationProvideServer.GetTranslationMemory(tm.Path, TranslationMemoryProperties.All);
			var translationUnits = GetServerBasedTranslationUnits(translationMemory.LanguageDirections);

			foreach (var tu in translationUnits)
			{
				listOfFieldNames.AddRange(new List<string>() { tu.SystemFields.CreationUser, tu.SystemFields.UseUser });
			}
			var uniqueUsers = listOfFieldNames.Distinct().ToList();
			foreach (var name in uniqueUsers)
			{
				uniqueUsernames.Add(new UniqueUsername() { Username = name });
			}
			var observableCollection = new ObservableCollection<UniqueUsername>(uniqueUsernames);
			return observableCollection;
		}

		public static void AnonymizeFileBasedSystemFields(TmFile tm, List<UniqueUsername> UniqueUsernames)
		{
			var filebasedTm = new FileBasedTranslationMemory(tm.Path);
			var translationUnits = GetFileBasedTranslationUnits(tm);
			foreach (var userName in UniqueUsernames)
			{
				if (userName.IsSelected && !String.IsNullOrEmpty(userName.Alias))
				{
					foreach (var tu in translationUnits)
					{
						if (userName.Username == tu.SystemFields.CreationUser || userName.Username == tu.SystemFields.UseUser)
						{
							tu.SystemFields.CreationUser = userName.Alias;
							tu.SystemFields.UseUser = userName.Alias;
							filebasedTm.LanguageDirection.UpdateTranslationUnit(tu);
						}
					}
				}
			}
		}

		public static void AnonymizeServerBasedSystemFields(TmFile tm, List<UniqueUsername> UniqueUsernames, TranslationProviderServer translationProvideServer)
		{
			var serverbasedTM = translationProvideServer.GetTranslationMemory(tm.Path, TranslationMemoryProperties.All);
			var languageDirections = serverbasedTM.LanguageDirections;
			var translationUnits = GetServerBasedTranslationUnits(serverbasedTM.LanguageDirections);
			
			foreach (var userName in UniqueUsernames)
			{
				if (userName.IsSelected && !String.IsNullOrEmpty(userName.Alias))
				{
					foreach (var tu in translationUnits)
					{
						if (userName.Username == tu.SystemFields.CreationUser || userName.Username == tu.SystemFields.UseUser)
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

		private static TranslationUnit[]  GetFileBasedTranslationUnits(TmFile tm)
		{
			var filebasedTm = new FileBasedTranslationMemory(tm.Path);
			var unitsCount = filebasedTm.LanguageDirection.GetTranslationUnitCount();
			var tmIterator = new RegularIterator(unitsCount);
			var translationUnits = filebasedTm.LanguageDirection.GetTranslationUnits(ref tmIterator);
			return translationUnits;
		}

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
	}
}
