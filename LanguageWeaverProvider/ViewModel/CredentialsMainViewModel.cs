using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Input;
using LanguageWeaverProvider.Command;
using LanguageWeaverProvider.Extensions;
using LanguageWeaverProvider.Model.Interface;
using LanguageWeaverProvider.ViewModel.Cloud;
using LanguageWeaverProvider.ViewModel.Edge;
using LanguageWeaverProvider.ViewModel.Interface;

namespace LanguageWeaverProvider.ViewModel
{
	public class CredentialsMainViewModel : BaseViewModel
	{
		const string ProviderView_Cloud = nameof(CloudCredentialsViewModel);
		const string ProviderView_Edge = nameof(EdgeCredentialsViewModel);

		bool _isEdgeSelected;
		bool _isCloudSelected;
		bool _isUserAttemptingLogin;
		string _currentActionMessage;
		ICredentialsViewModel _providerView;

		public CredentialsMainViewModel(ITranslationOptions options)
		{
			TranslationOptions = options;
			IsUserAttemptingLogin = false;
			InitializeCommands();
		}

		public ITranslationOptions TranslationOptions { get; set; }

		public bool SaveChanges { get; set; }

		public bool IsCloudSelected
		{
			get => _isCloudSelected;
			set
			{
				if (_isCloudSelected == value) return;
				_isCloudSelected = value;
				OnPropertyChanged();
				OnPropertyChanged(nameof(IsServiceSelected));
			}
		}

		public bool IsEdgeSelected
		{
			get => _isEdgeSelected;
			set
			{
				if (_isEdgeSelected == value) return;
				_isEdgeSelected = value;
				OnPropertyChanged();
				OnPropertyChanged(nameof(IsServiceSelected));
			}
		}

		public bool IsServiceSelected => IsCloudSelected || IsEdgeSelected;

		public bool IsUserAttemptingLogin
		{
			get => _isUserAttemptingLogin;
			set
			{
				_isUserAttemptingLogin = value;
				OnPropertyChanged();
			}
		}

		public string CurrentActionMessage
		{
			get => _currentActionMessage;
			set
			{
				_currentActionMessage = value;
				OnPropertyChanged();
			}
		}

		public ICredentialsViewModel ProviderView
		{
			get => _providerView;
			set
			{
				if (_providerView == value) return;
				_providerView = value;
				OnPropertyChanged();
			}
		}

		public ICommand BackCommand { get; private set; }

		public ICommand CloseCommand { get; private set; }

		public ICommand OpenHyperlinkCommand { get; private set; }

		public ICommand SelectLanguageWeaverServiceCommand { get; private set; }

		public delegate void CloseWindowEventRaiser();

		public event CloseWindowEventRaiser CloseEventRaised;

		private void InitializeCommands()
		{
			BackCommand = new RelayCommand(Back);
			CloseCommand = new RelayCommand(CloseApplication);
			OpenHyperlinkCommand = new RelayCommand(OpenHyperlink);
			SelectLanguageWeaverServiceCommand = new RelayCommand(SelectLanguageWeaverService);
		}

		private void Back(object parameter)
		{
			IsCloudSelected = false;
			IsEdgeSelected = false;
		}

		private void CloseApplication(object parameter)
		{
			SaveChanges = false;
			CloseEventRaised?.Invoke();
		}

		private void OpenHyperlink(object parameter)
		{
			Process.Start(parameter as string);
		}

		private void SelectLanguageWeaverService(object parameter)
		{
			if (parameter is not string requestedService)
			{
				return;
			}

			IsCloudSelected = requestedService == Constants.CloudService;
			IsEdgeSelected = requestedService == Constants.EdgeService;
			ICredentialsViewModel selectedViewModel = IsCloudSelected
													? new CloudCredentialsViewModel(TranslationOptions)
													: new EdgeCredentialsViewModel(TranslationOptions);
			selectedViewModel.CloseRequested += CloseCredentialsViewRequest;
			selectedViewModel.StartLoginProcess += StartLoginProcess;
			selectedViewModel.StopLoginProcess += StopLoginProcess;
			ProviderView = selectedViewModel;
		}

		private void CloseCredentialsViewRequest(object sender, EventArgs e)
		{
			ProviderView = null;
			SaveChanges = true;
			CloseEventRaised?.Invoke();
		}

		private async void StartLoginProcess(object sender, EventArgs e)
		{
			IsUserAttemptingLogin = true;
			CurrentActionMessage = PluginResources.Connection_Loading_Initiating;
			await Task.Delay(2000);
			var loginEventArgs = e as LoginEventArgs;
			CurrentActionMessage = loginEventArgs.Message;
		}

		private async void StopLoginProcess(object sender, EventArgs e)
		{
			if (e is not LoginEventArgs loginEventArgs)
			{
				IsUserAttemptingLogin = false;
				return;
			}

			CurrentActionMessage = loginEventArgs.Message;
			await Task.Delay(3000);
			IsUserAttemptingLogin = false;
		}
	}
}