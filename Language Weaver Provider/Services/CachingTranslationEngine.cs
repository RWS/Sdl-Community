using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LanguageWeaverProvider.Model;
using LanguageWeaverProvider.Model.Interface;
using LanguageWeaverProvider.Services.Model;
using Newtonsoft.Json;
using Trados.LocalCache;

namespace LanguageWeaverProvider.Services
{
    public sealed class CachingTranslationEngine : ITranslationEngine
    {
        private readonly ITranslationEngine _inner;
        private readonly ITranslationOptions _options;

        public CachingTranslationEngine(ITranslationEngine inner, ITranslationOptions options)
        {
            _inner = inner;
            _options = options;
        }

        public async Task<IReadOnlyList<TranslationResult>> TranslateAsync(AccessToken accessToken, PairMapping mappedPair, IReadOnlyList<SegmentSerializer> segmentSerializers)
        {
            if (!_options.ProviderSettings.UseLocalCache || segmentSerializers is null || segmentSerializers.Count == 0)
            {
                return await _inner.TranslateAsync(accessToken, mappedPair, segmentSerializers);
            }

            var results = new TranslationResult[segmentSerializers.Count];
            var cacheKeys = new string[segmentSerializers.Count];
            var pending = new List<SegmentSerializer>();
            var pendingIndices = new List<int>();

            for (var i = 0; i < segmentSerializers.Count; i++)
            {
                cacheKeys[i] = BuildCacheKey(mappedPair, segmentSerializers[i]);
                if (LanguageWeaverCache.Instance.TryGet(cacheKeys[i], out var json)
                 && JsonConvert.DeserializeObject<TranslationResult>(json) is { } cached)
                {
                    results[i] = cached;
                    continue;
                }

                pending.Add(segmentSerializers[i]);
                pendingIndices.Add(i);
            }

            if (pending.Count > 0)
            {
                var fresh = await _inner.TranslateAsync(accessToken, mappedPair, pending);
                for (var i = 0; i < pendingIndices.Count; i++)
                {
                    var result = fresh is not null && i < fresh.Count ? fresh[i] : null;
                    var index = pendingIndices[i];
                    results[index] = result;

                    if (!string.IsNullOrEmpty(result?.Translation))
                    {
                        LanguageWeaverCache.Instance.Set(cacheKeys[index], JsonConvert.SerializeObject(result));
                    }
                }
            }

            return results;
        }

        private string BuildCacheKey(PairMapping mappedPair, SegmentSerializer serializer)
        {
            var dictionaries = mappedPair.Dictionaries?.Where(d => d.IsSelected).Select(d => d.DictionaryId);
            var linguisticOptions = mappedPair.LinguisticOptions?.Select(lo => $"{lo.Id}={lo.SelectedValue}");

            return new CacheKeyBuilder()
                .Add("provider", "languageweaver")
                .Add("backend", _options.PluginVersion)
                .Add("model", mappedPair.SelectedModel?.Model)
                .Add("src", mappedPair.SourceCode)
                .Add("tgt", mappedPair.TargetCode)
                .Add("qe", mappedPair.SelectedModel?.QeSupport)
                .AddValues("dictionaries", dictionaries)
                .AddValues("linguisticOptions", linguisticOptions)
                .Add("content", serializer.SerializedSegment)
                .Build();
        }
    }
}
