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
using Trados.TargetRenamer.ViewModel;

namespace Trados.TargetRenamer.Control
{
	public partial class TargetRenamerSettingsControl : UserControl, ISettingsAware<TargetRenamerSettings>, IUISettingsControl
	{
		public TargetRenamerSettingsControl()
		{
			InitializeComponent();
			TargetRenamerSettingsViewModel = new TargetRenamerSettingsViewModel();
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
