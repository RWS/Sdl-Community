using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Input;
using Newtonsoft.Json;
using Sdl.Community.IATETerminologyProvider.Commands;
using Sdl.Community.IATETerminologyProvider.Interface;
using Sdl.Community.IATETerminologyProvider.Model;
using Sdl.Community.IATETerminologyProvider.Service;

namespace Sdl.Community.IATETerminologyProvider.ViewModel
{
	public class MainWindowViewModel
	{
		private ICommand _clearCache;
		private SettingsModel _providerSettings;
		private ICommand _resetToDefault;
		private ICommand _saveSettingsCommand;

		public MainWindowViewModel(List<ISettingsViewModel> viewModels, SettingsModel settingsModel)
		{
			//TODO maybe this viewModel should take care of the data initialization and distribution to the other VMs
			ViewModels = viewModels;
			ProviderSettings = settingsModel;
		}

		public ICommand ClearCache => _clearCache ?? (_clearCache = new CommandHandler(Clear, true));

		public SettingsModel ProviderSettings
		{
			get => _providerSettings;
			set
			{
				_providerSettings = value;
				ViewModels.ForEach(vm => vm.Settings = _providerSettings);
			}
		}

		public ICommand ResetToDefault => _resetToDefault ?? (_resetToDefault = new CommandHandler(Reset, true));

		public ICommand SaveSettingsCommand => _saveSettingsCommand ?? (_saveSettingsCommand = new CommandHandler(SaveSettingsAction, true));

		public List<ISettingsViewModel> ViewModels { get; set; }

		public async void Setup()
		{
			//This class' business is not to init the data classes
			//TODO: Do it elsewhere
			//if (!_inventoriesProvider.IsInitialized)
			{
				try
				{
					//IsLoading = true; TODO:Create event
					//await _inventoriesProvider.Initialize();
				}
				finally
				{
					//IsLoading = false; TODO:Create event
				}
			}

			//TODO: Give each view model its data
			//ViewModels.ForEach(vm => vm.Setup(Data))

			//LoadDomains();
			//LoadTermTypes();

			//SetFieldsSelection();

			//IsEnabled = true;
		}

		private void Clear()
		{
			var result = IATEApplication.MessageBoxService.ShowYesNoMessageBox("", PluginResources.ClearConfirmation);
			if (result != MessageDialogResult.Yes)
			{
				return;
			}

			IATEApplication.CacheProvider?.ClearCachedResults();
		}

		private void Reset()
		{
			ViewModels.ForEach(vm => vm.Reset());
		}

		private void SaveSettingsAction()
		{

			foreach (var viewModel in ViewModels)
			{
				switch (viewModel)
				{
					case DomainsAndTermTypesFilterViewModel domainsAndTermTypes:
					{
						if (domainsAndTermTypes.Domains.Count > 0)
						{
							ProviderSettings.Domains = domainsAndTermTypes.Domains;
						}
						if (domainsAndTermTypes.TermTypes.Count > 0)
						{
							ProviderSettings.TermTypes = domainsAndTermTypes.TermTypes;
						}
						ProviderSettings.SearchInSubdomains = domainsAndTermTypes.SearchInSubdomains;
						break;

					}
					case FineGrainedFilterViewModel fineGrainedFilter:
					{
						if (fineGrainedFilter.Collections.Count > 0)
						{
							ProviderSettings.Collections = fineGrainedFilter.Collections.Where(c => c.IsSelected).ToList();
						}
						
						if (fineGrainedFilter.Institutions.Count > 0)
						{
							ProviderSettings.Institutions = fineGrainedFilter.Institutions.Where(i => i.IsSelected).ToList();
						}

						ProviderSettings.Primary = fineGrainedFilter.Primary;
						ProviderSettings.NotPrimary = fineGrainedFilter.NotPrimary;

						ProviderSettings.SourceReliabilities = fineGrainedFilter.SourceReliabilities;
						ProviderSettings.TargetReliabilities = fineGrainedFilter.TargetReliabilities;

						break;
					}
				}

				SettingsService.SaveSettingsForCurrentProject(ProviderSettings);
			}


			//if (ProviderSettings.Domains.Count > 0)
			//{
			//	ProviderSettings.Domains = (ViewModels[0] as DomainsAndTermTypesFilterViewModel).Domains.ToList();
			//}


			//if (ProviderSettings.TermTypes.Count > 0)
			//{
			//	ProviderSettings.TermTypes = ProviderSettings.TermTypes.ToList();
			//}

			//ProviderSettings.SearchInSubdomains = ProviderSettings.SearchInSubdomains;

			////UnSubscribeToEvents();
		}
	}
}