using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Interfaces;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace QATracker.BatchTasks.UI
{
    /// <summary>
    /// Interaction logic for VerifyFilesExtendedSettings.xaml
    /// </summary>
    public partial class VerifyFilesExtendedSettingsView : ISettingsAware<VerifyFilesExtendedSettings>, IUISettingsControl
    {
        private VerifyFilesExtendedSettings _settings;

        public static readonly DependencyProperty SelectedStatusesProperty =
            DependencyProperty.Register(nameof(SelectedStatuses), typeof(ObservableCollection<string>),
                typeof(VerifyFilesExtendedSettingsView), new PropertyMetadata(new ObservableCollection<string>()));

        public VerifyFilesExtendedSettingsView()
        {
            InitializeComponent();
        }

        public ObservableCollection<string> AllStatuses { get; set; } =
        [
            "Not Translated",
            "Draft",
            "Translated",
            "Translation Approved",
            "Translation Rejected",
            "Signed Off",
            "Sign-off Rejected",
            "Locked Segments"
        ];

        public ObservableCollection<string> SelectedStatuses { get; set; } = new();

        public VerifyFilesExtendedSettings Settings
        {
            get => _settings;
            set
            {
                _settings = value;
                foreach (var settingsReportStatus in _settings.ReportStatuses)
                {
                    SelectedStatuses.Add(settingsReportStatus);
                    CheckStatuses();
                }
            }
        }

        private void CheckStatuses()
        {
            Statuses.Dispatcher.InvokeAsync(() =>
            {
                for (int i = 0; i < Statuses.Items.Count; i++)
                {
                    var container = (ContentPresenter)Statuses.ItemContainerGenerator.ContainerFromIndex(i);
                    if (container != null)
                    {
                        var checkBox = UiHelper.FindVisualChild<CheckBox>(container);
                        if (SelectedStatuses.Contains(checkBox?.Content?.ToString()))
                            checkBox.IsChecked = true;
                    }
                }
            }, DispatcherPriority.Loaded);
        }

        public void Dispose()
        { }
        
        public VerifyFilesExtendedSettings GetSettings()
        {
            _settings.ReportStatuses = SelectedStatuses.ToList();
            return _settings;
        }

        public bool ValidateChildren() => true;

        private void CheckBox_Checked(object sender, System.Windows.RoutedEventArgs e)
        {
            var cb = sender as CheckBox;
            if (cb?.Content is string status && !SelectedStatuses.Contains(status))
                SelectedStatuses.Add(status);
        }

        private void CheckBox_Unchecked(object sender, System.Windows.RoutedEventArgs e)
        {
            var cb = sender as CheckBox;
            if (cb?.Content is string status && SelectedStatuses.Contains(status))
                SelectedStatuses.Remove(status);
        }
    }
}