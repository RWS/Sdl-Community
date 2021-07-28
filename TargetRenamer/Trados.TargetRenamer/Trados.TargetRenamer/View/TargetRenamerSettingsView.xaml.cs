using System.Windows.Controls;
using Sdl.Desktop.IntegrationApi;
using Trados.TargetRenamer.BatchTask;
using Trados.TargetRenamer.ViewModel;

namespace Trados.TargetRenamer.View
{
	/// <summary>
	/// Interaction logic for TargetRenamerSettingsView.xaml
	/// </summary>
	public partial class TargetRenamerSettingsView : ISettingsAware<TargetRenamerSettings>
    {
        public TargetRenamerSettingsView()
        {
            InitializeComponent();
            AddErrorHandlers();
        }

        public TargetRenamerSettings Settings { get; set; }
        public TargetRenamerSettingsViewModel TargetRenamerSettingsViewModel => (TargetRenamerSettingsViewModel)DataContext;
        private bool CustomLocationHasErrors { get; set; }
        private bool DelimiterHasErrors { get; set; }

        public void AddErrorHandlers()
        {
            Validation.AddErrorHandler(CustomLocation, ErrorHandler);
            Validation.AddErrorHandler(Delimiter, ErrorHandler);
        }

        public void Dispose()
        {
        }

        private void ErrorHandler(object sender, ValidationErrorEventArgs e)
        {
            var errorSource = e.OriginalSource as TextBox;
            if (errorSource.Name == nameof(CustomLocation))
            {
                CustomLocationHasErrors = e.Action == ValidationErrorEventAction.Added;
            }
            if (errorSource.Name == nameof(Delimiter))
            {
                DelimiterHasErrors = e.Action == ValidationErrorEventAction.Added;
            }

            SetHasErrorOnViewModel();
        }

        private void SetHasErrorOnViewModel()
        {
            TargetRenamerSettingsViewModel.HasErrors = CustomLocationHasErrors || DelimiterHasErrors;
        }
    }
}