using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Sdl.Community.Legit.Properties;
using TmAccess;
using TMFileServices;
using Trados.Interop.SettingsManager;
using IManager = Trados.Interop.SettingsManager.IManager;
using IRunStatus = TMFileServices.IRunStatus;
using tmaTmAccessMode = TMFileServices.tmaTmAccessMode;

namespace Sdl.Community.Legit
{
    public partial class MainForm : Form
    {

        private IEnumerable<string> _filesToConvert;
        private String _sourceLangLcid;
        private String _targetLangLcid;
        private IEnumerable<Control> _toggledControls;

        public MainForm()
        {
            InitializeComponent();

            InitLanguageCombo();
            InitDragAndDropping();
            InitBackgroundConverter();

            AddTtFolderToThePathSysVariable();

            _filesToConvert = new List<String>();

            this.FormClosing += new FormClosingEventHandler(MainForm_FormClosing);
        }

        // we are saving the last used source language in the settings for the next time
        // this a poor-man's default settings feature
        void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Properties.Settings.Default.SourceLanguageLcid = (int)SourceLangCombo.SelectedValue;
            Properties.Settings.Default.Save();
        }

        private void InitLanguageCombo()
        {
            // use the trados control lib to get the list of all the languages
            // supported by the desktop tools and then use this list in our own combobox

            CultureInfo[] neutralCultures = CultureInfo.GetCultures(CultureTypes.NeutralCultures);
            CultureInfo[] cultures = CultureInfo.GetCultures(CultureTypes.AllCultures).Except(neutralCultures.ToList()).ToArray();

            SourceLangCombo.DataSource = cultures.OrderBy(l => l.EnglishName).ToList();
            SourceLangCombo.DisplayMember = "EnglishName";
            SourceLangCombo.ValueMember = "KeyboardLayoutId";
            SourceLangCombo.DropDownHeight = 200;
            SourceLangCombo.SelectedValueChanged += new EventHandler(SourceLangCombo_SelectedValueChanged);

            // set the default source language to the one saved in user settings

            TargetLangCombo.DataSource = cultures.Where(l => l.KeyboardLayoutId != (int)SourceLangCombo.SelectedValue).OrderBy(l => l.EnglishName).ToList();
            TargetLangCombo.DisplayMember = "EnglishName";
            TargetLangCombo.ValueMember = "KeyboardLayoutId";
            TargetLangCombo.DropDownHeight = 200;
            TargetLangCombo.SelectedValueChanged += new EventHandler(TargetLangCombo_SelectedValueChanged);

            SourceLangCombo.SelectedIndex = 0;
        }

        void SourceLangCombo_SelectedValueChanged(object sender, EventArgs e)
        {
            _sourceLangLcid = SourceLangCombo.SelectedValue.ToString();
            LogSelectedSourceLanguage();
        }

        void TargetLangCombo_SelectedValueChanged(object sender, EventArgs e)
        {
            _targetLangLcid = TargetLangCombo.SelectedValue.ToString();
            LogSelectedTargetLanguage();
        }

        private void LogSelectedSourceLanguage()
        {
            LogTextbox.LogMessage(string.Format(Properties.StringResources.SourceLanguageSetTo,
                                    SourceLangCombo.Text,
                                    SourceLangCombo.SelectedValue));
        }

        private void LogSelectedTargetLanguage()
        {
            LogTextbox.LogMessage(string.Format(Properties.StringResources.TargetLanguageSetTo,
                                    TargetLangCombo.Text,
                                    TargetLangCombo.SelectedValue));
        }

        private void InitDragAndDropping()
        { 
            this.DragEnter += new DragEventHandler(MainForm_DragEnter);
            this.DragDrop += new DragEventHandler(MainForm_DragDrop);
        }

        private void MainForm_DragDrop(object sender, DragEventArgs e)
        {
            var files = (e.Data.GetData(DataFormats.FileDrop) as string[]);
            if (files != null)
            {
                string path = files.FirstOrDefault();

                if (path != null && Directory.Exists(path))
                { 
                    // it's a directory so get all the files from the there
                    // including the subfolder
                    _filesToConvert = GetAllFilesFromFolderAndSubfolders(path);
                }
                else 
                {
                    // otherwise just get one or more files that are being dragged over
                    _filesToConvert = files.ToList();
                }
            }

            LogSourceFileListChange();

        }

        private IEnumerable<string> GetAllFilesFromFolderAndSubfolders(string folder)
        {
            // first let's get the files in the root of the folder
            var files = Directory.GetFiles(folder).AsEnumerable();

            // now let's go trough all the subfolders...recursion of course...
            Directory.GetDirectories(folder)
                               .Each(sf => files = files.Concat(GetAllFilesFromFolderAndSubfolders(sf)));

            return files;
        }

        private void LogSourceFileListChange()
        {
            SourceFilesList.Text = _filesToConvert.Aggregate("", (res, f) => res + f + "\r\n");

            LogTextbox.LogMessage(string.Format(StringResources.SelectedXFilesForConversion,
                                                _filesToConvert.Count()));
        }

        private void MainForm_DragEnter(object sender, DragEventArgs e)
        {
            // we will only allow files and directories to be droped on the textbox
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.Copy;
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }

        private void InitBackgroundConverter()
        {
            BackgroundConverter.DoWork += new DoWorkEventHandler(BackgroundConverter_DoWork);
            BackgroundConverter.RunWorkerCompleted += new RunWorkerCompletedEventHandler(BackgroundConverter_RunWorkerCompleted);
            BackgroundConverter.WorkerReportsProgress = true;
            BackgroundConverter.ProgressChanged += new ProgressChangedEventHandler(BackgroundConverter_ProgressChanged);
        }

        private void BackgroundConverter_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            // if the UserState of the progress changed event arg is a string then
            // we just need to report which file is being converted right now
            // and if it's a conversion result then we report the result of the conversion of course

            if (e.UserState is string)
            {
                LogTextbox.LogMessage(string.Format(StringResources.ConvertingFileX,
                                                    e.UserState as string));
            }
            else
            { 
            
                var result = e.UserState as ConversionResult;

                if (result != null && result.ConversionWasSuccessful)
                {
                    LogTextbox.LogMessage(StringResources.FileConversionSuccessfull);
                }
               
                LogTextbox.LogMessage(string.Empty);
                ConversionProgressBar.Value = e.ProgressPercentage;
            }
        }

        private void BackgroundConverter_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            LogTextbox.LogMessage(StringResources.CompletedMessage);
            LogTextbox.LogMessage(string.Empty);
            EnableDisabledControls();
        }

        private void BackgroundConverter_DoWork(object sender, DoWorkEventArgs e)
        {
            // the event args argument contains the array list 
            // with the files to be converted and the source language LCID from the combo box
            // and we need these two params for the conversion call
            var _params = (ArrayList)e.Argument;

            var worker = sender as BackgroundWorker;
            e.Result = ConvertFilesInBackground((IEnumerable<string>)_params[0], _params[1].ToString(), worker, e, (bool)_params[2], (bool)_params[3], _params[4].ToString(), _params[5].ToString());
        }


        private void BrowseForFolderButton_Click(object sender, EventArgs e)
        {
            BrowseForFilesDialog.Title = StringResources.BrowseForFilesDialogTitle;
            BrowseForFilesDialog.Filter = StringResources.BrowseForFilesDialogFilter;
            var result = BrowseForFilesDialog.ShowDialog(this); 
            if (result == DialogResult.OK)
            {
                _filesToConvert = BrowseForFilesDialog.FileNames.ToList();
                LogSourceFileListChange();
            }
        }

        private void ShowSettingsMangerButton_Click(object sender, EventArgs e)
        {
            var settingsManger = Helpers.CreateComObject<ManagerClass>(null) as IManager;
            settingsManger.AllowConflictingSettings = true;
            settingsManger.LoadFromRegistry();
            settingsManger.Interactive = true;
            settingsManger.ParentHwnd = (int)this.Handle;
            settingsManger.Show();
            settingsManger.SaveToRegistry();

            Helpers.ReleaseComObject(settingsManger);
        }

        private void ConvertButton_Click(object sender, EventArgs e)
        {
            // if there are no files selected for conversions
            // then there is nothing to convert...obviously...
            if (!_filesToConvert.Any())
            {
                MessageBox.Show(this,
                                Properties.StringResources.NoSourceFilesSelectedText,
                                Properties.StringResources.NoSourceFilesSelectedTitle,
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
                return;
            }

            if (!ckTTX.Checked && !ckBilingualDoc.Checked)
                return;

            // we should be fine here so let's start with the conversion...
            DisableEnabledControls();
            ConversionProgressBar.Value = 0;
            if (_sourceLangLcid == null) _sourceLangLcid = SourceLangCombo.SelectedValue.ToString();
            if (_targetLangLcid == null) _targetLangLcid = TargetLangCombo.SelectedValue.ToString();
            var _params = new ArrayList() { _filesToConvert, _sourceLangLcid, ckTTX.Checked, ckBilingualDoc.Checked, lableTM.Text, _targetLangLcid };
            BackgroundConverter.RunWorkerAsync(_params);
        }

        private IEnumerable<ConversionResult> ConvertFilesInBackground(IEnumerable<string> files, String sourceLcid, BackgroundWorker worker, DoWorkEventArgs e, bool doTtx, bool doBDoc, String mpath, String targetLcid)
        {
            IList<ConversionResult> _results = new List<ConversionResult>();

            var enumerable = files as IList<string> ?? files.ToList();
            int totalFiles = enumerable.Count();
            float counter = 0f;    // we'll use float for counter so the progress percentage is calculated properly below
                                    // dividing integers always gives an integer as a result

            enumerable.Each(f =>
                {
                    counter++;
                    if (String.IsNullOrEmpty(mpath))
                        mpath = CreateTm(sourceLcid, targetLcid);

                    if (doTtx)
                        _results.Add(CreateFileType(f, true, mpath));
                    if (doBDoc)
                        _results.Add(CreateFileType(f, false, mpath));

                    //_converter = ConverterFactory.GetConverterForFile(f);
                    worker.ReportProgress(0, f);

                    bool success = true;
                    if (doTtx)
                    {
                        if (!File.Exists(String.Format("{0}.ttx", f)))
                        {
                            worker.ReportProgress((int) ((counter/totalFiles)*100), "file to a ttx one failed.");
                            success = false;
                        }
                        else
                            worker.ReportProgress((int) ((counter/totalFiles)*100), "file to a ttx one succeeded.");
                    }

                    if (doBDoc)
                    {
                        if (!File.Exists(String.Format("{0}.BAK",Path.Combine(Path.GetDirectoryName(f),Path.GetFileNameWithoutExtension(f)))))
                        {
                            worker.ReportProgress((int) ((counter/totalFiles)*100),
                                                  "file to a bilingual doc failed.");
                            success = false;
                        }
                        else
                            worker.ReportProgress((int) ((counter/totalFiles)*100),
                                                  "file to a bilingual doc succeeded.");
                    }

                    if (success)
                        worker.ReportProgress((int) ((counter/totalFiles)*100),
                                              _results.Count == 0 ? null : _results.Last());
                }
                );

            return _results;
        }

        private static ConversionResult CreateFileType(string file, bool doTtx, String tmPath)
        {
            TranslateTask t = new TranslateTaskClass();
            t.Documents.Clear();
            t.Documents.Add(file);
            t.OpenTranslationMemory(String.Format("{0}.mdf", tmPath.Replace(".tmw", String.Empty)), Environment.UserName,
                                    tmaTmAccessMode.tmaTmAccessExclusive, null, 0);
            t.Settings.LogFileName = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                @"SDL\OpenExchange\TTXIT\Log\conversion.log");

            if (!Directory.Exists(Path.GetDirectoryName(t.Settings.LogFileName)))
                Directory.CreateDirectory(Path.GetDirectoryName(t.Settings.LogFileName));
            using (var sw = new StreamWriter(t.Settings.LogFileName, true))
            {
                sw.Write(" ");
            }

            t.Settings.SegmentUnknownSentences = true;
            t.Settings.SaveDocAsTtx = doTtx;

            var backdoor = (IRunStatus) t;
            int p1, p2, p3, p4;
            backdoor.GetParameters(out p1, out p2, out p3, out p4);
            GenerateResponse(ref p1, ref p2, ref p3, ref p4);
            backdoor.SetParameters(p1, p2, p3, p4);

            t.Execute();
            t.CloseTranslationMemory();
            
            return new ConversionResult(file);
        }

        private static String CreateTm(String sourceLcid, String targetLcid)
        {
            String filePath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                @"SDL\OpenExchange\TTXIT\TM\TM.tmw");
            if (!Directory.Exists(Path.GetDirectoryName(filePath)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(filePath));
            }
            CreateTmTask tTm = new CreateTmTaskClass();
            tTm.Settings.TranslationMemoryType = tmaTranslationMemoryType.tmaTranslationMemoryTypeFile;
            tTm.Settings.SourceLocale = sourceLcid;
            tTm.Settings.TargetLocales = targetLcid;
            tTm.Settings.FileName = filePath;

            var backdoor = (IRunStatus) tTm;
            int p1, p2, p3, p4;
            backdoor.GetParameters(out p1, out p2, out p3, out p4);
            GenerateResponse(ref p1, ref p2, ref p3, ref p4);
            backdoor.SetParameters(p1, p2, p3, p4);


            tTm.Execute();

            return filePath;
        }

        public static void GenerateResponse(ref int p1, ref int p2, ref int p3, ref int p4)
        {
            int tmp = p1;
            p1 = (int)(p2 ^ (0xA98C3793 + 0));
            p2 = (int)(p3 ^ (0xA98C3793 + 1));
            p3 = (int)(p4 ^ (0xA98C3793 + 2));
            p4 = (int)(tmp ^ (0xA98C3793 + 3));
        }

        private void FilterManagerButton_Click(object sender, EventArgs e)
        {
            string installFolder = GetDesktopToolsInstallationFolder();

            if (string.IsNullOrEmpty(installFolder))
            { 
                MessageBox.Show(this, StringResources.InstallFolderNotFoundInRegistryText, 
                                      StringResources.InstallFolderNotFoundInRegistryTitle, 
                                      MessageBoxButtons.OK, 
                                      MessageBoxIcon.Error);
                return;
            }

            // at the point we should have found the path to the filter settings manager so we can start it...
            string filterManagerFilePath = string.Format("{0}\\TT\\TradosFilterSettings.exe", installFolder);
            var manageProcess = new System.Diagnostics.Process();
            manageProcess.StartInfo.FileName = filterManagerFilePath;
            manageProcess.StartInfo.UseShellExecute = false;

            DisableEnabledControls();

            manageProcess.Start();
            manageProcess.WaitForExit();
            manageProcess.Dispose();

            EnableDisabledControls();
        }

        private void AddTtFolderToThePathSysVariable()
        {
            string installFolder = GetDesktopToolsInstallationFolder();
            string ttPath = string.Format("{0}\\TT\\", installFolder);

            string path = Environment.GetEnvironmentVariable("PATH");
            if (path != null && !path.ToLower().Contains(ttPath.ToLower()))
            { 
                path = path.EndsWith(";") ? string.Format("{0}{1}", path, ttPath) : string.Format("{0};{1}", path, ttPath);
                Environment.SetEnvironmentVariable("PATH", path, EnvironmentVariableTarget.Process);
            }
        }

        private string GetDesktopToolsInstallationFolder()
        { 
            // get the 2007 stub ior actual 2007 install folder from the registry
            var ttfolder = Environment.GetFolderPath(Environment.SpecialFolder.CommonProgramFiles) + @"\SDL\T2007";
            
            Microsoft.Win32.RegistryKey regKey = Microsoft.Win32.Registry
                                                                 .LocalMachine
                                                                 .OpenSubKey("SOFTWARE\\TRADOS\\Translation Tools\\8.0");
            if (regKey != null)
            {
                ttfolder = regKey.GetValue("InstallDir") as string;
                regKey.Close();
            }

            return ttfolder;
        }

        private void DisableEnabledControls()
        {
            _toggledControls = this.Controls.Cast<Control>().Where(c => c.Tag != null && c.Tag.ToString().ToLower() == "toggle")
                                                            .Each(c => c.Enabled = false);
        }

        private void EnableDisabledControls()
        {
            _toggledControls = _toggledControls.Each(c => c.Enabled = true);
        }

        private void ClearFilesListboxLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            _filesToConvert = new List<String>();
            SourceFilesList.Clear();
            LogSourceFileListChange();
        }

        private void ClearLogLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            LogTextbox.Clear();
        }

        private void SaveLogToTextFile_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            SaveToFileDialog.Title = StringResources.SaveLogDialogTitle;
            SaveToFileDialog.Filter = StringResources.SaveLogDialogFilter;
            SaveToFileDialog.OverwritePrompt = true;
            SaveToFileDialog.FileName = StringResources.SaveLogDialogDefaultFileName;

            DialogResult result = SaveToFileDialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                string logFilePath = SaveToFileDialog.FileName;
                using (var writer = new StreamWriter(new FileStream(logFilePath, FileMode.Create, FileAccess.ReadWrite),
                                                              System.Text.Encoding.UTF8))
                {
                    writer.Write(LogTextbox.Text);
                    MessageBox.Show(StringResources.SaveLogSuccessMessageText, 
                                    StringResources.SaveLogSuccessMessageTittle, 
                                    MessageBoxButtons.OK);
                }
            }
        }

        private void OpenResultingFolderLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            var folderPath = _filesToConvert.Select(Path.GetDirectoryName).FirstOrDefault();
            if (!string.IsNullOrEmpty(folderPath))
            {
                System.Diagnostics.Process.Start(folderPath);
            }
            else
            {
                MessageBox.Show(StringResources.OpenTargetFolderMessageText,
                                StringResources.OpenTargetFolderMessageTitle,
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
            }
        }

        private void btnTM_Click(object sender, EventArgs e)
        {
            BrowseForFilesDialog.Title = StringResources.MainForm_btnTM_Click_Select_TM;
            BrowseForFilesDialog.Filter = StringResources.MainForm_btnTM_Click_TM_Files____tmw____tmw;
            var result = BrowseForFilesDialog.ShowDialog(this);
            if (result == DialogResult.OK)
            {
                lableTM.Text = BrowseForFilesDialog.FileName;
                SourceLangCombo.Visible = TargetLangCombo.Visible = false;
                txtSourceLanguage.Visible = txtTargetLanguage.Visible = true;
                AddLanguagesFromTmToCombo(lableTM.Text);
            }
        }

        private void AddLanguagesFromTmToCombo(String tmFilePath)
        {
            var oTm = new TranslationMemory();
            oTm.Open(String.Format("{0}.mdf", tmFilePath.Replace(".tmw", String.Empty)), Environment.UserName,
                                    TmAccess.tmaTmAccessMode.tmaTmAccessRead, null, 0);
            _sourceLangLcid = oTm.Setup.SourceLocale;
            _targetLangLcid = oTm.Setup.TargetLocales;
            try
            {
                var ci = new CultureInfo(Convert.ToInt32(_sourceLangLcid));
                txtSourceLanguage.Text = ci.DisplayName;
                var targetLanguages = _targetLangLcid.Split(',');
                txtTargetLanguage.Text = String.Empty;
                foreach (var targetLanguage in targetLanguages)
                {
                    var tci = new CultureInfo(Convert.ToInt32(targetLanguage));
                    txtTargetLanguage.Text += tci.DisplayName + ",";
                }
                if (txtTargetLanguage.Text != String.Empty) txtTargetLanguage.Text = txtTargetLanguage.Text.Substring(0, txtTargetLanguage.Text.Length - 1);

                HelpTooltip.SetToolTip(txtSourceLanguage, txtSourceLanguage.Text);
                HelpTooltip.SetToolTip(txtTargetLanguage, txtTargetLanguage.Text);
            } catch
            {
            }
            oTm.Close();
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            lableTM.Text = String.Empty;
            SourceLangCombo.Visible = TargetLangCombo.Visible = true;
            txtSourceLanguage.Visible = txtTargetLanguage.Visible = false;
            InitLanguageCombo();
        }
    }
}
