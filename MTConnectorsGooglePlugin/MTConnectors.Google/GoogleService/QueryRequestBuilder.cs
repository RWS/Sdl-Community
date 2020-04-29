using Sdl.LanguagePlatform.MTConnectors.Google.Interfaces;

namespace Sdl.LanguagePlatform.MTConnectors.Google.GoogleService
{
    internal class QueryRequestBuilder
    {
        private readonly IGoogleSettings _googleSettings;

        public QueryRequestBuilder(IGoogleSettings settings)
        {
            _googleSettings = settings;
        }

        public Request BuildLanguageQueryRequest()
        {
            var request = new Request(URLs.LanguageUrl, RequestType.Get);
            ApplyAuthentication(request);
            return request;
        }

        public Request BuildTranslateRequest(string sourceLanguage, string targetLanguage)
        {
            Request r = new Request(URLs.TranslateUrl, RequestType.Post);
            ApplyAuthentication(r);

            r.AddField("format", "html");
            r.AddField("target", targetLanguage);
            r.AddField("source", sourceLanguage);
            r.AddField("model", _googleSettings.TranslationModel == MachineTranslationModel.Neural ? "nmt" : "base");
            return r;
        }

        private void ApplyAuthentication(Request request)
        {
            if (_googleSettings != null && _googleSettings.ApiKey != null)
            {
                request.AddField("key", _googleSettings.ApiKey);
            }
        }


    }
}
