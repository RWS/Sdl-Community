using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using LanguageWeaverProvider.Command;
using LanguageWeaverProvider.Extensions;
using LanguageWeaverProvider.Model.Interface;
using LanguageWeaverProvider.Model.Options;
using LanguageWeaverProvider.Services;
using LanguageWeaverProvider.Services.Model;
using Newtonsoft.Json;
using Sdl.ProjectAutomation.FileBased;
using Sdl.TranslationStudioAutomation.IntegrationApi;

namespace LanguageWeaverProvider.ViewModel
{
	public class AccountSubscriptionViewModel : BaseViewModel
	{
		readonly FileBasedProject _projectController;

		CloudUsageReport _usageReport;
		AccountCategoryFeature _subscriptionCharactersLimit;
		string _loadingAction;
		List<ITranslationOptions> _providers;
		ITranslationOptions _currentProvider;

		public AccountSubscriptionViewModel()
		{
			_projectController = SdlTradosStudio.Application.GetController<ProjectsController>().CurrentProject;
			Initialize();
		}

		public CloudUsageReport UsageReport
		{
			get => _usageReport;
			set
			{
				_usageReport = value;
				OnPropertyChanged();
			}
		}

		public AccountCategoryFeature SubscriptionCharactersLimit
		{
			get => _subscriptionCharactersLimit;
			set
			{
				_subscriptionCharactersLimit = value;
				OnPropertyChanged();
			}
		}

		public string LoadingAction
		{
			get => _loadingAction;
			set
			{
				_loadingAction = value;
				OnPropertyChanged();
			}
		}

		public List<ITranslationOptions> Providers
		{
			get => _providers;
			set
			{
				_providers = value;
				OnPropertyChanged();
			}
		}

		public ITranslationOptions CurrentProvider
		{
			get => _currentProvider;
			set
			{
				_currentProvider = value;
				OnPropertyChanged();
			}
		}

		public ICommand RefreshViewCommand { get; set; }

		public ICommand GetSubscriptionInfoCommand { get; set; }

		private void Initialize()
		{
			RefreshViewCommand = new RelayCommand(RefreshView);
			GetSubscriptionInfoCommand = new AsyncRelayCommand(GetSubscriptionInfo);
			GetProviders();
		}

		private void RefreshView(object parameter)
		{
			GetProviders();
		}

		private async Task GetSubscriptionInfo()
		{
			try
			{
				var accessToken = _currentProvider.AccessToken;
				LoadingAction = "Contacting the server...";
				var accountDetails = await CloudService.GetAccountDetails(accessToken);
				if (accountDetails.Errors?.Any() == true)
				{
					ErrorHandling.ShowDialog(null, null, accountDetails.Errors[0].Description);
					return;
				}

				LoadingAction = "Loading usage reports...";
				UsageReport = await CloudService.GetUsageReport(accessToken, accountDetails.Subscriptions);
				LoadingAction = "Calculating...";
				SubscriptionCharactersLimit = (await CloudService.GetSubscriptionDetails(accessToken)).First(sub => sub.Name.Equals("characters", System.StringComparison.InvariantCultureIgnoreCase));
			}
			finally
			{
				LoadingAction = null;
			}
		}

		private void GetProviders()
		{
			Providers = _projectController
				.GetTranslationProviderConfiguration()
				.Entries
				.Where(entry => entry.MainTranslationProvider.Uri.AbsoluteUri.StartsWith(Constants.BaseTranslationScheme))
				.Select(entry => JsonConvert.DeserializeObject<TranslationOptions>(entry.MainTranslationProvider.State) as ITranslationOptions)
				.ToList();

			foreach (var provider in Providers)
			{
				CredentialManager.GetCredentials(provider, true);
				if (provider.AccessToken is not null)
				{
					Service.ValidateToken(provider);
				}
			}

			CurrentProvider = _providers?.FirstOrDefault();
			if (CurrentProvider.AccessToken is not null)
			{
				GetSubscriptionInfoCommand.Execute(null);
			}
		}
	}
}