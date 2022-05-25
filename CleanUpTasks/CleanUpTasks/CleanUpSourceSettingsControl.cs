﻿using System;
using System.ComponentModel;
using System.Windows.Forms;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Interfaces;
using SDLCommunityCleanUpTasks;
using FileDialog = SDLCommunityCleanUpTasks.Dialogs.FileDialog;

namespace Sdl.Community.CleanUpTasks
{
	public partial class CleanUpSourceSettingsControl : UserControl, ISettingsAware<CleanUpSourceSettings>,IUISettingsControl
    {
	    public CleanUpSourceSettingsControl()
        {
            InitializeComponent();
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public CleanUpSourceSettings Settings { get; set; }

        protected override void OnLeave(EventArgs e)
        {
            SaveSettings();
        }

        public void SaveSettings()
        {
            segmentLockerControl.SaveSettings();
            tagsSettingsControl.SaveSettings();
            conversionsSettingsControl.SaveSettings();
        }

        protected override void OnLoad(EventArgs e)
        {
			base.OnLoad(e);
            // Set Settings Here!!
            Settings.Settings = Settings;

            // Make sure to set the settings first!
            // SegmentLockerControl
            segmentLockerControl.SetSettings(Settings);
            segmentLockerControl.SetPresenter(new SegmentLockerPresenter(segmentLockerControl));
            segmentLockerControl.InitializeUI();

            // ConversionSettingsControl
            conversionsSettingsControl.SetSettings(Settings, BatchTaskMode.Source);
            conversionsSettingsControl.SetPresenter(new ConversionSettingsPresenter(conversionsSettingsControl, new FileDialog()));
            conversionsSettingsControl.InitializeUI();

            // TagSettingsControl
            tagsSettingsControl.SetSettings(Settings);
            tagsSettingsControl.SetPresenter(new TagSettingsPresenter(tagsSettingsControl));
            tagsSettingsControl.InitializeUI();
        }
    }
}