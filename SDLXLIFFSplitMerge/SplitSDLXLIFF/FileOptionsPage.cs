using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Windows.Forms;
using System.Xml;
using Sdl.TranslationStudioAutomation.IntegrationApi;
using Sdl.Utilities.SplitSDLXLIFF.Helpers;
using Sdl.Utilities.SplitSDLXLIFF.Wizard;

namespace Sdl.Utilities.SplitSDLXLIFF
{
	public partial class FileOptionsPage : InternalWizardPage
    {
        public FileOptionsPage()
        {
            InitializeComponent();
        }

        private Dictionary<string, bool> _splitInFiles = new Dictionary<string, bool>();

        private void switchPanels(bool isMrg)
        {
            pMergeOptions.Visible = isMrg;
            pSplitOptions.Visible = !isMrg;
        }

	    private static ProjectsController GetProjectController()
	    {
		    return SdlTradosStudio.Application.GetController<ProjectsController>();
	    }

		private void findSplitInfoFile(string fileDir)
        {
	        if (Directory.Exists(fileDir))
	        {
		        string[] files = Directory.GetFiles(fileDir, "*.splitinfo");
		        if (files.Length == 1)
		        {
			        tbMergeInfoFile.Text = files[0];
		        }
	        }
        }
        
        private void bindInFiles()
        {
            lvSplitInFiles.Items.Clear();

            foreach (KeyValuePair<string, bool> _file in _splitInFiles)
                lvSplitInFiles.Items.Add(_file.Key);
        }

        private void bindSplitData()
        {
            _splitInFiles = AppOptions.splitInFiles;
            tbSplitOut.Text = AppOptions.splitOutPath;

            bindInFiles();
        }
        private void bindMergeData()
        {
            // restore current settings data
            if (AppOptions.mergeOrigFile.Length > 0 || AppOptions.mergeInPath.Length > 0)
            {
                tbMergeInfoFile.Text = AppOptions.mergeInfoFile;
                tbMergeSplitFiles.Text = AppOptions.mergeInPath;
                tbMergeOrigFile.Text = AppOptions.mergeOrigFile;
            }
            // get settings from file
            else
            {
                List<string> setts = AppSettingsFile.LoadSettings();
                if (setts.Count > 1)
                    if (Directory.Exists(Path.GetDirectoryName(setts[0])) && Directory.Exists(setts[1]))
                    {
                        tbMergeOrigFile.Text = setts[0];
                        tbMergeSplitFiles.Text = setts[1];
                        findSplitInfoFile(setts[1]);
                    }
            }
        }
        private void saveSplitData()
        {
            AppOptions.splitInFiles = _splitInFiles;
            AppOptions.splitOutPath = tbSplitOut.Text;
        }
        private void saveMergeData()
        {
            AppOptions.mergeInfoFile = tbMergeInfoFile.Text;
            AppOptions.mergeInPath = tbMergeSplitFiles.Text;
            AppOptions.mergeOrigFile = tbMergeOrigFile.Text;
        }

        private void RemoveSelectedItem()
        {
            for (int i = 0; i < lvSplitInFiles.Items.Count; i++)
                if (lvSplitInFiles.Items[i].Selected)
                    _splitInFiles.Remove(lvSplitInFiles.Items[i].Text);
        }        
        
        /// <summary>
        /// Parse project file and add sdlxliff files to dialog
        /// </summary>
        /// <param name="projectFilePath"></param>
        private void ParseProjectFile(string projPath)
        {
            // KLukianets
            // Skipping reference files while loading sdlxliff from project.

            Dictionary<string, string> targetFiles = new Dictionary<string, string>();

            // extract data from project file
            FileInfo projectFileInfo = new FileInfo(projPath);
            XmlDocument doc = new XmlDocument();
            doc.Load(projPath);
            string sourcelang = doc.SelectSingleNode("//SourceLanguageCode").InnerText;
            string targetlang = "";
            string targetPath = "";
            foreach (XmlNode node in doc.SelectNodes("//LanguageFiles/LanguageFile"))
            {
                targetlang = node.Attributes["LanguageCode"].Value;
                targetPath = projectFileInfo.DirectoryName + "\\" + node.SelectSingleNode("./FileVersions/FileVersion[last()]").Attributes["PhysicalPath"].Value;
                if (targetlang != sourcelang && Path.GetExtension(targetPath).ToLower() == ".sdlxliff")
                {
                    targetFiles.Add(targetPath, targetlang);
                }
            }

            if (targetFiles.Count > 0)
            {
                // show modal form
                TargetLanguageForm tlForm = new TargetLanguageForm(projPath, targetFiles);
                tlForm.ShowDialog();

                if (tlForm.SelectedLang.Count > 0)
                {
                    // add files with selected languages only
                    foreach (KeyValuePair<string, string> file in targetFiles)
                        if (tlForm.SelectedLang.Contains(file.Value))
                        {
                            AddFile(file.Key, false);
                        }
                }
                else MessageBox.Show(Properties.Resources.filesNoTargetLanguage,
                    Properties.Resources.Title);
            }
        }

        private void AddFile(string file, bool isSeparateFile)
        {
            if (!_splitInFiles.ContainsKey(file))
                _splitInFiles.Add(file, isSeparateFile);
            else _splitInFiles[file] = isSeparateFile;
        }

        private string validateInput(bool isMrg)
        {
            string errorMsg = "";

            if (isMrg)
            {
                if (tbMergeOrigFile.Text.Trim().Length == 0)
                    errorMsg = Properties.Resources.errMergeNoOrigFile;
                else if (!File.Exists(tbMergeOrigFile.Text))
                    errorMsg = Properties.Resources.errMergeOrigFileNotExist;
                else if (tbMergeSplitFiles.Text.Trim().Length == 0)
                    errorMsg = Properties.Resources.errMergeNoFolder;
                else if (!Directory.Exists(tbMergeSplitFiles.Text))
                    errorMsg = Properties.Resources.errMergeFolderNotExist;
                else if (tbMergeInfoFile.Text.Trim().Length == 0)
                    errorMsg = Properties.Resources.errMergeNoInfoFile;
                else if (!File.Exists(tbMergeInfoFile.Text))
                    errorMsg = Properties.Resources.errMergeInfoFileNotExist;
            }
            else
            {
                if (_splitInFiles.Count == 0)
                    errorMsg = Properties.Resources.errSplitNoFile;
                else if (tbSplitOut.Text.Trim().Length == 0)
                    errorMsg = Properties.Resources.errSplitNoFolder;
                else if (!Directory.Exists(tbSplitOut.Text))
                    errorMsg = Properties.Resources.errSplitFolderNotExist;
            }

            return errorMsg;
        }

        private void OptionsPage_SetActive(object sender, CancelEventArgs e)
        {
            // set titles for the wizard page
            if (!AppOptions.isMerge)
            {
                this.Banner.Title = Properties.Resources.msgSplitFOptsTitle;
                this.Banner.Subtitle = Properties.Resources.msgSplitFOptsSubtitle;
                bindSplitData();

                SetWizardButtons(WizardButtons.Back | WizardButtons.Next);
            }
            else
            {
                this.Banner.Title = Properties.Resources.msgMergeFOptsTitle;
                this.Banner.Subtitle = Properties.Resources.msgMergeFOptsSubtitle;
                bindMergeData();

                SetWizardButtons(WizardButtons.Back | WizardButtons.Finish);
            }

            switchPanels(AppOptions.isMerge);
        }

        private void FileOptionsPage_WizardPreNext(object sender, WizardPageEventArgs e)
        {
            // validation
            string validMsg = validateInput(AppOptions.isMerge);
            if (validMsg.Length == 0)
                this.AllowNext = true;
            else {
                this.AllowNext = false;
                MessageBox.Show(validMsg,
                    (AppOptions.isMerge ? Properties.Resources.MergeSettingsTitle : Properties.Resources.SplitSettingsTitle));
            }
        }
        private void FileOptionsPage_WizardNext(object sender, WizardPageEventArgs e)
        {
            if (AppOptions.isMerge)
                saveMergeData();
            else saveSplitData();
        }

        private void FileOptionsPage_WizardBack(object sender, WizardPageEventArgs e)
        {
            if (AppOptions.isMerge)
                saveMergeData();
            else saveSplitData();
        }

        // buttons functionality
        private void btnMergeOrigFile_Click(object sender, EventArgs e)
        {
	        var a = new FolderSelectDialog
	        {
		        Title = Properties.Resources.OrigFileDialogTitle,
				Filter = Constants.MergeOrigText,
		        Multiselect = true
	        };
			
            if (a.ShowDialog(string.Empty) == DialogResult.OK)
            {
                tbMergeOrigFile.Text = a.FileName;
            }
        }
        private void btnMergeSplitFiles_Click(object sender, EventArgs e)
        {
	        var folderDialog = new FolderSelectDialog();
			if (Directory.Exists(tbMergeSplitFiles.Text))
			{
				folderDialog.FileName = tbMergeSplitFiles.Text;
			}
			if (folderDialog.ShowDialog())
	        {
				tbMergeSplitFiles.Text = folderDialog.FileName;
		        findSplitInfoFile(folderDialog.FileName);
			}
		}
        private void btnMergeInfoFile_Click(object sender, EventArgs e)
        {
	        var a = new FolderSelectDialog
	        {
		        Title = Properties.Resources.InfoFileDialogTitle,
		        Filter = Constants.MergeInfoText,
		        Multiselect = true
	        };

            if (a.ShowDialog(string.Empty) == DialogResult.OK)
            {
                tbMergeInfoFile.Text = a.FileName;
            }
        }

        private void btnSplitOutBrowse_Click(object sender, EventArgs e)
        {
	        var folderDialog = new FolderSelectDialog();
	        if (Directory.Exists(tbSplitOut.Text))
	        {
		        folderDialog.FileName = tbSplitOut.Text;
	        }
	        if (folderDialog.ShowDialog())
	        {
		        tbSplitOut.Text = folderDialog.FileName;
	        }
        }
        private void btnSplitInFileAdd_Click(object sender, EventArgs e)
        {
			var a = new FolderSelectDialog
			{
				Title = Properties.Resources.FileDialogTitle,
				Filter = Constants.SplitInFilterText,
				Multiselect = true
			};
			if (a.ShowDialog(string.Empty) == DialogResult.OK)
            {
                string inFile = string.Empty;
                foreach (string file in a.Files)
                {
                    inFile = file.Trim().Replace("/", @"\");
                    AddFile(inFile, true);
                }
                bindInFiles();
            }
        }
        private void btnSplitRemoveAll_Click(object sender, EventArgs e)
        {
            if (lvSplitInFiles.Items.Count > 0)
            {
                _splitInFiles.Clear();
                bindInFiles();
            }
            else MessageBox.Show(Properties.Resources.msgEmptyList,
                Properties.Resources.SplitSettingsTitle);
        }
        private void btnSplitInFileRemove_Click(object sender, EventArgs e)
        {
            RemoveSelectedItem();
            bindInFiles();
        }
        private void btnSplitAddFromProject_Click(object sender, EventArgs e)
        {
	        var a = new FolderSelectDialog
	        {
		        Title = Properties.Resources.FileDialogTitle,
		        Filter = Constants.SplitAddFilterText,
		        Multiselect = true
	        };

            if (a.ShowDialog(string.Empty) == DialogResult.OK)
            {
                foreach (string file in a.Files)
                {
                    ParseProjectFile(file);
                }

                bindInFiles();
            }
        }
        private void lvSplitInFiles_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Delete)
            {
                RemoveSelectedItem();
                bindInFiles();
            }
        }

        private void lvSplitInFiles_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.Copy;
            else
                e.Effect = DragDropEffects.None; 
        }
        private void lvSplitInFiles_DragDrop(object sender, DragEventArgs e)
        {
            string[] fileList = (string[])e.Data.GetData(DataFormats.FileDrop, false);
            string inFile = "";
            foreach (string file in fileList)
            {
                if (File.Exists(file))
                {
                    inFile = file.Trim().Replace("/", @"\");
                    if (Path.GetExtension(inFile).ToLower() == ".sdlxliff")
                        AddFile(inFile, true);
                    else if (Path.GetExtension(inFile).ToLower() == ".sdlproj")
                        ParseProjectFile(inFile);
                }
            }

            bindInFiles();
        }
        private void tbSplitOut_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop) && ((string[])e.Data.GetData(DataFormats.FileDrop, false)).Length == 1)
                e.Effect = DragDropEffects.Copy;
            else
                e.Effect = DragDropEffects.None;
        }
        private void tbSplitOut_DragDrop(object sender, DragEventArgs e)
        {
            string[] pathList = (string[])e.Data.GetData(DataFormats.FileDrop, false);
            if (pathList.Length > 0)
            {
                string path = (File.Exists(pathList[0]) ? Path.GetDirectoryName(pathList[0]) : pathList[0])?.Replace("/", @"\");
                if (Directory.Exists(path))
                    tbSplitOut.Text = path;
            }
        }

        private void tbMergeOrigFile_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop) && ((string[])e.Data.GetData(DataFormats.FileDrop, false)).Length == 1)
                e.Effect = DragDropEffects.Copy;
            else
                e.Effect = DragDropEffects.None;
        }
        private void tbMergeOrigFile_DragDrop(object sender, DragEventArgs e)
        {
            string[] pathList = (string[])e.Data.GetData(DataFormats.FileDrop, false);
            if (pathList.Length > 0)
            {
                string path = pathList[0].Replace("/", @"\");
                if (Directory.Exists(Path.GetDirectoryName(path)) && File.Exists(path) && Path.GetExtension(path).ToLower() == ".sdlxliff")
                    tbMergeOrigFile.Text = path;
            }
        }
        private void tbMergeSplitFiles_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop) && ((string[])e.Data.GetData(DataFormats.FileDrop, false)).Length == 1)
                e.Effect = DragDropEffects.Copy;
            else
                e.Effect = DragDropEffects.None;
        }
        private void tbMergeSplitFiles_DragDrop(object sender, DragEventArgs e)
        {
            string[] pathList = (string[])e.Data.GetData(DataFormats.FileDrop, false);
            if (pathList.Length > 0)
            {
                string path = (File.Exists(pathList[0]) ? Path.GetDirectoryName(pathList[0]) : pathList[0])?.Replace("/", @"\");
                if (Directory.Exists(path))
                {
                    tbMergeSplitFiles.Text = path;
                    findSplitInfoFile(path);
                }
            }
        }
        private void tbMergeInfoFile_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop) && ((string[])e.Data.GetData(DataFormats.FileDrop, false)).Length == 1)
                e.Effect = DragDropEffects.Copy;
            else
                e.Effect = DragDropEffects.None;
        }
        private void tbMergeInfoFile_DragDrop(object sender, DragEventArgs e)
        {
            string[] pathList = (string[])e.Data.GetData(DataFormats.FileDrop, false);
            if (pathList.Length > 0)
            {
                string path = pathList[0].Replace("/", @"\");
                if (Directory.Exists(Path.GetDirectoryName(path)) && File.Exists(path) && Path.GetExtension(path).ToLower() == ".splitinfo")
                    tbMergeInfoFile.Text = path;
            }
        }

		private void btnLoadFilesActiveProject_Click(object sender, EventArgs e)
		{
			var projectController = GetProjectController();
			var currentProjectLocalPath = string.Empty;

			if (projectController != null)
			{
				if (projectController.CurrentProject != null)
				{
					currentProjectLocalPath = projectController.CurrentProject.GetProjectInfo() != null
						? projectController.CurrentProject.GetProjectInfo().LocalProjectFolder
						: string.Empty;
				}

				var a = new FolderSelectDialog
				{
					Title = Properties.Resources.FileDialogTitle,
					Filter = Constants.SplitAddFilterText,
					InitialDirectory = currentProjectLocalPath,
					Multiselect = true
				};

				if (a.ShowDialog(string.Empty) == DialogResult.OK)
				{
					foreach (string file in a.Files)
					{
						ParseProjectFile(file);
					}

					bindInFiles();
				}
			}
			else
			{
				btnLoadFilesActiveProject.Enabled = false;
			}
		}
	}
}
