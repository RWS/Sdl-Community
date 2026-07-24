using System;
using System.IO;
using LanguageWeaverProvider.Model;
using Trados.LocalCache;

namespace LanguageWeaverProvider.Services
{
    public static class LanguageWeaverCache
    {
        private static readonly Lazy<ILocalCache> Cache = new(CreateCache);

        public static ILocalCache Instance => Cache.Value;

        private static ILocalCache CreateCache()
        {
            var databasePath = Path.Combine(new PathInfo().ApplicationFolderPath, "Cache", "translation-cache.sqlite");
            return new SqliteLocalCache(databasePath);
        }
    }
}
