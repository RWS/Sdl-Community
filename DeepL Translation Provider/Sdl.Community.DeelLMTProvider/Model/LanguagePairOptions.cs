using Newtonsoft.Json;
using Sdl.LanguagePlatform.Core;
using System.Collections.Generic;
using System.Linq;

namespace Sdl.Community.DeepLMTProvider.Model
{
    public class LanguagePairOptions : ViewModel.ViewModel
    {
        public Formality Formality
        {
            get;
            set => SetField(ref field, value);
        }

        [JsonIgnore]
        public List<GlossaryInfo> Glossaries { get; set; }

        public LanguagePair LanguagePair { get; set; }

        public ModelType ModelType
        {
            get;
            set
            {
                if (!SetField(ref field, value)) return;
                if (value == ModelType.Latency_Optimized)
                    SelectedStyle = Styles.FirstOrDefault(s => s.Name == PluginResources.NoStyle);
            }
        }

        public GlossaryInfo SelectedGlossary
        {
            get;
            set => SetField(ref field, value);
        }

        public DeepLStyle SelectedStyle
        {
            get;
            set
            {
                if (!SetField(ref field, value)) return;
                if (value.Name != PluginResources.NoStyle)
                    ModelType = ModelType.Quality_Optimized;
            }
        }

        [JsonIgnore]
        public List<DeepLStyle> Styles
        {
            get;
            set => SetField(ref field, value);
        } = [];

        /// <summary>
        /// Indicates whether this language pair supports formality settings in DeepL V3 API
        /// </summary>
        [JsonIgnore]
        public bool SupportsFormality
        {
            get;
            set => SetField(ref field, value);
        } = true;

        /// <summary>
        /// Indicates whether this language pair supports glossaries in DeepL V3 API
        /// </summary>
        [JsonIgnore]
        public bool SupportsGlossaries
        {
            get;
            set => SetField(ref field, value);
        } = true;

        public override string ToString() => $"{nameof(LanguagePairOptions)}";

        public void Apply(LanguagePairValidationResult result)
        {
            if (!result.IsSourceLanguageSupported || !result.IsTargetLanguageSupported)
            {
                result.Messages.Clear();
                result.Messages = [$"Language pair {LanguagePair} not supported - removing from list."];
                return;
            }

            SupportsFormality = result.SupportsFormality;
            if (!result.SupportsFormality)
            {
                Formality = Formality.Not_Supported;
                result.Messages.Add($"Formality settings are not supported for target language '{LanguagePair.TargetCulture}'.");
            }

            SupportsGlossaries = result.SupportsGlossaries;
            if (!result.SupportsGlossaries)
            {
                SelectedGlossary = Glossaries?.FirstOrDefault(g => g.Name == PluginResources.GlossariesNotSupported);
                result.Messages.Add($"Glossaries are not supported for this language pair '{LanguagePair.SourceCulture}' → '{LanguagePair.TargetCulture}'.");
            }
        }

        public void ResetToUnsupported()
        {
            SupportsFormality = false;
            SupportsGlossaries = false;
        }
    }
}