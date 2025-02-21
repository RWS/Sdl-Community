using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace TMX_SimpleSearch
{
	public class SearchItem : INotifyPropertyChanged
	{
		private string _targetLanguage;
		private string _sourceLanguage;
		private SearchType _searchType = SearchType.Exact;
		private string _text = "";
		private int _searchTimeSeconds = -1;

		public SearchItem Copy()
		{
			return new SearchItem
			{
				Text = Text, 
				SearchType = SearchType, 
				SearchTimeSeconds = SearchTimeSeconds, 
				SourceLanguage = SourceLanguage, 
				TargetLanguage = TargetLanguage,
			};
		}

		public string Text
		{
			get => _text;
			set
			{
				if (value == _text) return;
				_text = value;
				OnPropertyChanged();
			}
		}

		public SearchType SearchType
		{
			get => _searchType;
			set
			{
				if (value == _searchType) return;
				_searchType = value;
				OnPropertyChanged();
			}
		}


		public string SourceLanguage
		{
			get => _sourceLanguage;
			set
			{
				if (value == _sourceLanguage) return;
				_sourceLanguage = value;
				OnPropertyChanged();
			}
		}

		public string TargetLanguage
		{
			get => _targetLanguage;
			set
			{
				if (value == _targetLanguage) return;
				_targetLanguage = value;
				OnPropertyChanged();
			}
		}

		public int SearchTimeSeconds
		{
			get => _searchTimeSeconds;
			set
			{
				if (value == _searchTimeSeconds) return;
				_searchTimeSeconds = value;
				OnPropertyChanged();
			}
		}


		public event PropertyChangedEventHandler PropertyChanged;

		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
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