using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using Sdl.Community.MTCloud.Languages.Provider.Interfaces;
using Sdl.Community.MTCloud.Languages.Provider.Model;
using Sdl.Community.MTCloud.Provider.Commands;
using Sdl.Community.MTCloud.Provider.Helpers;
using Sdl.Core.Globalization.LanguageRegistry;
using Application = System.Windows.Forms.Application;

namespace Sdl.Community.MTCloud.Provider.ViewModel
{
	public class MTCodesViewModel : BaseViewModel
	{
		private readonly ILanguageProvider _languageProvider;

		private ICommand _saveCommand;
		private ICommand _resetToDefaultsCommand;

		private MappedLanguage _selectedMappedLanguage;
		private List<MappedLanguage> _mappedLanguages;
		private string _message;
		private string _messageColor;
		private string _query;
		private bool _isWaiting;
		private string _itemsCountLabel;

		public MTCodesViewModel(Window owner, ILanguageProvider languageProvider)
		{
			Owner = owner;
			_languageProvider = languageProvider;
			MappedLanguages = new List<MappedLanguage>(GetAllMappedLanguages(false));
		}

		public ICommand SaveCommand => _saveCommand ??= new RelayCommand(Save);

		public ICommand ResetToDefaultsCommand => _resetToDefaultsCommand ??= new RelayCommand(ResetToDefaults);

		public Window Owner { get; }

		public List<MappedLanguage> MappedLanguages
		{
			get => _mappedLanguages;
			set
			{
				_mappedLanguages = value;

				OnPropertyChanged(nameof(MappedLanguages));
				ItemsCountLabel = string.Format(PluginResources.Total_Languages, _mappedLanguages.Count);
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

		public string MessageColor
		{
			get => _messageColor;
			set
			{
				_messageColor = value;
				OnPropertyChanged(nameof(MessageColor));
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

		public bool IsWaiting
		{
			get => _isWaiting;
			set
			{
				_isWaiting = value;
				OnPropertyChanged(nameof(IsWaiting));
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
				collectionViewSource.Filter = language =>
				{
					var mappedLanguage = language as MappedLanguage;

					return (mappedLanguage != null && mappedLanguage.Name.ToLower().Contains(query.ToLower()))
						|| (mappedLanguage != null && mappedLanguage.TradosCode.ToLower().Contains(query.ToLower()))
						|| (mappedLanguage != null && mappedLanguage.Region.ToLower().Contains(query.ToLower()))
						|| (mappedLanguage != null && mappedLanguage.MTCode.ToLower().Contains(query.ToLower()))
						|| (mappedLanguage != null && mappedLanguage.MTCodeLocale.ToLower().Contains(query.ToLower()));
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
				? string.Format(PluginResources.Total_And_Filtered_Languages, totalCount, filteredCount)
				: string.Format(PluginResources.Total_Languages, totalCount);
		}

		private IEnumerable<MappedLanguage> GetAllMappedLanguages(bool reset)
		{
			var mappedLanguages = _languageProvider.GetMappedLanguages(reset);
			if (AddStudioLanguages(mappedLanguages))
			{
				_languageProvider.SaveMappedLanguages(mappedLanguages);
			}

			return mappedLanguages;
		}

		private bool AddStudioLanguages(ICollection<MappedLanguage> mappedLanguages)
		{
			var updated = false;

			var studioLanguages = LanguageRegistryApi.Instance.GetAllLanguages();

			foreach (var studioLanguage in studioLanguages)
			{
				var mappedLanguage = mappedLanguages.FirstOrDefault(e => e.TradosCode.Equals(studioLanguage.CultureInfo.Name));
				if (mappedLanguage == null)
				{
					var languageName = GetLanguageName(studioLanguage, out var region);

					updated = true;
					var language = new MappedLanguage
					{
						Index = mappedLanguages.Count,
						Name = languageName,
						Region = region,
						TradosCode = studioLanguage.CultureInfo.Name,
						MTCode = string.Empty,
						MTCodeLocale = string.Empty
					};

					mappedLanguages.Add(language);
				}
			}

			return updated;
		}

		private string GetLanguageName(Core.Globalization.Language language, out string region)
		{
			region = string.Empty;
			if (language == null)
			{
				return null;
			}

			var languageName = language.DisplayName;
			if (!string.IsNullOrEmpty(languageName))
			{
				var regexSplit = new Regex(@"(?<language>[^\(]*)\((?<region>[^\)]*)", RegexOptions.IgnoreCase);
				if (languageName.Contains("(") && languageName.Contains(")"))
				{
					var match = regexSplit.Match(languageName);
					if (match.Success)
					{
						languageName = match.Groups["language"].Value.Trim();
						region = match.Groups["region"].Value.Trim();
					}
				}
			}

			return languageName;
		}

		private void ResetToDefaults(object obj)
		{
			MappedLanguages = new List<MappedLanguage>(GetAllMappedLanguages(true));

			MessageBox.Show(PluginResources.Message_Successfully_reset_to_defaults,
				Application.ProductName, MessageBoxButton.OK, MessageBoxImage.Information);
		}

		private void Save(object obj)
		{
			_languageProvider.SaveMappedLanguages(MappedLanguages.ToList());

			WindowCloser.SetDialogResult(Owner, true);
			Owner.Close();
		}
	}
}