using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Sdl.Community.CleanUpTasks.Dialogs;
using Sdl.Community.CleanUpTasks.Models;
using Sdl.Community.CleanUpTasks.Utilities;

namespace Sdl.Community.CleanUpTasks
{
	public class ConversionSettingsPresenter : IConversionsSettingsPresenter
    {
        private readonly IConversionsSettingsControl control = null;
        private readonly Dictionary<string, bool> convFiles = new Dictionary<string, bool>();
        private readonly IFileDialog dialog = null;
        private bool isDisabled = false;

        public ConversionSettingsPresenter(IConversionsSettingsControl control, IFileDialog dialog)
        {			  
            this.control = control;
            this.dialog = dialog;

            control.SetPresenter(this);
            UpdateButtons();
        }

        public void AddFile()
        {
            var files = dialog.GetFile(control.Settings.LastFileDirectory);

            foreach (var file in files)
            {
                if (File.Exists(file))
                {
                    XmlUtilities.Deserialize(file);
                    AddFileInternal(file);
                }
            }

            control.Settings.ConversionFiles = convFiles;
            UpdateLastFileDirectory();
        }

        public void DownClick()
        {
            if (control.FileList.Items.Count > 1)
            {
                if (control.FileList.SelectedIndex > -1 &&
                    control.FileList.SelectedIndex < control.FileList.Items.Count - 1)
                {
                    int selectedIndex = control.FileList.SelectedIndex;
                    var checkState = control.FileList.GetItemCheckState(selectedIndex);
                    var convFile = control.FileList.SelectedItem as ConversionFile;

                    control.FileList.Items.RemoveAt(selectedIndex);
                    control.FileList.Items.Insert(selectedIndex + 1, convFile);

                    control.FileList.SelectedIndex = selectedIndex + 1;
                    control.FileList.SetItemCheckState(control.FileList.SelectedIndex, checkState);
                }
            }
        }

        public void EditFile(IConversionFileView view)
        {
            var curItem = GetCurItem();
            if (curItem != null)
            {
                // Set the save path before opening the file!
                view.SavedFilePath = curItem.FullPath;
                DialogResult dr = ShowFileViewDialog(view);
                if (dr == DialogResult.OK)
                {
                    // TODO: Do something here?
                }
            }
        }

        public void GenerateFile(IConversionFileView view)
        {
            DialogResult dr = ShowFileViewDialog(view);
            if (dr == DialogResult.OK)
            {
                var savePath = view.SavedFilePath;
                AddFileInternal(savePath);
            }
        }

        public void Initialize()
        {
            var fileList = control.FileList.Items;

            foreach (var pair in control.Settings.ConversionFiles)
            {
                var convFile = new ConversionFile { FullPath = pair.Key, FileName = Path.GetFileName(pair.Key) };
                fileList.Add(convFile, pair.Value);
            }

            UpdateButtons();
        }

        public void RemoveFile()
        {
            // Multi-selection is not supported on CheckedListBox
            var curItem = GetCurItem();
            if (curItem != null)
            {
                control.FileList.Items.Remove(curItem);
                convFiles.Remove(curItem.FullPath);
                UpdateButtons();
            }

            control.Settings.ConversionFiles = convFiles;
        }

        public void SaveSettings()
        {
            var dict = new Dictionary<string, bool>();

            var count = control.FileList.Items.Count;
            var fileList = control.FileList;

            for (int i = 0; i < count; ++i)
            {
                var checkState = fileList.GetItemCheckState(i);
                var item = fileList.Items[i] as ConversionFile;

                if (checkState == CheckState.Checked)
                {
                    dict.Add(item.FullPath, true);
                }
                else
                {
                    dict.Add(item.FullPath, false);
                }
            }

            control.Settings.ConversionFiles = dict;
        }

        public void UpClick()
        {
            if (control.FileList.Items.Count > 1)
            {
                if (control.FileList.SelectedIndex > 0)
                {
                    int selectedIndex = control.FileList.SelectedIndex;
                    var checkState = control.FileList.GetItemCheckState(selectedIndex);
                    var convFile = control.FileList.SelectedItem as ConversionFile;

                    control.FileList.Items.RemoveAt(selectedIndex);
                    control.FileList.Items.Insert(selectedIndex - 1, convFile);

                    control.FileList.SelectedIndex = selectedIndex - 1;

                    control.FileList.SetItemCheckState(control.FileList.SelectedIndex, checkState);
                }
            }
        }

        private static DialogResult ShowFileViewDialog(IConversionFileView view)
        {
            view.InitializeUI();
            return view.ShowDialog();
        }

        private void AddFileInternal(string file)
        {
            var convFile = new ConversionFile { FullPath = file, FileName = Path.GetFileName(file) };

            var fileList = control.FileList.Items;

            if (control.FileList.FindStringExact(convFile.FileName) == ListBox.NoMatches)
            {
                fileList.Add(convFile, true);
                convFiles.Add(file, false);
                UpdateButtons();
            }
        }

        private ConversionFile GetCurItem()
        {
            return control.FileList.SelectedItem as ConversionFile;
        }

        private void UpdateButtons()
        {
            if (control.FileList.Items.Count == 0)
            {
                control.Edit.Enabled = false;
                control.Remove.Enabled = false;
                control.Up.Enabled = false;
                control.Down.Enabled = false;
                isDisabled = true;
            }
            else if (isDisabled)
            {
                control.Edit.Enabled = true;
                control.Remove.Enabled = true;
                control.Up.Enabled = true;
                control.Down.Enabled = true;
                isDisabled = false;
            }
        }

        private void UpdateLastFileDirectory()
        {
            if (convFiles.Count > 0)
            {
                var newDirectory = Path.GetDirectoryName(convFiles.Last().Key);

                if (!string.IsNullOrEmpty(newDirectory))
                {
                    // http://stackoverflow.com/a/1756776/906773
                    if (string.Compare(
                        Path.GetFullPath(newDirectory).TrimEnd(Path.DirectorySeparatorChar),
                        Path.GetFullPath(control.Settings.LastFileDirectory).TrimEnd(Path.DirectorySeparatorChar),
                        StringComparison.OrdinalIgnoreCase) != 0)
                    {
                        control.Settings.LastFileDirectory = newDirectory;
                    }
                }
            }
        }
    }
}