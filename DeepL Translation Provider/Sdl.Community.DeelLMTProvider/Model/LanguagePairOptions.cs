using Newtonsoft.Json;
using Sdl.LanguagePlatform.Core;
using System.Collections.Generic;

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
            set => SetField(ref field, value);
        }

        public GlossaryInfo SelectedGlossary
        {
            get;
            set => SetField(ref field, value);
        }

        public DeepLStyle SelectedStyle
        {
            get;
            set => SetField(ref field, value);
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