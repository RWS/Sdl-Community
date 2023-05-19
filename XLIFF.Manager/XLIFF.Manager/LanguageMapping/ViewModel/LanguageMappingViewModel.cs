using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using Sdl.Community.XLIFF.Manager.Commands;
using Sdl.Community.XLIFF.Manager.LanguageMapping.Interfaces;
using Sdl.Community.XLIFF.Manager.LanguageMapping.Model;
using Sdl.Community.XLIFF.Manager.Model;
using Sdl.Core.Globalization;
using Sdl.Core.Globalization.LanguageRegistry;
using Application = System.Windows.Forms.Application;

namespace Sdl.Community.XLIFF.Manager.LanguageMapping.ViewModel
{
	public class LanguageMappingViewModel : BaseModel
	{
		private readonly ILanguageProvider _languageProvider;
		private readonly List<Language> _languages;
		private ICommand _saveCommand;
		private ICommand _resetCommand;
		private ICommand _clearFilterCommand;
		private MappedLanguage _selectedMappedLanguage;
		private List<MappedLanguage> _mappedLanguages;		
		private string _message;
		private string _query;
		private string _itemsCountLabel;

		public LanguageMappingViewModel(Window owner, ILanguageProvider languageProvider)
		{
			Owner = owner;
			_languageProvider = languageProvider;
			_languages = LanguageRegistryApi.Instance.GetAllLanguages().ToList();
			MappedLanguages = new List<MappedLanguage>(GetAllMappedLanguages(false));
			Query = string.Empty;
		}

		public ICommand SaveCommand => _saveCommand ?? (_saveCommand = new CommandHandler(Save));

		public ICommand ResetCommand => _resetCommand ?? (_resetCommand = new CommandHandler(Reset));

		public ICommand ClearFilterCommand => _clearFilterCommand ?? (_clearFilterCommand = new CommandHandler(ClearFilter));

		public Window Owner { get; }

		public List<MappedLanguage> MappedLanguages
		{
			get => _mappedLanguages;
			set
			{
				_mappedLanguages = value;

				OnPropertyChanged(nameof(MappedLanguages));
				ItemsCountLabel = string.Format(PluginResources.StatusLabel_TotalLanguages, _mappedLanguages.Count);
			}
		}

		public MappedLanguage SelectedMappedLanguage
		{
			get => _selectedMappedLanguage;
			set
			{
				_selectedMappedLanguage = value;
				OnPropertyChanged(nameof(SelectedMappedLanguage));
			}
		}

		public string Message
		{
			get => _message;
			set
			{
				_message = value;
				OnPropertyChanged(nameof(Message));
			}
		}

		public string Query
		{
			get => _query;
			set
			{
				if (_query == value)
				{
					return;
				}

				_query = value;
				OnPropertyChanged(nameof(Query));

				SearchLanguages(_query);
			}
		}

		public string ItemsCountLabel
		{
			get => _itemsCountLabel;
			set
			{
				if (_itemsCountLabel == value)
				{
					return;
				}

				_itemsCountLabel = value;
				OnPropertyChanged(nameof(ItemsCountLabel));
			}
		}

		public void SearchLanguages(string query)
		{
			var collectionViewSource = CollectionViewSource.GetDefaultView(MappedLanguages);

			if (!string.IsNullOrEmpty(query))
			{
				query = query.ToLower();
				collectionViewSource.Filter = language =>
				{
					var mappedLanguage = language as MappedLanguage;

					return (mappedLanguage?.LanguageCode != null && mappedLanguage.LanguageCode.ToLower().Contains(query))
						   || (mappedLanguage?.LanguageDisplayName != null && mappedLanguage.LanguageDisplayName.ToLower().Contains(query))
						   || (mappedLanguage?.MappedCode != null && mappedLanguage.MappedCode.ToLower().Contains(query))
						   || (mappedLanguage?.CustomDisplayName != null && mappedLanguage.CustomDisplayName.ToLower().Contains(query));
				};

				SelectedMappedLanguage = collectionViewSource.CurrentItem as MappedLanguage;
			}
			else
			{
				collectionViewSource.Filter = null;
			}

			var filtered = collectionViewSource.Cast<MappedLanguage>().ToList();
			var filteredCount = filtered.Count;
			var totalCount = MappedLanguages.Count;
			ItemsCountLabel = filteredCount < totalCount
				? string.Format(PluginResources.StatusLabel_TotalLanguagesFiltered, totalCount, filteredCount)
				: string.Format(PluginResources.StatusLabel_TotalLanguages, totalCount);
		}

		private IEnumerable<MappedLanguage> GetAllMappedLanguages(bool reset)
		{
			var foundNewLanguage = false;
			var mappedLanguages = _languageProvider.GetMappedLanguages(reset);
			foreach (var language in _languages)
			{				
				var mappedLanguage = mappedLanguages.FirstOrDefault(a =>
					string.Compare(a.LanguageCode, language.LanguageCode, StringComparison.CurrentCultureIgnoreCase) == 0);
				if (mappedLanguage != null)
				{
					mappedLanguage.LanguageDisplayName = language.DisplayName;
				}
				else if (!mappedLanguages.Exists(a => a.LanguageCode == language.LanguageCode))
				{
					foundNewLanguage = true;
					mappedLanguages.Add(new MappedLanguage
					{
						LanguageCode = language.LanguageCode,
						LanguageDisplayName = language.DisplayName
					});
				}
			}

			if (foundNewLanguage)
			{
				_languageProvider.SaveMappedLanguages(mappedLanguages);
			}

			return mappedLanguages;
		}
		
		private void Reset(object obj)
		{
			MappedLanguages = new List<MappedLanguage>(GetAllMappedLanguages(true));

			MessageBox.Show(PluginResources.Messagel_ResetLanguagesSuccess,
				Application.ProductName, MessageBoxButton.OK, MessageBoxImage.Information);
		}

		private void Save(object obj)
		{
			_languageProvider.SaveMappedLanguages(MappedLanguages.ToList());
			Owner.Close();
		}

		private void ClearFilter(object obj)
		{
			Query = string.Empty;
		}
	}
}