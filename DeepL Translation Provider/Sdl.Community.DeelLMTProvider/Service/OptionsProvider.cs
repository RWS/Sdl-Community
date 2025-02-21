using Newtonsoft.Json;
using Sdl.Community.DeepLMTProvider.Model;
using System;

namespace Sdl.Community.DeepLMTProvider.Service
{
    public class OptionsProvider
    {
        public DeepLTranslationOptions GetTranslationOptions(Uri translationProviderUri, string state)
        {
            var translationOptions = new DeepLTranslationOptions();
            if (!string.IsNullOrWhiteSpace(state)) translationOptions = GetDeserializedOptions(state);

            translationOptions.Uri = translationProviderUri;
            return translationOptions;
        }

        private static DeepLTranslationOptions GetDeserializedOptions(string state)
        {
            try
            {
                return JsonConvert.DeserializeObject<DeepLTranslationOptions>(state);
            }
            catch
            {
                return new DeepLTranslationOptions();
            }
        }
    }
}