using System;
using System.IO;
using Trados.LocalCache;

namespace GoogleCloudTranslationProvider.GoogleAPI
{
    public static class GoogleTranslationCache
    {
        private static readonly Lazy<ILocalCache> Cache = new(CreateCache);

        public static ILocalCache Instance => Cache.Value;

        private static ILocalCache CreateCache()
        {
            var databasePath = Path.Combine(Constants.AppDataFolder, "Cache", "translation-cache.sqlite");
            return new SqliteLocalCache(databasePath);
        }
    }
}
