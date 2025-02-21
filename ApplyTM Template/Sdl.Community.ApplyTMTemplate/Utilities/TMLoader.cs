using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using Sdl.Community.ApplyTMTemplate.Models;
using Sdl.LanguagePlatform.TranslationMemoryApi;

namespace Sdl.Community.ApplyTMTemplate.Utilities
{
	public class TmLoader
	{
		private bool IsValid(string file)
		{
			return Path.GetExtension(file).ToLower() != ".sdltm";
		}

		public ObservableCollection<TranslationMemory> GetTms(IEnumerable<string> files, ObservableCollection<TranslationMemory> tmCollection)
		{
			var newTmsCollection = new ObservableCollection<TranslationMemory>();

			foreach (var file in files)
			{
				if (IsValid(file)) continue;

				var fileBasedTm = new TranslationMemory(file);

				if (tmCollection.All(tm => tm.Name != fileBasedTm.Name))
				{
					newTmsCollection.Add(fileBasedTm);
				}
			}

			return newTmsCollection;
		}

		public ObservableCollection<TranslationMemory> GetTMsAndApplySelection(IEnumerable<TranslationMemoryEntry> tmEntries, ObservableCollection<TranslationMemory> tmCollection)
		{
            var newTmsCollection = new ObservableCollection<TranslationMemory>();

            foreach (var entry in tmEntries)
            {
                if (IsValid(entry.Path)) continue;

                var fileBasedTm = new TranslationMemory(entry.Path);
				fileBasedTm.IsSelected = entry.IsSelected;

                if (tmCollection.All(tm => tm.Name != fileBasedTm.Name))
                {
                    newTmsCollection.Add(fileBasedTm);
                }
            }

            return newTmsCollection;
        }
	}
}