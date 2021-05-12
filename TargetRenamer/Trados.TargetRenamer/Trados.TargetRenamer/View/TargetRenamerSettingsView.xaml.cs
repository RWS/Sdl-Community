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

		private void Selector_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
		public void Dispose()
		{
			var comboBox = (ComboBox) sender;
			var vm = (TargetRenamerSettingsViewModel) comboBox.DataContext;

			if (vm == null) return;

			switch (e.AddedItems[0])
			{
				case ReplacementEnum.Suffix:
					vm.AppendAsSuffix = true;
					vm.AppendAsPrefix = false;
					vm.UseRegularExpression = false;
					TargetComboBox.SelectedItem = TargetLanguageEnum.TargetLanguage;
					break;
				case ReplacementEnum.Prefix:
					vm.AppendAsPrefix = true;
					vm.AppendAsSuffix = false;
					vm.UseRegularExpression = false;
					TargetComboBox.SelectedItem = TargetLanguageEnum.TargetLanguage;
					break;
				case ReplacementEnum.RegularExpression:
					vm.UseRegularExpression = true;
					vm.AppendAsSuffix = false;
					vm.AppendAsPrefix = false;
					vm.AppendTargetLanguage = false;
					vm.AppendCustomString = false;
					TargetComboBox.SelectedItem = null;
					break;
				default:
					throw new ArgumentOutOfRangeException($"{nameof(e)}. Selection does not exist as an option.");
			}
		}

		private void Combobox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
		public bool ValidateChildren()
		{
			var comboBox = (ComboBox) sender;
			var vm = (TargetRenamerSettingsViewModel) comboBox.DataContext;

			if (vm == null || e.AddedItems.Count < 1) return;

			switch (e.AddedItems[0])
			{
				case TargetLanguageEnum.TargetLanguage:
					vm.AppendTargetLanguage = true;
					vm.AppendCustomString = false;
					break;
				case TargetLanguageEnum.CustomString:
					vm.AppendCustomString = true;
					vm.AppendTargetLanguage = false;
					break;
				default:
					throw new ArgumentOutOfRangeException($"{nameof(e)}. Selection does not exist as an option.");
			}

			return true;
		}

		public TargetRenamerSettings Settings { get; set; }
		public TargetRenamerSettingsViewModel TargetRenamerSettingsViewModel { get; set; }
	}
}
