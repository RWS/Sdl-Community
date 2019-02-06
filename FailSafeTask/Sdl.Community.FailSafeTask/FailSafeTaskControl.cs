using System;
using System.Windows.Forms;
using Sdl.Desktop.IntegrationApi;

namespace Sdl.Community.FailSafeTask
{
	public partial class FailSafeTaskControl : UserControl, ISettingsAware<FailSafeTaskSettings>
	{
		public FailSafeTaskControl()
		{
			InitializeComponent();
		}

		public FailSafeTaskSettings Settings { get; set; }

		protected override void OnLoad(EventArgs e)
		{
			SettingsBinder.DataBindSetting<bool>(copyToTargetRadioButton, "Checked", Settings, nameof(Settings.CopyToTarget));
			SettingsBinder.DataBindSetting<bool>(pseudoTranslateRadioButton, "Checked", Settings, nameof(Settings.PseudoTranslate));
		}
	}
}