using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using GoogleCloudTranslationProvider.Helpers;
using GoogleCloudTranslationProvider.Interfaces;
using GoogleCloudTranslationProvider.Models;
using NLog;
using Sdl.LanguagePlatform.Core;

namespace GoogleCloudTranslationProvider.GoogleAPI
{
    public class V3ResourceManager
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private static readonly string DummyLocation = "gctp-sdl";

        public static List<string> GetLocations(ITranslationOptions tempOptions)
        {
            string errorMessage;
            var output = new List<string>();
            try
            {
                var v3Connector = new V3Connector(tempOptions);
                v3Connector.TryAuthenticate();
                errorMessage = string.Empty;
            }
            catch (Exception e)
            {
                errorMessage = e.Message;
            }

            if (string.IsNullOrEmpty(errorMessage))
            {
                return null;
            }

            if (!errorMessage.Contains("Unsupported location"))
            {
                ErrorHandler.HandleError(PluginResources.Validation_AuthenticationFailed, "Authentication failed");
                return null;
            }

            var matches = new Regex(@"(['])(?:(?=(\\?))\2.)*?\1").Matches(errorMessage);
            return matches.Cast<object>()
                          .Select(match => match.ToString().Replace("'", ""))
                          .Where(locationValue => !locationValue.Equals(DummyLocation) && !locationValue.Equals("parent"))
                          .Distinct()
                          .ToList();
        }

        public static List<RetrievedGlossary> GetGlossaries(TranslationOptions translationOptions)
        {
            var output = new List<RetrievedGlossary>();

            try
            {
                var v3Connector = new V3Connector(translationOptions);
                output.Add(new(new()));
                output.AddRange(v3Connector.GetProjectGlossaries(translationOptions.ProjectLocation).Select(retrievedGlossary => new RetrievedGlossary(retrievedGlossary)));
            }
            catch
            {
                output.Clear();
                output.Add(new(null));
            }

            return output;
        }

        public static async Task<List<RetrievedCustomModel>> GetCustomModelsAsync(TranslationOptions translationOptions)
        {
            var output = new List<RetrievedCustomModel>();

            try
            {
                var v3Connector = new V3Connector(translationOptions);
                output.Add(new RetrievedCustomModel(new CombinedModel())); // Assuming you want an initial entry

                var models = await v3Connector.GetProjectCustomModels();

                // Populate the output based on the retrieved models
                foreach (var model in models)
                {
                    var customModel = new RetrievedCustomModel(model);
                    output.Add(customModel);
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
                output.Clear();
                output.Add(new RetrievedCustomModel(new CombinedModel()));
            }

            return output;
        }

        public static List<RetrievedGlossary> GetPairGlossaries(LanguagePair languagePair, List<RetrievedGlossary> projectGlossaries)
        {
            var output = new List<RetrievedGlossary>();
            foreach (var glossary in projectGlossaries)
            {
                if ((string.Compare(languagePair.SourceCulture.Name, glossary.SourceLanguage?.Name,
                         StringComparison.InvariantCultureIgnoreCase) == 0 &&
                     string.Compare(languagePair.TargetCulture.Name, glossary.TargetLanguage?.Name,
                         StringComparison.InvariantCultureIgnoreCase) == 0) ||
                    (string.Compare(languagePair.SourceCulture.Name, glossary.SourceLanguage?.IetfLanguageTag,
                         StringComparison.InvariantCultureIgnoreCase) == 0 &&
                     string.Compare(languagePair.TargetCulture.Name, glossary.TargetLanguage?.IetfLanguageTag,
                         StringComparison.InvariantCultureIgnoreCase) == 0) ||
                    (string.Compare(languagePair.SourceCulture.RegionNeutralName, glossary.SourceLanguage?.IetfLanguageTag,
                         StringComparison.InvariantCultureIgnoreCase) == 0 &&
                     string.Compare(languagePair.TargetCulture.RegionNeutralName, glossary.TargetLanguage?.IetfLanguageTag,
                         StringComparison.InvariantCultureIgnoreCase) == 0)
                   ) 
                {
                    output.Add(glossary);
                }
                else if (glossary.Languages is not null
                      && glossary.Languages.Contains(languagePair.SourceCulture.RegionNeutralName)
                      && glossary.Languages.Contains(languagePair.TargetCulture.RegionNeutralName))
                {
                    output.Add(glossary);
                }
            }

            output.Insert(0, output.Count == 0 ? new(null) : projectGlossaries.First());
            return output;
        }

        public static List<RetrievedCustomModel> GetPairModels(LanguagePair languagePair, List<RetrievedCustomModel> projectModels)
        {
            var output = projectModels.Where(model => model.SourceLanguage is not null
                                                   && model.TargetLanguage is not null
                                                   && model.SourceLanguage.Equals(languagePair.SourceCulture.RegionNeutralName)
                                                   && model.TargetLanguage.Equals(languagePair.TargetCulture.RegionNeutralName))
                                      .ToList();
            output.Insert(0, output.Count == 0 ? new(null) : projectModels.First());
            return output;
        }
    }
}