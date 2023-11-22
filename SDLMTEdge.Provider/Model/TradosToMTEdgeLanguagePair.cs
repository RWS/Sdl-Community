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
		private int _selectedDictionaryIndex;
		private MTEdgeLanguagePair _selectedModel;
		private int _selectedModelIndex;

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
			get
			{
				return IsSupported
					? _selectedDictionary ??= Dictionaries?[SelectedDictionaryIndex]
					: new();
			}
		}

		public int SelectedDictionaryIndex
		{
			get => _selectedDictionaryIndex;
			set
			{
				if (_selectedDictionaryIndex == value) return;
				_selectedDictionaryIndex = value;
				OnPropertyChanged(nameof(SelectedDictionaryIndex));
			}
		}

		public MTEdgeLanguagePair SelectedModel
		{
			get
			{
				return IsSupported
					? _selectedModel ??= MtEdgeLanguagePairs?[SelectedModelIndex]
					: new();
			}
		}

		public int SelectedModelIndex
		{
			get => _selectedModelIndex;
			set
			{
				if (_selectedModelIndex == value) return;
				_selectedModelIndex = value;
				OnPropertyChanged(nameof(SelectedModelIndex));
			}
		}

		public bool IsSupported { get; set; } = true;
	}
}