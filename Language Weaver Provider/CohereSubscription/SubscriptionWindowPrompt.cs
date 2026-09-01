using LanguageWeaverProvider.Model.Interface;
using LanguageWeaverProvider.SubscriptionJourney.View;
using LanguageWeaverProvider.SubscriptionJourney.ViewModel;

namespace LanguageWeaverProvider.CohereSubscription
{
    public class SubscriptionWindowPrompt : ISubscriptionPrompt
    {
        public void Show(SubscriptionViewModel viewModel)
        {
            var view = new CohereSubscriptionWindow
            {
                DataContext = viewModel
            };

            _ = view.ShowDialog();
        }
    }
}
