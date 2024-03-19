using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using MicrosoftTranslatorProvider.ApiService;
using MicrosoftTranslatorProvider.Commands;
using MicrosoftTranslatorProvider.Helpers;
using MicrosoftTranslatorProvider.Interface;
using MicrosoftTranslatorProvider.Interfaces;
using MicrosoftTranslatorProvider.Model;

namespace MicrosoftTranslatorProvider.ViewModel
{
	public class MicrosoftAuthenticationViewModel : BaseModel, IAuthenticationView
	{
		ITranslationOptions _translationOptions;

		string _apiKey;
		AccountRegion _selectedRegion;

		public MicrosoftAuthenticationViewModel(ITranslationOptions translationOptions)
		{
			_translationOptions = translationOptions;
			Regions = RegionsProvider.GetSubscriptionRegions();
			SelectedRegion = Regions.FirstOrDefault(x => x.Name.Equals(translationOptions.MicrosoftCredentials?.Region ?? string.Empty));
			InitializeCommands();
		}

		public List<AccountRegion> Regions { get; private set; }

		public string ApiKey
		{
			get => _apiKey;
			set
			{
				if (_apiKey == value) return;
				_apiKey = value;
				OnPropertyChanged();
			}
		}

		public AccountRegion SelectedRegion
		{
			get => _selectedRegion;
			set
			{
				if (_selectedRegion == value) return;
				_selectedRegion = value;
				OnPropertyChanged();
			}
		}

		public ICommand SignInCommand { get; private set; }

		public event EventHandler CloseRequested;

		private void InitializeCommands()
		{
			SignInCommand = new RelayCommand(SignIn);
		}

		private async void SignIn(object parameter)
		{
			var credentials = new MicrosoftCredentials()
			{
				APIKey = ApiKey,
				Region = SelectedRegion.Name,
			};

			var successful = await MicrosoftService.AuthenticateUser(credentials);
			if (successful)
			{
				CloseRequested.Invoke(null, EventArgs.Empty);
			}
		}
	}
}