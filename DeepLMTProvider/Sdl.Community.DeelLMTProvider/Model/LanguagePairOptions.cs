using System.Collections.Generic;
using Newtonsoft.Json;
using Sdl.LanguagePlatform.Core;

namespace Sdl.Community.DeepLMTProvider.Model
{
	public class LanguagePairOptions : ViewModel.ViewModel
	{
		private Formality _formality;
		private GlossaryInfo _selectedGlossary;

		public Formality Formality
		{
			get => _formality;
			set => SetField(ref _formality, value);
		}

		[JsonIgnore]
		public List<GlossaryInfo> Glossaries { get; set; }

        public override string ToString() => $"{nameof(LanguagePairOptions)}";

        public GlossaryInfo SelectedGlossary
		{
			get => _selectedGlossary;
			set => SetField(ref _selectedGlossary, value);
		}

		public LanguagePair LanguagePair { get; set; }
	}
}