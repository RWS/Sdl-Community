using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using GoogleCloudTranslationProvider.Commands;
using GoogleCloudTranslationProvider.Interfaces;

namespace GoogleCloudTranslationProvider.ViewModel
{
	public class ProviderSelectorViewModel : BaseViewModel
	{
		public ProviderSelectorViewModel(IEnumerable<ITranslationOptions> translationOptions)
		{
			TranslationOptions = new(translationOptions);
			SelectProviderCommand = new RelayCommand(SelectProvider);
			CloseWindowCommand = new RelayCommand(CloseWindow);
		}

		public ObservableCollection<ITranslationOptions> TranslationOptions { get; set; }

		public ITranslationOptions SelectedProvider { get; set; }

		public ICommand SelectProviderCommand { get; set; }

		public ICommand CloseWindowCommand { get; set; }

		public delegate void CloseWindowEventRaiser();

		public event CloseWindowEventRaiser CloseEventRaised;

		private void SelectProvider(object parameter)
		{
			if (parameter is not ITranslationOptions selectedProvider)
			{
				return;
			}

			SelectedProvider = selectedProvider;
			CloseWindow(null);
		}

		private void CloseWindow(object parameter)
		{
			CloseEventRaised?.Invoke();
		}
	}
}