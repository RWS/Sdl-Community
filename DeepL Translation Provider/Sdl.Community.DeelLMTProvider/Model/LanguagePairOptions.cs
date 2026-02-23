using Newtonsoft.Json;
using Sdl.LanguagePlatform.Core;
using System.Collections.Generic;
using System.Linq;

namespace Sdl.Community.DeepLMTProvider.Model
{
    public class LanguagePairOptions : ViewModel.ViewModel
    {
        private Formality _formality;
        private ModelType _modelType;
        private GlossaryInfo _selectedGlossary;
        private DeepLStyle _selectedStyle;
        private List<DeepLStyle> _styles = [];

        public Formality Formality
        {
            get => _formality;
            set => SetField(ref _formality, value);
        }

        [JsonIgnore]
        public List<GlossaryInfo> Glossaries { get; set; }

        public LanguagePair LanguagePair { get; set; }

        public ModelType ModelType
        {
            get => _modelType;
            set
            {
                if (!SetField(ref _modelType, value)) return;
                if (value == ModelType.Latency_Optimized)
                    SelectedStyle = Styles.FirstOrDefault(s => s.Name == PluginResources.NoStyle);
            }
        }

        public GlossaryInfo SelectedGlossary
        {
            get => _selectedGlossary;
            set => SetField(ref _selectedGlossary, value);
        }

        public DeepLStyle SelectedStyle
        {
            get => _selectedStyle;
            set
            {
                if (!SetField(ref _selectedStyle, value)) return;
                if (value.Name != PluginResources.NoStyle)
                    ModelType = ModelType.Quality_Optimized;
            }
        }

        [JsonIgnore]
        public List<DeepLStyle> Styles
        {
            get => _styles;
            set => SetField(ref _styles, value);
        }

        public override string ToString() => $"{nameof(LanguagePairOptions)}";
    }
}