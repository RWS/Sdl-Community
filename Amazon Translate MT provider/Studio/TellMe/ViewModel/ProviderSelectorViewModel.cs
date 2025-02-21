using Sdl.Community.AmazonTranslateTradosPlugin.Command;
using Sdl.Community.AmazonTranslateTradosPlugin.Model;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Sdl.Community.AmazonTranslateTradosPlugin.Studio.TellMe.ViewModel
{
    public class ProviderSelectorViewModel : BaseViewModel
    {
        public ProviderSelectorViewModel(IEnumerable<TranslationOptions> translationOptions)
        {
            TranslationOptions = new(translationOptions);
            SelectProviderCommand = new RelayCommand(SelectProvider);
            CloseWindowCommand = new RelayCommand(CloseWindow);
        }

        public ObservableCollection<TranslationOptions> TranslationOptions { get; set; }

        public TranslationOptions SelectedProvider { get; set; }

        public ICommand SelectProviderCommand { get; set; }

        public ICommand CloseWindowCommand { get; set; }

        public delegate void CloseWindowEventRaiser();
        public event CloseWindowEventRaiser CloseEventRaised;

        private void SelectProvider(object parameter)
        {
            if (parameter is not TranslationOptions selectedProvider)
            {
                return;
            }

            SelectedProvider = selectedProvider;
            CloseWindow();
        }

        private void CloseWindow(object parameter = null)
        {
            CloseEventRaised?.Invoke();
        }
    }
}