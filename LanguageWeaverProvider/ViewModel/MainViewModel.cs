﻿using System.Windows.Input;
using LanguageWeaverProvider.Command;
using LanguageWeaverProvider.Model.Options.Interface;
using LanguageWeaverProvider.ViewModel.Cloud;
using LanguageWeaverProvider.ViewModel.Interface;

namespace LanguageWeaverProvider.ViewModel
{
	public class MainViewModel : BaseViewModel
	{
		private IMainProviderViewModel _providerView;

		private bool _isEdgeSelected;
		private bool _isCloudSelected;

		public MainViewModel(ITranslationOptions options, bool editProvider = false)
		{
			TranslationOptions = options;
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

		public IMainProviderViewModel ProviderView
		{
			get => _providerView;
			set
			{
				if (_providerView == value) return;
				_providerView = value;
				OnPropertyChanged();
			}
		}

		public ICommand CloseCommand { get; private set; }

		public ICommand SelectLanguageWeaverServiceCommand { get; private set; }

		public ICommand BackCommand { get; private set; }

		public delegate void CloseWindowEventRaiser();

		public event CloseWindowEventRaiser CloseEventRaised;

		private void InitializeCommands()
		{
			CloseCommand = new RelayCommand(CloseApplication);
			SelectLanguageWeaverServiceCommand = new RelayCommand(SelectLanguageWeaverService);
			BackCommand = new RelayCommand(Back);
		}

		private void SelectLanguageWeaverService(object parameter)
		{
			if (parameter is not string requestedService)
			{
				return;
			}

			IsCloudSelected = requestedService == "Cloud";
			IsEdgeSelected = requestedService == "Edge";
			ProviderView = IsCloudSelected ? new CloudMainViewModel(TranslationOptions) : null;
		}

		private void CloseApplication(object parameter)
		{
			SaveChanges = false;
			CloseEventRaised?.Invoke();
		}

		private void Back(object parameter)
		{
			IsCloudSelected = false;
			IsEdgeSelected = false;
		}
	}
}