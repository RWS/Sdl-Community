using System.Collections.Generic;
using Sdl.LanguagePlatform.Core;

namespace Sdl.Community.DeepLMTProvider.Model
{
	public class LanguagePairOptions : ViewModel.ViewModel
	{
		private Formality _formality;

		public Formality Formality
		{
			get => _formality;
			set => SetField(ref _formality, value);
		}

		public List<Glossary> Glossaries { get; set; }

		public Glossary SelectedGlossary { get; set; }

		public LanguagePair LanguagePair { get; set; }
	}
}