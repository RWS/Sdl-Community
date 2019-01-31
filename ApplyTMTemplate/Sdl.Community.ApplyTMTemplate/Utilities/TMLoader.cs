using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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


			foreach (var file in files)
			{
				if (IsValid(file)) continue;

				var fileBasedTm = new FileBasedTranslationMemory(file);

				if (tmCollection.All(tm => tm.Name != fileBasedTm.Name))
				{
					tmCollection.Add(new TranslationMemory(fileBasedTm));
				}
			}

			return tmCollection;
		}
	}
}
