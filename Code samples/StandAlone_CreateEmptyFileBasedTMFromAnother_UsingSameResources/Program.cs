using Sdl.LanguagePlatform.TranslationMemoryApi;
using System.Collections.Generic;

namespace StandAlone_CreateEmptyFileBasedTMFromAnother_UsingSameResources
{
    public class Program
    {
        private static void Main(string[] args)
        {
            var originalTm = new FileBasedTranslationMemory("filePath");

            var langResBundleList = new List<LanguageResourceBundle>();
            foreach (var bundle in originalTm.LanguageResourceBundles)
            {
                langResBundleList.Add(bundle.Clone());
            }

            var tmCreator = new TmCreator();

            var newTm = tmCreator.CreateFileBasedTm("destFileName");
            langResBundleList.ForEach(b => newTm.LanguageResourceBundles.Add(b));

            newTm.Save();
        }
    }
}