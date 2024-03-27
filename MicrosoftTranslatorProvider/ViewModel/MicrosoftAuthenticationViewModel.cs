using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using MicrosoftTranslatorProvider.Service;
using MicrosoftTranslatorProvider.Commands;
using MicrosoftTranslatorProvider.Helpers;
using MicrosoftTranslatorProvider.Interface;
using MicrosoftTranslatorProvider.Interfaces;
using MicrosoftTranslatorProvider.Model;

namespace MicrosoftTranslatorProvider.ViewModel
{
	public class MicrosoftAuthenticationViewModel : BaseViewModel, IAuthenticationView
	{
		readonly ITranslationOptions _translationOptions;

		string _apiKey;
		AccountRegion _selectedRegion;

		public MicrosoftAuthenticationViewModel(ITranslationOptions translationOptions)
		{
			_translationOptions = translationOptions;
			Regions = RegionsProvider.GetSubscriptionRegions();
			InitializeCommands();
			LoadCredentials();
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

		private void LoadCredentials()
		{
			if (_translationOptions.MicrosoftCredentials is null)
			{
				SelectedRegion = Regions.First();
				return;
			}

			SelectedRegion = Regions.FirstOrDefault(x => x.Name.Equals(_translationOptions.MicrosoftCredentials?.Region ?? string.Empty));
			ApiKey = _translationOptions.MicrosoftCredentials.APIKey;
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
				_translationOptions.MicrosoftCredentials = credentials;
				CloseRequested.Invoke(null, EventArgs.Empty);
			}
		}
	}
}