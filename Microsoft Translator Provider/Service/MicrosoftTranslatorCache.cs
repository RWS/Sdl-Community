using System;
using System.IO;
using Trados.LocalCache;

namespace MicrosoftTranslatorProvider.Service
{
    public static class MicrosoftTranslatorCache
    {
        private static readonly Lazy<ILocalCache> Cache = new(CreateCache);

        public static ILocalCache Instance => Cache.Value;

        private static ILocalCache CreateCache()
        {
            var databasePath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "Trados AppStore", "Microsoft Translator Provider", "Cache", "translation-cache.sqlite");
            return new SqliteLocalCache(databasePath);
        }
    }
}
