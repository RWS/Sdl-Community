using System;
using System.Diagnostics;
using System.Linq;
using System.Windows.Input;
using LanguageWeaverProvider.Command;
using LanguageWeaverProvider.Model.Interface;
using LanguageWeaverProvider.Model.Options;
using LanguageWeaverProvider.View;
using LanguageWeaverProvider.ViewModel.Cloud;
using LanguageWeaverProvider.ViewModel.Interface;
using Sdl.LanguagePlatform.Core;

namespace LanguageWeaverProvider.ViewModel
{
	public class PairMappingViewModel : BaseViewModel
	{
		private readonly LanguagePair[] _languagePairs;
		private IPairMappingViewModel _pairMappingView;
		private ITranslationOptions _translationOptions;

		public PairMappingViewModel(ITranslationOptions translationOptions, LanguagePair[] languagePairs)
		{
			_languagePairs = languagePairs;
			_translationOptions = translationOptions;
			InitializeCommands();
			SelectPairMappingView();
		}

		public IPairMappingViewModel PairMappingView
		{
			get => _pairMappingView;
			set
			{
				if (_pairMappingView == value) return;
				_pairMappingView = value;
				OnPropertyChanged();
			}
		}

		public bool SaveChanges { get; private set; }


		public ICommand SaveCommand { get; private set; }

		public ICommand CloseCommand { get; private set; }

		public ICommand NavigateToCommand { get; private set; }

		public ICommand OpenSettingsViewCommand { get; private set; }

		public ICommand OpenLanguageMappingProviderViewCommand { get; private set; }

		public delegate void CloseWindowEventRaiser();

		public event CloseWindowEventRaiser CloseEventRaised;

		private void InitializeCommands()
		{
			SaveCommand = new RelayCommand(Save);
			CloseCommand = new RelayCommand(Close);
			NavigateToCommand = new RelayCommand(NavigateTo);
			OpenSettingsViewCommand = new RelayCommand(OpenSettingsView);
			OpenLanguageMappingProviderViewCommand = new RelayCommand(OpenLanguageMappingProviderView);
		}

		private void SelectPairMappingView()
		{
			IPairMappingViewModel pairMappingView = _translationOptions.Version == PluginVersion.LanguageWeaverCloud
												  ? new CloudMappingViewModel(_translationOptions, _languagePairs)
												  : new CloudMappingViewModel(_translationOptions, _languagePairs);
			PairMappingView = pairMappingView;
		}

		private void Save(object parameter)
		{
			SaveChanges = true;
			_translationOptions.PairMappings = PairMappingView.PairMappings.ToList();
			CloseEventRaised.Invoke();
		}

		private void Close(object parameter)
		{
			SaveChanges = false;
			CloseEventRaised.Invoke();
		}

		private void NavigateTo(object parameter)
		{
			if (parameter is not string uriTarget)
			{
				return;
			}

			Process.Start(uriTarget);
		}

		private void OpenSettingsView(object parameter)
		{

		}

		private void OpenLanguageMappingProviderView(object parameter)
		{
			var lmpViewModel = new LanguageMappingProviderViewModel(_translationOptions);
			var lmpView = new LanguageMappingProviderView() { DataContext = lmpViewModel };
			lmpViewModel.CloseEventRaised += lmpView.Close;

			var dialog = lmpView.ShowDialog();
		}
	}
}