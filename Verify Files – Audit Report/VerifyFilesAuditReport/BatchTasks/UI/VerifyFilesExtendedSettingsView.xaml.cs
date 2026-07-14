using System.Collections.Generic;
using System.Windows;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Interfaces;

namespace VerifyFilesAuditReport.BatchTasks.UI
{
    /// <summary>
    /// Interaction logic for VerifyFilesExtendedSettings.xaml
    /// </summary>
    public partial class VerifyFilesExtendedSettingsView : ISettingsAware<VerifyFilesExtendedSettings>, IUISettingsControl
    {
        private VerifyFilesExtendedSettings _settings;

        public VerifyFilesExtendedSettingsView()
        {
            InitializeComponent();
            DataContext = ViewModel;
        }

        public SettingsViewModel ViewModel { get; } = new();

        public VerifyFilesExtendedSettings Settings
        {
            get => _settings;
            set
            {
                _settings = value;
                ViewModel.LoadFrom(value);
            }
        }

        public void Dispose()
        { }

        public VerifyFilesExtendedSettings GetSettings()
        {
            ViewModel.SaveTo(_settings);
            return _settings;
        }

        public void SetAvailableStatuses(IEnumerable<string> statusNames) =>
            ViewModel.SetAvailableStatuses(statusNames);

        public bool ValidateChildren() => true;

        private void AllStatusesButton_OnClick(object sender, RoutedEventArgs e) =>
            ViewModel.ToggleAllStatuses();
    }
}
