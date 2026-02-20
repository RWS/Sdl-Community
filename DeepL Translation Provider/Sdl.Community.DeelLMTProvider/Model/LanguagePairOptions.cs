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

        public override string ToString() => $"{nameof(LanguagePairOptions)}";
    }
}