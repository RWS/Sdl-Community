using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Newtonsoft.Json;
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
		public string Text { get; set; }
		public int Score { get; set; }
	}

	public class MainWindowViewModel : INotifyPropertyChanged
	{
		private bool _loaded;
		private IReadOnlyList<string> _databases = new List<string>();

		public double ItemHeight => 30;

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

		public ObservableCollection<SearchItem> LastSearches { get; } = new ObservableCollection<SearchItem>();

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

		private string FileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "SDL Community", "TMX_lib", "simple_search_settings.txt");
		private int _lastSearchIndex = 0;
		private int _selectedTargetLanguageIndex = -1;
		private int _selectedSourceLanguageIndex = -1;
		private int _selectedDbIndex = -1;
		private IReadOnlyList<SimpleSearchResult> _results = new List<SimpleSearchResult>();
		private int _searchTypeIdx = 0;

		private TmxSearch searcher_;
		private IReadOnlyList<string> _languages = new List<string>();
		private string _searchText = "";
		private bool _isSearching = false;

		private string DefaultSourceLanguage() => Languages.Count > 0 ? Languages[0] : "";
		private string DefaultTargetLanguage() => Languages.Count > 1 ? Languages[1] : "";
		private void LoadLastSearches()
		{
			if (File.Exists(FileName))
			{
				var list = JsonConvert.DeserializeObject< List<SearchItem>>(File.ReadAllText(FileName));
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

		public void SaveLastSearches()
		{

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
			// if anything in the text changed + old text not same -> create a copy of this search

			// if current search text != existing search -> create new entry
			// otherwise, save here

			// care about max no of search items

			// update results
		}

		private void LoadSearch()
		{
			if (LastSearchIndex < 0)
				return;
			var info = LastSearches[LastSearchIndex];
			var sourceLangIdx = Languages.ToList().IndexOf(info.SourceLanguage);
			var targetLangIdx = Languages.ToList().IndexOf(info.TargetLanguage);
			SelectedSourceLanguageIndex = sourceLangIdx >= 0 ? sourceLangIdx : 0;
			SelectedTargetLanguageIndex = targetLangIdx >= 0 ? targetLangIdx : 0;
			SearchText = info.Text;
		}

		private async void VmPropertyChanged(string name)
		{
			if (!_loaded)
				return;
			switch (name)
			{
				case nameof(SelectedDbIndex):
					if (SelectedDbIndex >= 0)
						searcher_ = new TmxSearch(new TmxMongoDb("localhost:27017", Databases[SelectedDbIndex]));
					await searcher_.LoadLanguagesAsync();
					Languages = searcher_.SupportedLanguages();
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
