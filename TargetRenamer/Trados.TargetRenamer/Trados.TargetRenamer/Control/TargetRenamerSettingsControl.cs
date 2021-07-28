﻿using System.Windows.Forms;
using Sdl.Desktop.IntegrationApi;
using Trados.TargetRenamer.BatchTask;
using Trados.TargetRenamer.Interfaces;
using Trados.TargetRenamer.Services;
using Trados.TargetRenamer.ViewModel;

namespace Trados.TargetRenamer.Control
{
	public partial class TargetRenamerSettingsControl : UserControl, ISettingsAware<TargetRenamerSettings>
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

		public TargetRenamerSettings Settings { get; set; }
		public TargetRenamerSettingsViewModel TargetRenamerSettingsViewModel { get; set; }
	}
}