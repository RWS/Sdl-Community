using System;
using SDLCommunityCleanUpTasks.Dialogs;
using SDLCommunityCleanUpTasks.Utilities;

namespace SDLCommunityCleanUpTasks
{
	public class CleanUpTargetSettingsPresenter : ICleanUpTargetSettingsPresenter
    {
        private readonly ICleanUpTargetSettingsControl control = null;
        private readonly IFolderDialog dialog = null;

        public CleanUpTargetSettingsPresenter(ICleanUpTargetSettingsControl control, IFolderDialog dialog)
        {
            this.control = control;
            this.dialog = dialog;
        }

        public void Initialize()
        {
            AttachEventHandlers();

            SetTextBoxes();
        }

        private void SetTextBoxes()
        {
            control.SaveFolder.Text = control.Settings.SaveFolder;
            control.BackupFolder.Text = control.BackupFolder.Text;
        }

        private void AttachEventHandlers()
        {
            control.SaveButton.Click += SaveButton_Click;
            control.BackupButton.Click += BackupButton_Click;
        }

        private void BackupButton_Click(object sender, EventArgs e)
        {
            var saveFolder = control.Settings.BackupsSaveFolder;

            if (string.IsNullOrEmpty(saveFolder))
            {
                saveFolder = ProjectFileManager.GetProjectFolder();
            }

            var folder = dialog.GetFolder(saveFolder, "Choose Backup Folder");

            if (!string.IsNullOrEmpty(folder))
            {
                control.Settings.BackupsSaveFolder = folder;
                control.BackupFolder.Text = folder;
            }
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            var saveFolder = control.Settings.SaveFolder;

            if (string.IsNullOrEmpty(saveFolder))
            {
                saveFolder = ProjectFileManager.GetTargetLanguageFolder();
            }

            var folder = dialog.GetFolder(saveFolder, "Choose Save Folder");

            if (!string.IsNullOrEmpty(folder))
            {
                control.Settings.SaveFolder = folder;
                control.SaveFolder.Text = folder;
            }
        }
    }
}