using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Sdl.Community.MTEdge.Provider.Model
{
	public class TradosToMTEdgeLanguagePair : BaseModel
    {
		private string _languagePair;
		private CultureInfo _tradosCulture;
		private List<DictionaryModel> _dictionaries;
		private List<MTEdgeLanguagePair> _mtEdgeLanguagePairs;
		private DictionaryModel _selectedDictionary;
		private MTEdgeLanguagePair _selectedModel;

        public TradosToMTEdgeLanguagePair(string languagePair, CultureInfo tradosCulture, List<MTEdgeLanguagePair> languagePairs)
        {
            LanguagePair = languagePair;
            TradosCulture = tradosCulture;
            MtEdgeLanguagePairs = languagePairs;
		}

		public string LanguagePair
		{
			get => _languagePair;
			set
			{
				_languagePair = value;
				OnPropertyChanged(nameof(LanguagePair));
			}
		}

		public CultureInfo TradosCulture
		{
			get => _tradosCulture;
			set
			{
				_tradosCulture = value;
				OnPropertyChanged(nameof(TradosCulture));
			}
		}

		public List<DictionaryModel> Dictionaries
		{
			get => _dictionaries;
			set
			{
				_dictionaries = value;
				OnPropertyChanged(nameof(Dictionaries));
			}
		}

		public List<MTEdgeLanguagePair> MtEdgeLanguagePairs
		{
			get => _mtEdgeLanguagePairs;
			set
			{
				_mtEdgeLanguagePairs = value;
				OnPropertyChanged(nameof(MtEdgeLanguagePairs));
			}
		}

		public DictionaryModel SelectedDictionary
		{
			get => _selectedDictionary ??= Dictionaries.FirstOrDefault();
			set
			{
				_selectedDictionary = value;
				OnPropertyChanged(nameof(SelectedDictionary));
			}
		}

		public MTEdgeLanguagePair SelectedModel
		{
			get => _selectedModel ??= MtEdgeLanguagePairs.FirstOrDefault();
			set
			{
				_selectedModel = value;
				OnPropertyChanged(nameof(SelectedModel));
			}
		}
	}
}