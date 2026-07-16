using System;
using System.IO;
using Trados.LocalCache;

namespace Sdl.Community.DeepLMTProvider.Service
{
    /// <summary>
    /// Process-wide local translation cache backed by
    /// %AppData%\Trados AppStore\DeepL\Cache\translation-cache.sqlite.
    /// Payloads are the plain translated text; keys are built per segment in
    /// <see cref="Client.DeepLTranslationProviderClient"/>.
    /// </summary>
    public static class DeepLTranslationCache
    {
        private static readonly Lazy<ILocalCache> Cache = new(CreateCache);

        public static ILocalCache Instance => Cache.Value;

        private static ILocalCache CreateCache()
        {
            var databasePath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                Constants.DeepLDataPath, "Cache", "translation-cache.sqlite");
            return new SqliteLocalCache(databasePath);
        }
    }
}
