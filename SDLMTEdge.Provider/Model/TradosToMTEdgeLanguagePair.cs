using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Sdl.Community.MTEdge.Provider.Model
{
	public class TradosToMTEdgeLanguagePair
    {
		private DictionaryModel _selectedDictionary;
		private MTEdgeLanguagePair _selectedModel;

        public TradosToMTEdgeLanguagePair(string languagePair, CultureInfo tradosCulture, List<MTEdgeLanguagePair> languagePairs)
        {
            LanguagePair = languagePair;
            TradosCulture = tradosCulture;
            MtEdgeLanguagePairs = languagePairs;
        }

        public string LanguagePair { get; set; }

        public CultureInfo TradosCulture { get; }

        public List<DictionaryModel> Dictionaries { get; set; }

        public List<MTEdgeLanguagePair> MtEdgeLanguagePairs { get; }

		public DictionaryModel SelectedDictionary
		{
			get => _selectedDictionary ??= Dictionaries.FirstOrDefault();
			set => _selectedDictionary = value;
		}

		public MTEdgeLanguagePair SelectedModel
		{
			get => _selectedModel ??= MtEdgeLanguagePairs.FirstOrDefault();
			set => _selectedModel = value;
		}
	}
}