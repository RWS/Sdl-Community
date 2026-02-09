using LanguageWeaverProvider.SubscriptionJourney.ViewModel;
using System.Windows;

namespace LanguageWeaverProvider.SubscriptionJourney.View
{
    /// <summary>
    /// Interaction logic for CohereSubscriptionWindow.xaml
    /// </summary>
    public partial class CohereSubscriptionWindow : Window
    {
        public CohereSubscriptionWindow()
        {
            InitializeComponent();
            Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            if (DataContext is SubscriptionViewModel vm)
            {
                vm.RequestClose += () =>
                {
                    DialogResult = true;
                    Close();
                };
            }
        }
    }
}
