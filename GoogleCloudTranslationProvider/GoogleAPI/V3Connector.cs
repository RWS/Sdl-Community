using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using Google.Api.Gax.ResourceNames;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.AutoML.V1;
using Google.Cloud.Translate.V3;
using GoogleCloudTranslationProvider.Helpers;
using GoogleCloudTranslationProvider.Interfaces;
using GoogleCloudTranslationProvider.Models;
using Newtonsoft.Json;
using NLog;
using Sdl.Core.Globalization;

namespace GoogleCloudTranslationProvider.GoogleAPI
{
    public class V3Connector
    {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private readonly ITranslationOptions _options;

        private TranslationServiceClient _translationServiceClient;
        private List<V3LanguageModel> _supportedLanguages;

        public V3Connector(ITranslationOptions options)
        {
            _options = options;
            ConnectToApi();
        }


        private void ConnectToApi()
        {
            try
            {
                Environment.SetEnvironmentVariable(Constants.GoogleApiEnvironmentVariableName, _options.JsonFilePath);
                _translationServiceClient = TranslationServiceClient.Create();
            }
            catch (Exception e)
            {
                _logger.Error($"{MethodBase.GetCurrentMethod().Name}: {e}");
                ErrorHandler.HandleError(e);
            }
        }

        public void TryAuthenticate()
        {
            var sourceCultureInfo = new CultureInfo("en-GB");
            var targetCultureInfo = new CultureInfo("de-DE");
            var request = new TranslateTextRequest
            {
                Contents = { string.Empty },
                SourceLanguageCode = "en",
                TargetLanguageCode = "de",
                ParentAsLocationName = new LocationName(_options.ProjectId, _options.ProjectLocation),
                MimeType = "text/plain",
                Model = SetCustomModel(sourceCultureInfo, targetCultureInfo),
                GlossaryConfig = SetGlossary(sourceCultureInfo, targetCultureInfo)
            };

            _translationServiceClient.TranslateText(request);
        }

        public bool IsSupportedLanguage(CultureCode sourceCulture, CultureCode targetCulture)
        {
            EnsureSupportedLanguagesInitialized();
            var sourceLanguageCode = sourceCulture.GetLanguageCode(ApiVersion.V3);
            var targetLanguageCode = targetCulture.GetLanguageCode(ApiVersion.V3);
            if (string.IsNullOrEmpty(sourceLanguageCode) || string.IsNullOrEmpty(targetLanguageCode))
            {
                return false;
            }

            var sourceGoogleLanguage = GetSupportedLanguage(sourceLanguageCode);
            var targetGoogleLanguage = GetSupportedLanguage(targetLanguageCode);
            return sourceGoogleLanguage?.SupportSource is true
                && targetGoogleLanguage?.SupportTarget is true;
        }

        private void EnsureSupportedLanguagesInitialized()
        {
            if (_supportedLanguages is null || !_supportedLanguages.Any())
            {
                SetGoogleAvailableLanguages();
            }
        }

        private V3LanguageModel GetSupportedLanguage(string languageCode)
        {
            return _supportedLanguages.FirstOrDefault(lang => lang.GoogleLanguageCode.Equals(languageCode));
        }

        public List<V3LanguageModel> GetLanguages()
        {
            var locationName = new LocationName(_options.ProjectId, "global");
            var request = new GetSupportedLanguagesRequest { ParentAsLocationName = locationName };
            var response = _translationServiceClient.GetSupportedLanguages(request);
            return response.Languages.Select(language => new V3LanguageModel
            {
                GoogleLanguageCode = language.LanguageCode,
                SupportSource = language.SupportSource,
                SupportTarget = language.SupportTarget,
                CultureInfo = new CultureInfo(language.LanguageCode)
            }).ToList();
        }

        private void SetGoogleAvailableLanguages()
        {
            try
            {
                TrySetGoogleAvailableLanguages();
            }
            catch (Exception e)
            {
                _logger.Error($"{MethodBase.GetCurrentMethod().Name}: {e}");
            }
        }

        private void TrySetGoogleAvailableLanguages()
        {
            _supportedLanguages ??= new();
            var locationName = new LocationName(_options.ProjectId, "global");
            var request = new GetSupportedLanguagesRequest { ParentAsLocationName = locationName };
            var response = _translationServiceClient.GetSupportedLanguages(request);

            _supportedLanguages.AddRange(response.Languages.Select(language => new V3LanguageModel
            {
                GoogleLanguageCode = language.LanguageCode,
                SupportSource = language.SupportSource,
                SupportTarget = language.SupportTarget,
                CultureInfo = new CultureInfo(language.LanguageCode)
            }));
        }

        public string TranslateText(CultureCode sourceLanguage, CultureCode targetLanguage, string sourceText, string format)
        {
            try
            {
                return TryTranslateText(sourceLanguage, targetLanguage, sourceText, format);
            }
            catch (Exception e)
            {
                _logger.Error($"{MethodBase.GetCurrentMethod().Name}: {e}");
                throw e;
            }
        }

        private string TryTranslateText(CultureCode sourceLanguage, CultureCode targetLanguage, string sourceText, string format)
        {
            var request = CreateTranslationRequest(sourceLanguage, targetLanguage, sourceText, format);
            if (_translationServiceClient.TranslateText(request) is not TranslateTextResponse translationResponse)
            {
                return string.Empty;
            }

            return request.GlossaryConfig is null ? translationResponse.Translations[0].TranslatedText
                : translationResponse.GlossaryTranslations[0].TranslatedText;
        }

        private TranslateTextRequest CreateTranslationRequest(CultureCode sourceLanguage, CultureCode targetLanguage, string sourceText, string format)
        {
            var request = new TranslateTextRequest
            {
                Contents = { sourceText },
                SourceLanguageCode = sourceLanguage.GetLanguageCode(ApiVersion.V3),
                TargetLanguageCode = targetLanguage.GetLanguageCode(ApiVersion.V3),
                ParentAsLocationName = new LocationName(_options.ProjectId, _options.ProjectLocation),
                MimeType = format == "text" ? "text/plain" : "text/html",
                Model = SetCustomModel(sourceLanguage, targetLanguage),
                GlossaryConfig = SetGlossary(sourceLanguage, targetLanguage)
            };
            return request;
        }

        public List<Glossary> GetProjectGlossaries(string location = null)
        {
            var locationName = LocationName.FromProjectLocation(_options.ProjectId, location ?? _options.ProjectLocation);
            var glossariesRequest = new ListGlossariesRequest { ParentAsLocationName = locationName };

            var list = _translationServiceClient.ListGlossaries(glossariesRequest).ToList();
            return list;
        }

        private TranslateTextGlossaryConfig SetGlossary(CultureInfo sourceLanguage, CultureInfo targetLanguage)
        {

            var selectedGlossary = _options.LanguageMappingPairs?
                                           .FirstOrDefault(x => x.LanguagePair.SourceCulture.Name == sourceLanguage.Name && x.LanguagePair.TargetCulture.Name == targetLanguage.Name)?
                                           .SelectedGlossary
                                           .Glossary;

            if (selectedGlossary is null
             || string.IsNullOrEmpty(selectedGlossary.Name))
            {
                return null;
            }

            return new TranslateTextGlossaryConfig
            {
                Glossary = selectedGlossary.Name,
                IgnoreCase = true
            };
        }

        public class TranslationModelsResponse
        {
            public List<CombinedModel> Models { get; set; }
        }

        public async Task<string> GetCredential()
        {
            var credential = await GoogleCredential.FromFile(_options.JsonFilePath)
                .CreateScoped("https://www.googleapis.com/auth/cloud-translation")
                .UnderlyingCredential
                .GetAccessTokenForRequestAsync();

            return credential;
        }

        public async Task<List<CombinedModel>> GetV3TranslationModelsAsync()
        {
            // Get access token
            var credential = await GetCredential();

            using (var httpClient = new HttpClient())
            {
                // Set the authorization header
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", credential);

                var url = $"https://translate.googleapis.com/v3/projects/{_options.ProjectId}/locations/{_options.ProjectLocation}/models";

                var response = await httpClient.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    var modelsResponse = JsonConvert.DeserializeObject<TranslationModelsResponse>(jsonResponse);

                    return modelsResponse.Models;
                }

                _logger.Error($"Error: {response.StatusCode} - {await response.Content.ReadAsStringAsync()}");
            }

            return [];
        }

        public async Task<List<CombinedModel>> GetProjectCustomModels()
        {
            var client = await AutoMlClient.CreateAsync();
            var parent = new LocationName(_options.ProjectId, _options.ProjectLocation);

            var combinedModels = new List<CombinedModel>();

            // Get models
            var autoMlModels = client.ListModels(parent);
            foreach (var autoMlModel in autoMlModels)
            {
                var model = new CombinedModel
                {
                    Dataset = autoMlModel.DatasetId,
                    DisplayName = autoMlModel.DisplayName,
                    Name = autoMlModel.ModelName.ToString(),
                    SourceLanguageCode = autoMlModel.TranslationModelMetadata?.SourceLanguageCode,
                    TargetLanguageCode = autoMlModel.TranslationModelMetadata?.TargetLanguageCode
                };

                var existingModel = combinedModels.FirstOrDefault(a =>
                    a.Name == model.Name);
                if (existingModel == null)
                {
                    combinedModels.Add(model);
                }
            }

            // Add V3 translation models while checking for duplicates
            var v3Models = await GetV3TranslationModelsAsync();
            
            foreach (var model in v3Models)
            {
                var existingModel = combinedModels.FirstOrDefault(a => 
                    a.Name == model.Name);
                if (existingModel == null)
                {
                    combinedModels.Add(model);
                }
            }

            return combinedModels.ToList();
        }

        private string SetCustomModel(CultureInfo sourceLanguage, CultureInfo targetLanguage)
        {
            var defaultPath = $"projects/{_options.ProjectId}/locations/{_options.ProjectLocation}/models/general/nmt";
            var selectedModel = _options.LanguageMappingPairs?
                                           .FirstOrDefault(x => x.LanguagePair.SourceCulture.Name == sourceLanguage.Name && x.LanguagePair.TargetCulture.Name == targetLanguage.Name)?
                                           .SelectedModel?
                                           .ModelPath;
            return selectedModel switch
            {
                not null => selectedModel,
                _ => defaultPath
            };
        }
    }
}