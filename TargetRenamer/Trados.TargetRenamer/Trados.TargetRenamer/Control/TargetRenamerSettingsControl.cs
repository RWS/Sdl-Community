using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Interfaces;
using Trados.TargetRenamer.Interfaces;
using Trados.TargetRenamer.Services;
using Trados.TargetRenamer.ViewModel;

namespace Trados.TargetRenamer.Control
{
	public partial class TargetRenamerSettingsControl : UserControl, ISettingsAware<TargetRenamerSettings>, IUISettingsControl
	{
		private readonly IFolderDialogService _folderDialogService;
		public TargetRenamerSettingsControl()
		{
			InitializeComponent();
			_folderDialogService = new FolderDialogService();
			TargetRenamerSettingsViewModel = new TargetRenamerSettingsViewModel(_folderDialogService);
			var targetRenamerSettingsControl = new View.TargetRenamerSettingsView
			{
				DataContext = TargetRenamerSettingsViewModel
			};
			targetRenamerSettingsControl.InitializeComponent();
			targetRenamerHost.Child = targetRenamerSettingsControl;
		}

		public TargetRenamerSettingsViewModel TargetRenamerSettingsViewModel { get; set; }

		public TargetRenamerSettings Settings { get; set; }
	}
}
