using System;
using System.ComponentModel;
using System.Windows.Forms;
using Sdl.Desktop.IntegrationApi;
using SDLCommunityCleanUpTasks;
using SDLCommunityCleanUpTasks.Dialogs;
using FileDialog = SDLCommunityCleanUpTasks.Dialogs.FileDialog;

namespace Sdl.Community.CleanUpTasks
{
	public partial class CleanUpTargetSettingsControl : UserControl, ISettingsAware<CleanUpTargetSettings>, ICleanUpTargetSettingsControl
    {
        private readonly ICleanUpTargetSettingsPresenter presenter = null;

        public CleanUpTargetSettingsControl()
        {
            InitializeComponent();

            presenter = new CleanUpTargetSettingsPresenter(this, new FolderDialog());
        }

        public Button BackupButton { get { return backupButton; } }

        public TextBox BackupFolder { get { return backupTextBox; } }

        public CheckBox GenerateTarget { get { return generateTargetCheckBox; } }

        public CheckBox MakeBackups { get { return backupCheckBox; } }

        public CheckBox OverwriteSdlXliff { get { return preservePlaceHoldersCheckBox; } }

        public Button SaveButton { get { return saveFolderButton; } }

        public TextBox SaveFolder { get { return saveFoldertextBox; } }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public CleanUpTargetSettings Settings { get; set; }

        protected override void OnLeave(EventArgs e)
        {
            conversionsSettingsControl.SaveSettings();
        }

        protected override void OnLoad(EventArgs e)
        {
            // Used only in CleanUpSource but needed
            Settings.Settings = Settings;

            SetupBindings();
            presenter.Initialize();

            // ConversionSettingsControl
            conversionsSettingsControl.SetSettings(Settings, BatchTaskMode.Target);
            conversionsSettingsControl.SetPresenter(new ConversionSettingsPresenter(conversionsSettingsControl, new FileDialog()));
            conversionsSettingsControl.InitializeUI();
        }

        private void SetupBindings()
        {
            SettingsBinder.DataBindSetting<bool>(backupCheckBox, "Checked", Settings,
                                                 nameof(Settings.MakeBackups));
            SettingsBinder.DataBindSetting<bool>(preservePlaceHoldersCheckBox, "Checked", Settings,
                                                 nameof(Settings.OverwriteSdlXliff));
            SettingsBinder.DataBindSetting<bool>(generateTargetCheckBox, "Checked", Settings,
                                                 nameof(Settings.SaveTarget));
        }
    }
}