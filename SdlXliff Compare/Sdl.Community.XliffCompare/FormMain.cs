using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Sdl.Community.XliffCompare.Core;
using Sdl.Community.XliffCompare.Core.Comparer;

namespace Sdl.Community.XliffCompare
{
    public partial class FormMain : UserControl
    {
        public FormMain()
        {
            InitializeComponent();
        }


        private void FormMain_Load(object sender, EventArgs e)
        {
            Text = Application.ProductName + " (" + Application.ProductVersion + ")";
            tabControl_comparison_type.TabPages[0].Select();
            textBox_OriginalFile.Select();



            toolStripStatusLabel_Progress_Percentage.Text = "0%";
            toolStripStatusLabel_Message.Text = "";
            toolStripStatusLabel_Status.Text = "Ready";
            Application.DoEvents();
       


            LoadVisualSettings();

            CheckEnableProcess();
        }


   
  

        private void StartProcessing()
        {
            try
            {
                SaveVisualSettings();


              
                Enabled = false;
                Cursor = Cursors.WaitCursor;

                toolStripProgressBar1.Maximum = 10;
                toolStripProgressBar1.Value = 0;

                toolStripStatusLabel_Progress_Percentage.Text = "0%";
                toolStripStatusLabel_Message.Text = "...";
                toolStripStatusLabel_Status.Text = "Ready";
                Application.DoEvents();
           
                
                var comparer = new Processor();
                try
                {
                    comparer.Progress += comparer_Progress;

                    comparer.ProcessFiles(tabControl_comparison_type.SelectedIndex != 0);
                }
                finally
                {
                    comparer.Progress -= comparer_Progress;
                }
            }
            catch (Exception ex)
            {
               
                Enabled = true;
                Cursor = Cursors.Default;

                MessageBox.Show(this, ex.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                Enabled = true;
                Cursor = Cursors.Default;

           
                toolStripProgressBar1.Value = toolStripProgressBar1.Maximum;

                toolStripStatusLabel_Progress_Percentage.Text = "100%";
                toolStripStatusLabel_Message.Text = "Finished processing";
                toolStripStatusLabel_Status.Text = "Ready";
                Application.DoEvents();
            
            }

        }


   
        #region  |  progress callbacks  |


        private void comparer_Progress(int maximum, int current, int percent, string message)
        {
            if (maximum < current) 
                return;
           
            toolStripProgressBar1.Maximum = maximum;
            toolStripProgressBar1.Value = current;

            toolStripStatusLabel_Progress_Percentage.Text = percent.ToString() + "%" ;
            toolStripStatusLabel_Message.Text = message;
            toolStripStatusLabel_Status.Text = "Processing...";

            Application.DoEvents();

        }

        #endregion



        #region  |  settings  |

        private void LoadVisualSettings()
        {

            comboBox_reportFormat.Items.Add(new ComboBoxExItem("html", 0));
            comboBox_reportFormat.Items.Add(new ComboBoxExItem("xml", 1));

            textBox_OriginalFile.Text = Processor.Settings.FilePathOriginal;
            textBox_UpdatedFile.Text = Processor.Settings.FilePathUpdated;

            textBox_OriginalDirectory.Text = Processor.Settings.DirectoryPathOriginal;
            textBox_UpdatedDirectory.Text = Processor.Settings.DirectoryPathUpdated;
            checkBox_processSubFolders.Checked = Processor.Settings.SearchSubFolders;


            textBox_reportDirectory.Text = Processor.Settings.ReportDirectory;
            textBox_reportFileName.Text = Processor.Settings.ReportFileName;
            checkBox_viewReportWhenProcessingFinished.Checked = Processor.Settings.ViewReportWhenFinishedProcessing;


            comboBox_reportFormat.SelectedIndex = Processor.Settings.ReportingFormat == Settings.ReportFormat.Html ? 0 : 1;
        }
        private void SaveVisualSettings()
        {
            Processor.Settings.FilePathOriginal = textBox_OriginalFile.Text;
            Processor.Settings.FilePathUpdated = textBox_UpdatedFile.Text;

            Processor.Settings.DirectoryPathOriginal = textBox_OriginalDirectory.Text;
            Processor.Settings.DirectoryPathUpdated = textBox_UpdatedDirectory.Text;
            Processor.Settings.SearchSubFolders = checkBox_processSubFolders.Checked;


            Processor.Settings.ReportDirectory = textBox_reportDirectory.Text;
            Processor.Settings.ReportFileName = textBox_reportFileName.Text;
            Processor.Settings.ViewReportWhenFinishedProcessing = checkBox_viewReportWhenProcessingFinished.Checked;


            Processor.Settings.ReportingFormat = comboBox_reportFormat.SelectedIndex == 0 ? Settings.ReportFormat.Html : Settings.ReportFormat.Xml;


            Processor.SaveSettings();
        }

        private void LoadSettingsDialog()
        {
            var f = new FormSettings
            {
                comboBox_comparisonType =
                {
                    SelectedIndex = Processor.Settings.CompareType == Settings.ComparisonType.Words ? 0 : 1
                },
                checkBox_includeTagsInComparison = {Checked = Processor.Settings.ComparisonIncludeTags},
                checkBox_includeIndividualFileInformationMergedFiles =
                {
                    Checked = Processor.Settings.IncludeIndividualFileInformation
                },
                checkBox_reportFilterChangedTargetContent =
                {
                    Checked = Processor.Settings.ReportFilterChangedTargetContent
                },
                checkBox_reportFilterSegmentStatusChanged =
                {
                    Checked = Processor.Settings.ReportFilterSegmentStatusChanged
                },
                checkBox_reportFilterSegmentsContainingComments =
                {
                    Checked = Processor.Settings.ReportFilterSegmentsContainingComments
                },
                checkBox_reportFilterFilesWithNoRecordsFiltered =
                {
                    Checked = Processor.Settings.ReportFilterFilesWithNoRecordsFiltered
                },
                checkBox_useCustomStyleSheet = {Checked = Processor.Settings.UseCustomStyleSheet},
                textBox_Custom_xsltFilePath = {Text = Processor.Settings.FilePathCustomStyleSheet},
                StyleNewText = Processor.Settings.StyleNewText,
                StyleRemovedText = Processor.Settings.StyleRemovedText,
                StyleNewTag = Processor.Settings.StyleNewTag,
                StyleRemovedTag = Processor.Settings.StyleRemovedTag
            };





            f.ShowDialog();

            if (!f.SaveOptions) 
                return;

            Processor.Settings.CompareType = f.comboBox_comparisonType.SelectedIndex == 0 ? Settings.ComparisonType.Words : Settings.ComparisonType.Characters;
            Processor.Settings.ComparisonIncludeTags = f.checkBox_includeTagsInComparison.Checked;
            Processor.Settings.IncludeIndividualFileInformation = f.checkBox_includeIndividualFileInformationMergedFiles.Checked;

            Processor.Settings.ReportFilterChangedTargetContent = f.checkBox_reportFilterChangedTargetContent.Checked;
            Processor.Settings.ReportFilterSegmentStatusChanged = f.checkBox_reportFilterSegmentStatusChanged.Checked;
            Processor.Settings.ReportFilterSegmentsContainingComments = f.checkBox_reportFilterSegmentsContainingComments.Checked;
            Processor.Settings.ReportFilterFilesWithNoRecordsFiltered = f.checkBox_reportFilterFilesWithNoRecordsFiltered.Checked;

            if (f.textBox_Custom_xsltFilePath.Text.Trim()!=string.Empty && !File.Exists(f.textBox_Custom_xsltFilePath.Text))
                f.checkBox_useCustomStyleSheet.Checked = false;
                

            Processor.Settings.UseCustomStyleSheet = f.checkBox_useCustomStyleSheet.Checked;
            Processor.Settings.FilePathCustomStyleSheet = f.textBox_Custom_xsltFilePath.Text;

            Processor.Settings.StyleNewText = f.StyleNewText;
            Processor.Settings.StyleRemovedText = f.StyleRemovedText;
            Processor.Settings.StyleNewTag = f.StyleNewTag;
            Processor.Settings.StyleRemovedTag = f.StyleRemovedTag;
        }


        #endregion
        


        #region  |  private form methods  |


     



        private void LoadAboutDialog()
        {
            var f = new About();
            f.ShowDialog();
        }

        private void CheckEnableProcess()
        {
            var isReady = false;

            if (textBox_reportFileName.Text.Trim() != string.Empty && Directory.Exists(textBox_reportDirectory.Text))
            {
                switch (tabControl_comparison_type.SelectedIndex)
                {
                    case 0:
                        {
                            if (textBox_OriginalFile.Text.Trim() != string.Empty && File.Exists(textBox_OriginalFile.Text.Trim())
                                && textBox_UpdatedFile.Text.Trim() != string.Empty && File.Exists(textBox_UpdatedFile.Text.Trim()))
                                isReady = true;
                        } break;
                    case 1:
                        {
                            if (textBox_OriginalDirectory.Text.Trim() != string.Empty && Directory.Exists(textBox_OriginalDirectory.Text.Trim())
                                && textBox_UpdatedDirectory.Text.Trim() != string.Empty && Directory.Exists(textBox_UpdatedDirectory.Text.Trim()))
                                isReady = true;

                        } break;
                }
            }

            toolStripButton_Run.Enabled = isReady;
            runToolStripMenuItem.Enabled = isReady;
        }
        private void AutoUpdateReportFileLocation()
        {
            switch (tabControl_comparison_type.SelectedIndex)
            {
                case 0: textBox_reportDirectory.Text = textBox_UpdatedFile.Text.Trim() != string.Empty ? Path.GetDirectoryName(textBox_UpdatedFile.Text) : string.Empty; break;
                case 1: textBox_reportDirectory.Text = textBox_UpdatedDirectory.Text; break;
            }

        }

        private void button_Browse_OriginalFile_Click(object sender, EventArgs e)
        {
            var sPath = textBox_OriginalFile.Text;

            if (!Directory.Exists(sPath))
            {
                while (sPath.Contains("\\"))
                {
                    sPath = sPath.Substring(0, sPath.LastIndexOf("\\", StringComparison.Ordinal));
                    if (Directory.Exists(sPath))
                    {
                        break;
                    }
                }
            }



            var openFileDialog = new OpenFileDialog
            {
                InitialDirectory = sPath,
                Filter = "SDL XLIFF file (*.sdlxliff)|*.sdlxliff;",
                Title = "Select original file",
                FilterIndex = 0,
                Multiselect = false,
                RestoreDirectory = true
            };


            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                if (openFileDialog.FileName != null && openFileDialog.FileName.Trim() != "")
                {
                    textBox_OriginalFile.Text = openFileDialog.FileName;
                }
            }
            CheckEnableProcess();

        }

        private void button_Browse_UpdatedFile_Click(object sender, EventArgs e)
        {
            var sPath = textBox_UpdatedFile.Text;

            if (!Directory.Exists(sPath))
            {
                while (sPath.Contains("\\"))
                {
                    sPath = sPath.Substring(0, sPath.LastIndexOf("\\", StringComparison.Ordinal));
                    if (Directory.Exists(sPath))
                    {
                        break;
                    }
                }
            }



            var openFileDialog = new OpenFileDialog
            {
                InitialDirectory = sPath,
                Filter = "SDL XLIFF file (*.sdlxliff)|*.sdlxliff;",
                Title = "Select updated file",
                FilterIndex = 0,
                Multiselect = false,
                RestoreDirectory = true
            };


            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                if (openFileDialog.FileName != null && openFileDialog.FileName.Trim() != "")
                {
                    textBox_UpdatedFile.Text = openFileDialog.FileName;
                }
            }
            CheckEnableProcess();
        }

        private void button_Browse_OriginalDirectory_Click(object sender, EventArgs e)
        {
            var sPath = textBox_OriginalDirectory.Text;

            if (!Directory.Exists(sPath))
            {
                while (sPath.Contains("\\"))
                {
                    sPath = sPath.Substring(0, sPath.LastIndexOf("\\", StringComparison.Ordinal));
                    if (Directory.Exists(sPath))
                    {
                        break;
                    }
                }
            }

            var folderBrowserDialog = new FolderBrowserDialog
            {
                SelectedPath = sPath,
                ShowNewFolderButton = true,
                Description = "Select the original directory"
            };
            if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                textBox_OriginalDirectory.Text = folderBrowserDialog.SelectedPath;
            }
            CheckEnableProcess();
        }

        private void button_Browse_UpdatedDirectory_Click(object sender, EventArgs e)
        {
            var sPath = textBox_UpdatedDirectory.Text;

            if (!Directory.Exists(sPath))
            {
                while (sPath.Contains("\\"))
                {
                    sPath = sPath.Substring(0, sPath.LastIndexOf("\\", StringComparison.Ordinal));
                    if (Directory.Exists(sPath))
                    {
                        break;
                    }
                }
            }

            var folderBrowserDialog = new FolderBrowserDialog
            {
                SelectedPath = sPath,
                ShowNewFolderButton = true,
                Description = "Select the updated directory"
            };
            if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                textBox_UpdatedDirectory.Text = folderBrowserDialog.SelectedPath;
            }
            CheckEnableProcess();
        }

        private void button_browse_reportDirectory_Click(object sender, EventArgs e)
        {
            var sPath = textBox_reportDirectory.Text;

            if (!Directory.Exists(sPath))
            {
                while (sPath.Contains("\\"))
                {
                    sPath = sPath.Substring(0, sPath.LastIndexOf("\\", StringComparison.Ordinal));
                    if (Directory.Exists(sPath))
                    {
                        break;
                    }
                }
            }

            var folderBrowserDialog = new FolderBrowserDialog
            {
                SelectedPath = sPath,
                ShowNewFolderButton = true,
                Description = "Select the report directory"
            };
            if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                textBox_reportDirectory.Text = folderBrowserDialog.SelectedPath;
            }
            CheckEnableProcess();
        }

        private void textBox_OriginalFile_TextChanged(object sender, EventArgs e)
        {
            CheckEnableProcess();
        }

        private void textBox_UpdatedFile_TextChanged(object sender, EventArgs e)
        {
            AutoUpdateReportFileLocation();
            CheckEnableProcess();
        }

        private void textBox_OriginalDirectory_TextChanged(object sender, EventArgs e)
        {
            CheckEnableProcess();
        }

        private void textBox_UpdatedDirectory_TextChanged(object sender, EventArgs e)
        {
            AutoUpdateReportFileLocation();
            CheckEnableProcess();
        }

        private void textBox_reportDirectory_TextChanged(object sender, EventArgs e)
        {
            CheckEnableProcess();
        }

        private void tabControl_comparison_type_SelectedIndexChanged(object sender, EventArgs e)
        {
            CheckEnableProcess();
        }

        private void toolStripButton_Run_Click(object sender, EventArgs e)
        {
            StartProcessing();
        }

        private void runToolStripMenuItem_Click(object sender, EventArgs e)
        {
            StartProcessing();
        }

        private void textBox_reportFileName_TextChanged(object sender, EventArgs e)
        {
            CheckEnableProcess();
        }

        private void optionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LoadSettingsDialog();
        }

        private void toolStripButton_Options_Click(object sender, EventArgs e)
        {
            LoadSettingsDialog();
        }

        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            SaveVisualSettings();
        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
           
        }

       

        private void helpToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            var f = new About();
            f.ShowDialog();
        }

        private void toolStripButton_Help_Click(object sender, EventArgs e)
        {
            Help.ShowHelp(this, helpProvider1.HelpNamespace);
        
        }

        private void help1ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Help.ShowHelp(this, helpProvider1.HelpNamespace);
        }



        private void textBox_OriginalFile_DragOver(object sender, DragEventArgs e)
        {
            e.Effect = e.Data.GetDataPresent(DataFormats.FileDrop) ? DragDropEffects.Copy : DragDropEffects.None;
        }

        private void textBox_OriginalFile_DragDrop(object sender, DragEventArgs e)
        {

            var fileList = (string[])e.Data.GetData(DataFormats.FileDrop, false);

            var sdlXliffFiles = new List<FileInfo>();
            foreach (var file in fileList)
            {
                if (file.ToLower().EndsWith(".sdlxliff"))
                {
                    sdlXliffFiles.Add(new FileInfo(file));
                }
            }

            if (!sdlXliffFiles.Any()) return;
            if (sdlXliffFiles.Count == 2)
            {

                if (sdlXliffFiles[0].LastWriteTime > sdlXliffFiles[1].LastWriteTime)
                {
                    textBox_OriginalFile.Text = sdlXliffFiles[1].FullName;
                    textBox_UpdatedFile.Text = sdlXliffFiles[0].FullName;
                }
                else
                {
                    textBox_OriginalFile.Text = sdlXliffFiles[0].FullName;
                    textBox_UpdatedFile.Text = sdlXliffFiles[1].FullName;
                }
            }
            else
            {
                textBox_OriginalFile.Text = sdlXliffFiles[0].FullName;
            }
        }

        private void textBox_UpdatedFile_DragOver(object sender, DragEventArgs e)
        {
            e.Effect = e.Data.GetDataPresent(DataFormats.FileDrop) ? DragDropEffects.Copy : DragDropEffects.None;
        }

        private void textBox_UpdatedFile_DragDrop(object sender, DragEventArgs e)
        {
            // Extract the data from the DataObject-Container into a string list
            var fileList = (string[])e.Data.GetData(DataFormats.FileDrop, false);

            var sdlXliffFiles = (from file in fileList where file.ToLower().EndsWith(".sdlxliff") select new FileInfo(file)).ToList();

            if (!sdlXliffFiles.Any()) return;
            if (sdlXliffFiles.Count == 2)
            {

                if (sdlXliffFiles[0].LastWriteTime > sdlXliffFiles[1].LastWriteTime)
                {
                    textBox_OriginalFile.Text = sdlXliffFiles[1].FullName;
                    textBox_UpdatedFile.Text = sdlXliffFiles[0].FullName;
                }
                else
                {
                    textBox_OriginalFile.Text = sdlXliffFiles[0].FullName;
                    textBox_UpdatedFile.Text = sdlXliffFiles[1].FullName;
                }
            }
            else
            {
                textBox_UpdatedFile.Text = sdlXliffFiles[0].FullName;
            }
        }

        private void textBox_OriginalDirectory_DragOver(object sender, DragEventArgs e)
        {
            e.Effect = e.Data.GetDataPresent(DataFormats.FileDrop) ? DragDropEffects.Copy : DragDropEffects.None;
        }

        private void textBox_OriginalDirectory_DragDrop(object sender, DragEventArgs e)
        {
            // Extract the data from the DataObject-Container into a string list
            var directoryList = (string[])e.Data.GetData(DataFormats.FileDrop, false);


            if (directoryList.Length == 2
                && Directory.Exists(directoryList[0]) && Directory.Exists(directoryList[1]))
            {
                var pairedFiles = new PairedFiles(directoryList[0], directoryList[1], new string[] { "*.sdlxliff" },
                    Processor.Settings.SearchSubFolders).PairedProcessingFiles;

                var i0 = 0;
                var i1 = 0;
                foreach (var pairedFile in pairedFiles)
                {
                    if (pairedFile.OriginalFilePath == null || pairedFile.UpdatedFilePath == null) 
                        continue;

                    if (pairedFile.OriginalFilePath.LastWriteTime > pairedFile.UpdatedFilePath.LastWriteTime)
                        i0++;
                    else if (pairedFile.OriginalFilePath.LastWriteTime < pairedFile.UpdatedFilePath.LastWriteTime)
                        i1++;
                }
                if (i0 > i1)
                {
                    textBox_OriginalDirectory.Text = directoryList[1];
                    textBox_UpdatedDirectory.Text = directoryList[0];
                }
                else
                {
                    textBox_OriginalDirectory.Text = directoryList[0];
                    textBox_UpdatedDirectory.Text = directoryList[1];
                }
            }
            else if (Directory.Exists(directoryList[0]))
            {
                textBox_OriginalDirectory.Text = directoryList[0];
            }


        }

        private void textBox_UpdatedDirectory_DragOver(object sender, DragEventArgs e)
        {
            e.Effect = e.Data.GetDataPresent(DataFormats.FileDrop) ? DragDropEffects.Copy : DragDropEffects.None;
        }

        private void textBox_UpdatedDirectory_DragDrop(object sender, DragEventArgs e)
        {
            // Extract the data from the DataObject-Container into a string list
            var directoryList = (string[])e.Data.GetData(DataFormats.FileDrop, false);


            if (directoryList.Length == 2
                && Directory.Exists(directoryList[0]) && Directory.Exists(directoryList[1]))
            {

                var pairedFiles = new PairedFiles(directoryList[0], directoryList[1], new string[] { "*.sdlxliff" },
                    Processor.Settings.SearchSubFolders).PairedProcessingFiles;

                var i0 = 0;
                var i1 = 0;
                foreach (var pairedFile in pairedFiles)
                {
                    if (pairedFile.OriginalFilePath == null || pairedFile.UpdatedFilePath == null) continue;
                    if (pairedFile.OriginalFilePath.LastWriteTime > pairedFile.UpdatedFilePath.LastWriteTime)
                        i0++;
                    else if (pairedFile.OriginalFilePath.LastWriteTime < pairedFile.UpdatedFilePath.LastWriteTime)
                        i1++;
                }
                if (i0 > i1)
                {
                    textBox_OriginalDirectory.Text = directoryList[1];
                    textBox_UpdatedDirectory.Text = directoryList[0];
                }
                else
                {
                    textBox_OriginalDirectory.Text = directoryList[0];
                    textBox_UpdatedDirectory.Text = directoryList[1];
                }
            }
            else if (Directory.Exists(directoryList[0]))
            {
                textBox_UpdatedDirectory.Text = directoryList[0];
            }
        }


        private void panel_compareFiles_DragOver(object sender, DragEventArgs e)
        {
            e.Effect = ((string[])e.Data.GetData(DataFormats.FileDrop, false)).Length == 2 ? DragDropEffects.Copy : DragDropEffects.None;
        }

        private void panel_compareFiles_DragDrop(object sender, DragEventArgs e)
        {
            if (((string[])e.Data.GetData(DataFormats.FileDrop, false)).Length == 2)
                textBox_OriginalFile_DragDrop(sender, e);
        }

        private void panel_compareDirectories_DragOver(object sender, DragEventArgs e)
        {
            e.Effect = ((string[])e.Data.GetData(DataFormats.FileDrop, false)).Length == 2 ? DragDropEffects.Copy : DragDropEffects.None;
        }

        private void panel_compareDirectories_DragDrop(object sender, DragEventArgs e)
        {
            if (((string[])e.Data.GetData(DataFormats.FileDrop, false)).Length == 2)
                textBox_OriginalDirectory_DragDrop(sender, e);
        }

        #endregion



      

    }
}
