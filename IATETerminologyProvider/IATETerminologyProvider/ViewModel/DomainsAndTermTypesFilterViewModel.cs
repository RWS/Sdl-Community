using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using NLog;
using Sdl.Community.IATETerminologyProvider.Helpers;
using Sdl.Community.IATETerminologyProvider.Interface;
using Sdl.Community.IATETerminologyProvider.Model;
using Sdl.Community.IATETerminologyProvider.Model.ResponseModels;

namespace Sdl.Community.IATETerminologyProvider.ViewModel
{
	public class DomainsAndTermTypesFilterViewModel : ViewModelBase, ISettingsViewModel, IDisposable
	{
		private readonly Logger _logger = LogManager.GetCurrentClassLogger();
		private List<DomainModel> _domains = new List<DomainModel>();
		private bool _isEnabled;
		private bool _isLoading;
		private int _maxEntries;
		private bool _searchInSubdomains;
		private DomainModel _selectedDomain;
		private TermTypeModel _selectedTermType;
		private SettingsModel _settings;
		private List<TermTypeModel> _termTypes = new List<TermTypeModel>();

		public DomainsAndTermTypesFilterViewModel()
		{
			PropertyChanged += DomainsAndTermTypesFilterViewModel_PropertyChanged;
		}

		public bool AllDomainsChecked
		{
			get => AreAllDomainsSelected();
			set
			{
				SwitchAllDomains(value);
				OnPropertyChanged(nameof(AllDomainsChecked));
			}
		}

		public bool AllTermTypesChecked
		{
			get => AreAllTypesSelected();
			set
			{
				SwitchAllTermTypes(value);
				OnPropertyChanged(nameof(AllTermTypesChecked));
			}
		}

		public List<DomainModel> Domains
		{
			get => _domains;
			set
			{
				if (_domains.Any())
				{
					foreach (var domain in _domains)
					{
						domain.PropertyChanged -= DomainsOnPropertyChanged;
					}
				}

				_domains = value;

				if (_domains.Any())
				{
					foreach (var domain in _domains)
					{
						domain.PropertyChanged += DomainsOnPropertyChanged;
					}
				}

				OnPropertyChanged(nameof(Domains));
			}
		}

		public bool IsEnabled
		{
			get => _isEnabled;
			set
			{
				if (_isEnabled == value)
				{
					return;
				}

				_isEnabled = value;
				OnPropertyChanged(nameof(IsEnabled));
			}
		}

		public bool IsLoading
		{
			get => _isLoading;
			set
			{
				if (_isLoading == value)
				{
					return;
				}

				_isLoading = value;
				IsEnabled = !_isLoading;

				OnPropertyChanged(nameof(IsLoading));
			}
		}

		public int MaxEntries
		{
			get => _maxEntries;
			set
			{
				if (_maxEntries == value)
				{
					return;
				}

				_maxEntries = value;
				OnPropertyChanged(nameof(MaxEntries));
			}
		}

		public bool SearchInSubdomains
		{
			get => _searchInSubdomains;
			set
			{
				if (_searchInSubdomains == value)
				{
					return;
				}

				_searchInSubdomains = value;

				OnPropertyChanged(nameof(SearchInSubdomains));
			}
		}

		public DomainModel SelectedDomain
		{
			get => _selectedDomain;
			set
			{
				_selectedDomain = value;
				OnPropertyChanged(nameof(SelectedDomain));
			}
		}

		public TermTypeModel SelectedTermType
		{
			get => _selectedTermType;
			set
			{
				_selectedTermType = value;

				OnPropertyChanged(nameof(SelectedTermType));
			}
		}

		public SettingsModel Settings
		{
			get => _settings;
			set
			{
				_settings = value;
				OnPropertyChanged(nameof(Settings));
			}
		}

		public List<TermTypeModel> TermTypes
		{
			get => _termTypes;
			set
			{
				if (_termTypes.Any())
				{
					foreach (var termType in _termTypes)
					{
						termType.PropertyChanged -= TermTypesOnPropertyChanged;
					}
				}

				_termTypes = value;

				if (_termTypes.Any())
				{
					foreach (var termType in _termTypes)
					{
						termType.PropertyChanged += TermTypesOnPropertyChanged;
					}
				}

				OnPropertyChanged(nameof(TermTypes));
			}
		}

		public void Dispose()
		{
			UnSubscribeToEvents();
		}

		public void Reset()
		{
			ResetDomains();
			ResetTypes();
			SearchInSubdomains = false;
		}

		public void Setup()
		{
			LoadDomains();
			LoadTermTypes();

			SetFieldsSelection();
		}

		private bool AreAllDomainsSelected()
		{
			return Domains.Count > 0 && Domains.All(d => d.IsSelected);
		}

		private bool AreAllTypesSelected()
		{
			return TermTypes.Count > 0 && TermTypes.All(t => t.IsSelected);
		}

		private void DomainsAndTermTypesFilterViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == nameof(Settings))
			{
				Setup();
			}
		}

		private void DomainsOnPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName != "IsSelected")
			{
				return;
			}

			OnPropertyChanged(nameof(AllDomainsChecked));
		}

		private void LoadDomains()
		{
			try
			{
				if (IATEApplication.InventoriesProvider.Domains?.Count > 0)
				{
					SetDomains();
				}
			}
			catch (InvalidAsynchronousStateException e)
			{
				_logger.Error(e);
			}
		}

		private void LoadTermTypes()
		{
			if (IATEApplication.InventoriesProvider.TermTypes?.Count > 0)
			{
				SetTermTypes(IATEApplication.InventoriesProvider.TermTypes);
			}
		}

		private void ResetDomains()
		{
			foreach (var domain in Domains)
			{
				domain.IsSelected = false;
			}

			OnPropertyChanged(nameof(AllDomainsChecked));
		}

		private void ResetTypes()
		{
			foreach (var type in TermTypes)
			{
				type.IsSelected = false;
			}

			OnPropertyChanged(nameof(AllTermTypesChecked));
		}

		private void SwitchAllDomains(bool select)
		{
			foreach (var domain in Domains)
			{
				domain.IsSelected = select;
			}
		}

		private void SwitchAllTermTypes(bool select)
		{
			foreach (var termType in TermTypes)
			{
				termType.IsSelected = select;
			}
		}

		private void SetDomains()
		{
			var domains = new List<DomainModel>();
			foreach (var domain in IATEApplication.InventoriesProvider.Domains)
			{
				if (domain.Name.Equals(Constants.NotSpecifiedCode)) domain.EurovocCode = "00";

				var selectedDomainName = Utils.UppercaseFirstLetter(domain.Name.ToLower());

				var discriminator = "";
				if (!string.IsNullOrWhiteSpace(domain.CjeuCode)) discriminator = "CJEU";

				var domainModel = new DomainModel
				{
					Code = domain.Code,
					Name = $"{domain.EurovocCode ?? domain.CjeuCode} {selectedDomainName} {discriminator}"
				};

				domains.Add(domainModel);
			}

			Domains = domains;
		}

		private void SetFieldsSelection()
		{
			if (Settings is null)
			{
				return;
			}

			if (Settings.Domains?.Count > 0)
			{
				Domains = Settings.Domains;
			}

			if (Settings.TermTypes?.Count > 0)
			{
				TermTypes = Settings.TermTypes;
			}

			SearchInSubdomains = Settings.SearchInSubdomains;
		}

		private void SetTermTypes(List<ItemsResponseModel> termTypesResponse)
		{
			var termTypes = new List<TermTypeModel>();
			foreach (var item in termTypesResponse)
			{
				var selectedTermTypeName = Utils.UppercaseFirstLetter(item.Name.ToLower());

				var termType = new TermTypeModel
				{
					Code = int.TryParse(item.Code, out _) ? int.Parse(item.Code) : 0,
					Name = selectedTermTypeName
				};

				termTypes.Add(termType);
			}

			TermTypes = new List<TermTypeModel>(termTypes);
		}

		private void TermTypesOnPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName != "IsSelected")
			{
				return;
			}

			OnPropertyChanged(nameof(AllTermTypesChecked));
		}

		private void UnSubscribeToEvents()
		{
			foreach (var domain in Domains)
			{
				domain.PropertyChanged -= DomainsOnPropertyChanged;
			}

			foreach (var termType in TermTypes)
			{
				termType.PropertyChanged -= TermTypesOnPropertyChanged;
			}
		}
	}
}