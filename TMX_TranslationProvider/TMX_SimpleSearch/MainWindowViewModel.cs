using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Newtonsoft.Json;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.TranslationMemory;
using TMX_Lib.Db;
using TMX_Lib.Search;

namespace TMX_SimpleSearch
{
	public enum SearchType
	{
		Exact, Fuzzy, Concordance,
	}

	public class SimpleSearchResult 
	{
		public string SourceText { get; set; }
		public string TargetText { get; set; }
		public int Score { get; set; }
	}

	public class MainWindowViewModel : INotifyPropertyChanged
	{
		private bool _loaded;
		private bool _ignoreUpdate;
		private IReadOnlyList<string> _databases = new List<string>();
		private int _lastSearchIndex = 0;
		private int _selectedTargetLanguageIndex = -1;
		private int _selectedSourceLanguageIndex = -1;
		private int _selectedDbIndex = -1;
		private IReadOnlyList<SimpleSearchResult> _results = new List<SimpleSearchResult>();
		private int _searchTypeIdx = 0;

		private TmxSearch _searcher;
		private IReadOnlyList<string> _languages = new List<string>();
		private string _searchText = "";
		private bool _isSearching = false;

		private string _settingsFileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Trados AppStore", "TMX_lib", "simple_search_settings.txt");
		private string _error = "";
		private long _searchTimeMs = 0;
		private long _searchTimeSeconds = 0;
		private string _status = "";

		private const int MAX_SAVE_SEARCH_COUNT = 20;

		private string DefaultSourceLanguage() => Languages.Count > 0 ? Languages[0] : "";
		private string DefaultTargetLanguage() => Languages.Count > 1 ? Languages[1] : "";

		public double ItemHeight => 30;
		public ObservableCollection<SearchItem> LastSearches { get; } = new ObservableCollection<SearchItem>();

		public IReadOnlyList<string> Databases
		{
			get => _databases;
			set
			{
				if (Equals(value, _databases)) return;
				_databases = value;
				OnPropertyChanged();
			}
		}
		public IReadOnlyList<string> SearchTypes => Enum.GetValues(typeof(SearchType)).OfType<SearchType>().Select(st => st.ToString()).ToList();

		public SearchType Type() => (SearchType)SearchTypeIdx;

		public int SearchTypeIdx
		{
			get => _searchTypeIdx;
			set
			{
				if (value == _searchTypeIdx) return;
				_searchTypeIdx = value;
				OnPropertyChanged();
			}
		}

		public IReadOnlyList<string> Languages
		{
			get => _languages;
			set
			{
				if (Equals(value, _languages)) return;
				_languages = value;
				OnPropertyChanged();
			}
		}

		public int SelectedDbIndex
		{
			get => _selectedDbIndex;
			set
			{
				if (value == _selectedDbIndex) return;
				_selectedDbIndex = value;
				OnPropertyChanged();
			}
		}

		public int SelectedSourceLanguageIndex
		{
			get => _selectedSourceLanguageIndex;
			set
			{
				if (value == _selectedSourceLanguageIndex) return;
				_selectedSourceLanguageIndex = value;
				OnPropertyChanged();
			}
		}

		public int SelectedTargetLanguageIndex
		{
			get => _selectedTargetLanguageIndex;
			set
			{
				if (value == _selectedTargetLanguageIndex) return;
				_selectedTargetLanguageIndex = value;
				OnPropertyChanged();
			}
		}


		public int LastSearchIndex
		{
			get => _lastSearchIndex;
			set
			{
				if (value == _lastSearchIndex) return;
				_lastSearchIndex = value;
				OnPropertyChanged();
			}
		}

		public string Error
		{
			get => _error;
			set
			{
				if (value == _error) return;
				_error = value;
				OnPropertyChanged();
			}
		}

		public string Status
		{
			get => _status;
			set
			{
				if (value == _status) return;
				_status = value;
				OnPropertyChanged();
			}
		}

		public IReadOnlyList<SimpleSearchResult> Results
		{
			get => _results;
			set
			{
				if (Equals(value, _results)) return;
				_results = value;
				OnPropertyChanged();
			}
		}

		public string SearchText
		{
			get => _searchText;
			set
			{
				if (value == _searchText) return;
				_searchText = value;
				OnPropertyChanged();
			}
		}

		public long SearchTimeSeconds
		{
			get => _searchTimeSeconds;
			set
			{
				if (value == _searchTimeSeconds) return;
				_searchTimeSeconds = value;
				OnPropertyChanged();
			}
		}

		public bool IsSearching
		{
			get => _isSearching;
			set
			{
				if (value == _isSearching) return;
				_isSearching = value;
				OnPropertyChanged();
			}
		}

		private void LoadLastSearches()
		{
			if (File.Exists(_settingsFileName))
			{
				var list = JsonConvert.DeserializeObject< List<SearchItem>>(File.ReadAllText(_settingsFileName));
				foreach (var item in list)
					LastSearches.Add(item);
			}

			if (LastSearches.Count > 0)
				LastSearchIndex = LastSearches.Count - 1;
			else
			{
				LastSearches.Add(new SearchItem
				{
					SourceLanguage = DefaultSourceLanguage(),
					TargetLanguage = DefaultTargetLanguage(),
				});
				LastSearchIndex = 0;
			}

			_loaded = true;
		}

		private void SaveLastSearches()
		{
			var str = JsonConvert.SerializeObject(LastSearches);
			File.WriteAllText(_settingsFileName, str);
		}

		public async Task InitAsync()
		{
			Databases = await TmxMongoDb.GetLocalDatabaseNamesAsync();
			LoadLastSearches();
			SelectedDbIndex = Databases.Count > 0 ? 0 : -1;
			LoadSearch();
		}


		public async Task DoSearchNow()
		{
			if (_searcher == null || SearchText == "" || SelectedSourceLanguageIndex < 0 || SelectedTargetLanguageIndex < 0)
				return;// can't search

			var segment = new Segment();
			segment.Add(SearchText);
			var sourceLangugage = Languages[SelectedSourceLanguageIndex];
			var targetLanguage = Languages[SelectedTargetLanguageIndex];
			var settings = TmxSearchSettings.Default();
			switch ((SearchType)SearchTypeIdx)
			{
				case SearchType.Exact:
					settings.Mode = SearchMode.ExactSearch;
					break;
				case SearchType.Fuzzy:
					settings.Mode = SearchMode.FuzzySearch;
					break;
				case SearchType.Concordance:
					settings.Mode = SearchMode.ConcordanceSearch;
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}

			// the idea: save this, just in case search takes too long, and user cancels/closes app
			_searchTimeMs = -1;
			SaveSearch();

			var watch = Stopwatch.StartNew();
			Error = "";
			Status = "Searching...";
			try
			{
				var results = await _searcher.Search(settings, segment, new LanguagePair(sourceLangugage, targetLanguage));
				Results = results.Select(r => new SimpleSearchResult
					{
						Score = r.ScoringResult.BaseScore,
						SourceText = r.TranslationProposal.SourceSegment.ToPlain(),
						TargetText = r.TranslationProposal.TargetSegment.ToPlain(),
					})
					.ToList();
				Status = $"{Results.Count} results.";
			}
			catch (Exception e)
			{
				Error = e.Message;
			}

			_searchTimeMs = watch.ElapsedMilliseconds;
			SaveSearch();
		}

		private void SaveSearch()
		{
			// if anything in the text changed + old text not same -> create a copy of this search
			var oldText = LastSearchIndex >= 0 && LastSearchIndex < LastSearches.Count ? LastSearches[LastSearchIndex].Text : "";
			var same = SearchText == oldText;
			var sourceLangugage = Languages[SelectedSourceLanguageIndex];
			var targetLanguage = Languages[SelectedTargetLanguageIndex];

			// note: if old text is "" -> override that
			if (!same && oldText != "")
			{
				_ignoreUpdate = true;
				LastSearches.Add(new SearchItem());
				if (LastSearches.Count > MAX_SAVE_SEARCH_COUNT)
					LastSearches.RemoveAt(0);
				LastSearchIndex = LastSearches.Count - 1;
				_ignoreUpdate = false;
			}

			var curSearch = LastSearches[LastSearchIndex];
			curSearch.SourceLanguage = sourceLangugage;
			curSearch.TargetLanguage = targetLanguage;
			curSearch.SearchType = Type();
			curSearch.Text = SearchText;
			SearchTimeSeconds = curSearch.SearchTimeSeconds = (int)(_searchTimeMs / 1000);

			SaveLastSearches();
		}

		private void LoadSearch()
		{
			if (LastSearchIndex < 0)
				return;
			var info = LastSearches[LastSearchIndex];
			SearchTypeIdx = (int)info.SearchType;
			SearchTimeSeconds = info.SearchTimeSeconds;
			SearchText = info.Text;
			LoadLanguages();
		}

		private void LoadLanguages()
		{
			if (LastSearchIndex < 0)
				return;
			var info = LastSearches[LastSearchIndex];
			if (Languages.Count > 0)
			{
				var sourceLangIdx = Languages.ToList().IndexOf(info.SourceLanguage);
				var targetLangIdx = Languages.ToList().IndexOf(info.TargetLanguage);
				SelectedSourceLanguageIndex = sourceLangIdx >= 0 ? sourceLangIdx : 0;
				SelectedTargetLanguageIndex = targetLangIdx >= 0 ? targetLangIdx : 0;
			}
		}

		private async void VmPropertyChanged(string name)
		{
			if (!_loaded || _ignoreUpdate)
				return;
			switch (name)
			{
				case nameof(SelectedDbIndex):
					if (SelectedDbIndex >= 0)
						_searcher = new TmxSearch(new TmxMongoDb(Databases[SelectedDbIndex]));
					await _searcher.LoadLanguagesAsync();
					Languages = _searcher.SupportedLanguages();
					LoadLanguages();
					break;

				case nameof(LastSearchIndex):
					LoadSearch();
					break;
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;

		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			VmPropertyChanged(propertyName);
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		protected bool SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
		{
			if (EqualityComparer<T>.Default.Equals(field, value)) return false;
			field = value;
			OnPropertyChanged(propertyName);
			return true;
		}
	}
}
