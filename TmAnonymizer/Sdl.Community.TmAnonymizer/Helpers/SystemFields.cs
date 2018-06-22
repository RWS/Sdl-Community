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
			var filebasedTm = new FileBasedTranslationMemory(tm.Path);
			var unitsCount = filebasedTm.LanguageDirection.GetTranslationUnitCount();
			var tmIterator = new RegularIterator(unitsCount);
			var tus = filebasedTm.LanguageDirection.GetTranslationUnits(ref tmIterator);
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
			var languageDirections = translationMemory.LanguageDirections;
			foreach (var languageDirection in languageDirections)
			{
				var unitsCount = languageDirection.GetTranslationUnitCount();
				if (unitsCount == 0) continue;
				var tmIterator = new RegularIterator(unitsCount);
				var translationUnits = languageDirection.GetTranslationUnits(ref tmIterator);
				foreach (var tu in translationUnits)
				{
					listOfFieldNames.AddRange(new List<string>() { tu.SystemFields.CreationUser, tu.SystemFields.UseUser });
				}
			}
			var uniqueUsers = listOfFieldNames.Distinct().ToList();
			foreach (var name in uniqueUsers)
			{
				uniqueUsernames.Add(new UniqueUsername() { Username = name });
			}
			var observableCollection = new ObservableCollection<UniqueUsername>(uniqueUsernames);
			return observableCollection;
		}
	}
}
