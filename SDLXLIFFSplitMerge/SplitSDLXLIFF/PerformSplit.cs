using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Sdl.Utilities.SplitSDLXLIFF.Wizard;
using Sdl.Utilities.SplitSDLXLIFF.Lib;
using System.Threading;
using System.IO;

namespace Sdl.Utilities.SplitSDLXLIFF
{
    public partial class PerformSplit : InternalWizardPage
    {
        delegate void UpdateProgressDelegate(double value);
        delegate void UpdateLogDelegate(string value);
        delegate void UpdateButtonsDelegate(bool value);

        private enum SplitStatus { Success, SuccessWithErrors, Fail };

        public PerformSplit()
        {
            InitializeComponent();
        }

        private void PerformSplit_SetActive(object sender, CancelEventArgs e)
        {
            this.Banner.Title = Properties.Resources.msgSplitTitle;
            this.Banner.Subtitle = Properties.Resources.msgSplitSubtitle;

            SetWizardButtons(WizardButtons.Start | WizardButtons.Close);
            changeButtonsView(false);
            EnableCancelButton(false);
        }
        private void PerformSplit_PostSetActive(object sender, CancelEventArgs e)
        {
            tbLog.Text = "";
            updateLog(Properties.Resources.msgSplitStart);

            // start split process >>
            Thread t = new Thread(new ThreadStart(SplitFiles));
            t.IsBackground = true;
            t.Start();
            // <<
        }
        private void PerformSplit_WizardNewStart(object sender, WizardPageEventArgs e)
        {
            AppOptions.RestoreOptions();
            AppOptions.splitOpts.SplitNonCountStatus = new List<SegStatus>();
            AppOptions.splitOpts.SplitNonCountStatus.Add(SegStatus.ApprovedSignOff);
            AppOptions.splitOpts.SplitNonCountStatus.Add(SegStatus.ApprovedTranslation);
        }
        private void btnOpenFolder_Click(object sender, EventArgs e)
        {
            string myPath = AppOptions.splitOutPath;
            System.Diagnostics.Process prc = new System.Diagnostics.Process();
            prc.StartInfo.FileName = myPath;
            prc.Start();
        }

        private void SplitFiles()
        {
            SplitStatus status = SplitStatus.Success;
            FileParser _splitter;
            int filesCnt = AppOptions.splitInFiles.Count;
            try
            {
                for (int f = 0; f < filesCnt; f++)
                {
                    updateLog(string.Format(Properties.Resources.msgSplitting, AppOptions.splitInFiles.ElementAt(f).Key));
                    updateProgressFiles((f * 100) / filesCnt);
                    updateProgressStatus(string.Format(Properties.Resources.msgSplittingFiles, f + 1, filesCnt));

                    _splitter = new FileParser(AppOptions.splitInFiles.ElementAt(f).Key,
                        AppOptions.splitInFiles.ElementAt(f).Value,
                        AppOptions.splitOutPath);
                    _splitter.OnProgress += new FileParser.OnProgressDelegate(updateProgress);

                    _splitter.Split(AppOptions.splitOpts);

                    if (_splitter.FilesCount.HasValue && _splitter.FilesCount.Value > 0)
                    {
                        updateLog(string.Format(Properties.Resources.msgSplitScc, _splitter.FilesCount.Value));

                        #region write warnings
                        if (AppOptions.splitOpts.Criterion == SplitOptions.SplitType.EqualParts 
                            && _splitter.FilesCount.Value < AppOptions.splitOpts.PartsCount)
                            updateLog(Properties.Resources.msgSplitWarning);
                        if (AppOptions.splitOpts.Criterion == SplitOptions.SplitType.SegmentNumbers
                            && _splitter.Warnings.Count > 0)
                            foreach (Warning wrn in _splitter.Warnings)
                                updateLog(wrn.GetMessage());
                        #endregion

                        // save last split settings
                        List<string> settsList = new List<string>(2);
                        settsList.Add(_splitter.FilePath);
                        settsList.Add(_splitter.OutPath);
                        AppSettingsFile.SaveSettings(settsList);
                    }
                    else if (_splitter.FilesCount.HasValue && _splitter.FilesCount.Value == -1)
                    {
                        updateLog(Properties.Resources.msgSplitCorruptFile);
                        status = SplitStatus.SuccessWithErrors;
                    }
                    else updateLog(Properties.Resources.errSplitNoFiles);
                }
            }
            catch (Exception ex)
            {
                updateLog(string.Format(Properties.Resources.errSplitUnexp, ex.Message));
                status = SplitStatus.Fail;
            }

            updateProgress(100);
            updateProgressFiles(100);
            updateProgressStatus(string.Format(Properties.Resources.msgSplitFinished, filesCnt));

            switch (status)
            {
                case SplitStatus.Success:
                    MessageBox.Show(Properties.Resources.msbSplitFinished,
                                         Properties.Resources.SplitTitle,
                                         MessageBoxButtons.OK,
                                         MessageBoxIcon.Information);
                    break;
                case SplitStatus.SuccessWithErrors:
                    MessageBox.Show(Properties.Resources.msbSplitFinishedWithErr,
                              Properties.Resources.SplitTitle,
                                         MessageBoxButtons.OK,
                                         MessageBoxIcon.Information);
                    break;
                case SplitStatus.Fail:
                    MessageBox.Show(Properties.Resources.errSplit,
                                 Properties.Resources.SplitTitle,
                                         MessageBoxButtons.OK,
                                         MessageBoxIcon.Error);
                    break;
            }

            changeButtonsView(true);
        }

        private void updateProgressStatus(string progress)
        {
            if (InvokeRequired)
            {
                // not in the UI thread, so need to call BeginInvoke
                BeginInvoke(new UpdateLogDelegate(updateProgressStatus), new object[] { progress });
                return;
            }
            lblProgress.Text = progress;
        }
        private void updateProgressFiles(double progress)
        {
            if (InvokeRequired)
            {
                // not in the UI thread, so need to call BeginInvoke
                BeginInvoke(new UpdateProgressDelegate(updateProgressFiles), new object[] { progress });
                return;
            }
            pbProgressFiles.Value = (int)progress;
        }
        private void updateProgress(double progress)
        {
            if (InvokeRequired)
            {
                // not in the UI thread, so need to call BeginInvoke
                BeginInvoke(new UpdateProgressDelegate(updateProgress), new object[] { progress });
                return;
            }
            pbProgress.Value = (int)progress;
        }
        private void updateLog(string msg)
        {
            if (InvokeRequired)
            {
                // not in the UI thread, so need to call BeginInvoke
                BeginInvoke(new UpdateLogDelegate(updateLog), new object[] { msg });
                return;
            }
            tbLog.AppendText(msg.Replace("\\r\\n", "\r\n"));
        }
        private void changeButtonsView(bool isEnabled)
        {
            if (InvokeRequired)
            {
                // not in the UI thread, so need to call BeginInvoke
                BeginInvoke(new UpdateButtonsDelegate(changeButtonsView), new object[] { isEnabled });
                return;
            }
            EnableCloseButton(isEnabled);
            EnableStartButton(isEnabled);
            btnOpenFolder.Enabled = isEnabled;
        }

    }
}
