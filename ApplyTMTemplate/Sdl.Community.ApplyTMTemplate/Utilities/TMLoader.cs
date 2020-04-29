using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using Sdl.Community.ApplyTMTemplate.Models;
using Sdl.LanguagePlatform.TranslationMemoryApi;

namespace Sdl.Community.ApplyTMTemplate.Utilities
{
	public class TMLoader
	{
		private bool IsValid(string file)
		{
			return Path.GetExtension(file) != ".sdltm";
		}

		public ObservableCollection<TranslationMemory> GetTms(IEnumerable<string> files, ObservableCollection<TranslationMemory> tmCollection)
		{
			var newTmsCollection = new ObservableCollection<TranslationMemory>();

			foreach (var file in files)
			{
				if (IsValid(file)) continue;

				var fileBasedTm = new FileBasedTranslationMemory(file);

				if (tmCollection.All(tm => tm.Name != fileBasedTm.Name))
				{
					newTmsCollection.Add(new TranslationMemory(fileBasedTm));
				}
			}

			return newTmsCollection;
		}
	}
}