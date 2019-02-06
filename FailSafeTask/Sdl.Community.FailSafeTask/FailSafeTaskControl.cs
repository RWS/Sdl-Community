namespace Sdl.Community.FailSafeTask
{
    using Sdl.Desktop.IntegrationApi;
    using System;
    using System.Windows.Forms;

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