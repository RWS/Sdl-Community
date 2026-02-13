using System.Collections.Generic;
using Newtonsoft.Json;
using Sdl.LanguagePlatform.Core;

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

        public override string ToString() => $"{nameof(LanguagePairOptions)}";

        public GlossaryInfo SelectedGlossary
        {
            get;
            set => SetField(ref field, value);
        }

        public LanguagePair LanguagePair { get; set; }
	}
}