using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using CaptureQARuleState.BatchTasks.Extension;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Interfaces;

namespace CaptureQARuleState.BatchTasks.UI
{
    /// <summary>
    /// Interaction logic for VerifyFilesExtendedSettings.xaml
    /// </summary>
    public partial class VerifyFilesExtendedSettingsView : ISettingsAware<VerifyFilesExtendedSettings>, IUISettingsControl
    {
        private readonly List<string> _allStatuses = [];
        private VerifyFilesExtendedSettings _settings;

        public VerifyFilesExtendedSettingsView()
        {
            InitializeComponent();
        }

        public List<string> AllStatuses
        {
            get => _allStatuses;
            set
            {
                foreach (var status in value)
                    _allStatuses.AddIfNotPresent(status);
            }
        }

        public bool IncludeIgnoredMessages { get; set; }
        public bool IncludeVerificationDetails { get; set; }
        public List<string> SelectedStatuses { get; set; } = new();

        public VerifyFilesExtendedSettings Settings
        {
            get => _settings;
            set
            {
                _settings = value;
                foreach (var settingsReportStatus in _settings.ReportStatuses)
                    SelectedStatuses.Add(settingsReportStatus);

                IncludeIgnoredMessages = _settings.IncludeIgnoredMessages;
                IncludeVerificationDetails = _settings.IncludeVerificationDetails;

                CheckStatuses();
            }
        }

        private bool AllStatusesChecked { get; set; }

        public void Dispose()
        { }

        public VerifyFilesExtendedSettings GetSettings()
        {
            _settings.ReportStatuses = SelectedStatuses.ToList();
            _settings.IncludeIgnoredMessages = IncludeIgnoredMessages;
            _settings.IncludeVerificationDetails = IncludeVerificationDetails;
            return _settings;
        }

        public bool ValidateChildren() => true;

        private void AllStatusesButton_OnClick(object sender, RoutedEventArgs e)
        {
            AllStatusesChecked = !AllStatusesChecked;

            if (AllStatusesChecked)
                foreach (var status in AllStatuses.Where(status => !SelectedStatuses.Contains(status)))
                    SelectedStatuses.Add(status);
            else
                SelectedStatuses.Clear();

            CheckStatuses();
        }

        private void CheckStatuses()
        {
            Statuses.Dispatcher.InvokeAsync(() =>
            {
                for (var i = 0; i < Statuses.Items.Count; i++)
                {
                    var container = (ContentPresenter)Statuses.ItemContainerGenerator.ContainerFromIndex(i);
                    if (container == null)
                        continue;

                    var checkBox = UiHelper.FindVisualChild<CheckBox>(container);
                    checkBox.IsChecked = SelectedStatuses.Contains(checkBox.Content?.ToString());
                }

                IncludeIgnoredMessagesButton.IsChecked = _settings.IncludeIgnoredMessages;
                IncludeVerificationDetailsButton.IsChecked = _settings.IncludeVerificationDetails;
            }, DispatcherPriority.Loaded);
        }

        private void StatusChecked(object sender, RoutedEventArgs e)
        {
            var cb = sender as CheckBox;
            if (cb?.Content is string status && !SelectedStatuses.Contains(status))
                SelectedStatuses.Add(status);
        }

        private void StatusUnchecked(object sender, RoutedEventArgs e)
        {
            var cb = sender as CheckBox;
            if (cb?.Content is not string status || !SelectedStatuses.Contains(status))
                return;

            SelectedStatuses.Remove(status);
        }
    }
}