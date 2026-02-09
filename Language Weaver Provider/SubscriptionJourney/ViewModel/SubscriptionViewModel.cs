using LanguageWeaverProvider.Model.Interface;
using LanguageWeaverProvider.SubscriptionJourney.Command;
using LanguageWeaverProvider.SubscriptionJourney.Model;
using LanguageWeaverProvider.SubscriptionJourney.Services;
using System;
using System.Windows.Input;

namespace LanguageWeaverProvider.SubscriptionJourney.ViewModel
{
    public class SubscriptionViewModel : BaseViewModel
    {
        private readonly ISubscriptionOptions _subscriptionOptions;
        private readonly IUriOpener _uriOpener;

        private bool _doNotShowThisAgain;

        public SubscriptionViewModel(
        string windowTitle,
            ISubscriptionOptions subscriptionOptions,
            IUriOpener uriOpener)
        {
            _subscriptionOptions = subscriptionOptions
                ?? throw new ArgumentNullException(nameof(subscriptionOptions));
            _uriOpener = uriOpener
                ?? throw new ArgumentNullException(nameof(uriOpener));

            WindowTitle = windowTitle;
            Title = subscriptionOptions.Title;
            Description = subscriptionOptions.Description;
            IsDoNotShowAgainVisible = subscriptionOptions.IsDoNotShowAgainVisible;
            DoNotShowThisAgain = subscriptionOptions.DoNotShowThisAgain;
            ShowPrimaryButton = subscriptionOptions.ShowPrimary;
            PrimaryButtonContent = subscriptionOptions.PrimaryContent ?? string.Empty;
            ShowSecondaryButton = subscriptionOptions.ShowSecondary;
            SecondaryButtonContent = subscriptionOptions.SecondaryContent ?? string.Empty;
            CancelContent = subscriptionOptions.CancelContent ?? string.Empty;
            PrimaryCommand = new RelayCommand(_ =>
            {
                if (!string.IsNullOrWhiteSpace(subscriptionOptions.TrialUri))
                {
                    uriOpener.OpenUri(subscriptionOptions.TrialUri);
                }

                RequestClose?.Invoke();
            });

            SecondaryCommand = new RelayCommand(_ =>
            {
                if (!string.IsNullOrWhiteSpace(subscriptionOptions.BuyUri))
                {
                    uriOpener.OpenUri(subscriptionOptions.BuyUri);
                }

                RequestClose?.Invoke();
            });
        }

        public event Action RequestClose;

        public string WindowTitle { get; }

        public string Title { get; }

        public string Description { get; }

        public bool IsDoNotShowAgainVisible { get; }

        public bool DoNotShowThisAgain 
        {
            get => _doNotShowThisAgain; 
            set
            {
                _doNotShowThisAgain = value;
                OnPropertyChanged();
            }
        }

        public ICommand PrimaryCommand { get; }

        public bool ShowPrimaryButton { get; }

        public string PrimaryButtonContent { get; }

        public ICommand SecondaryCommand { get; }

        public bool ShowSecondaryButton { get; } 

        public string SecondaryButtonContent { get; }

        public string CancelContent { get; }
    }
}
