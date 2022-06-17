using System.Windows.Controls;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Interfaces;
using Trados.TargetRenamer.BatchTask;
using Trados.TargetRenamer.Interfaces;
using Trados.TargetRenamer.Services;
using Trados.TargetRenamer.ViewModel;

namespace Trados.TargetRenamer.View
{
	/// <summary>
	/// Interaction logic for TargetRenamerSettingsView.xaml
	/// </summary>
	public partial class TargetRenamerSettingsView : IUISettingsControl, ISettingsAware<TargetRenamerSettings>
    {
        public TargetRenamerSettingsView()
        {
            InitializeComponent();
            IFolderDialogService folderDialogService = new FolderDialogService();
            TargetRenamerSettingsViewModel = new TargetRenamerSettingsViewModel(folderDialogService);
            DataContext = TargetRenamerSettingsViewModel;
        }

        public TargetRenamerSettings Settings { get; set; }

        public TargetRenamerSettingsViewModel TargetRenamerSettingsViewModel { get; set; }

        public void Dispose()
        {
        }

        public bool ValidateChildren()
        {
	        return Validation.GetErrors(CustomLocation).Count == 0
	               && Validation.GetErrors(Delimiter).Count == 0
	               && Validation.GetErrors(RegexReplaceWith).Count == 0
	               && Validation.GetErrors(CustomString).Count == 0;
        }
    }
}