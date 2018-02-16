using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Sdl.Utilities.SplitSDLXLIFF.Wizard;
using System.IO;
using Sdl.Utilities.SplitSDLXLIFF.Lib;
using System.Threading;

namespace Sdl.Utilities.SplitSDLXLIFF
{
    public partial class PerformMerge : InternalWizardPage
    {
        delegate void UpdateProgressDelegate(double value);
        delegate void UpdateLogDelegate(string value);
        delegate void UpdateButtonsDelegate(bool value);

        public PerformMerge()
        {
            InitializeComponent();
        }

        private void PerformMerge_SetActive(object sender, CancelEventArgs e)
        {
            this.Banner.Title = Properties.Resources.msgMergeTitle;
            this.Banner.Subtitle = Properties.Resources.msgMergeSubtitle;

            SetWizardButtons(WizardButtons.Start | WizardButtons.Close);
            changeButtonsView(false);
            EnableCancelButton(false);
        }
        private void PerformMerge_PostSetActive(object sender, CancelEventArgs e)
        {
            tbLog.Text = "";
            updateLog(Properties.Resources.msgMergeStart);

            // merge process >>
            Thread t = new Thread(new ThreadStart(MergeFiles));
            t.IsBackground = true;
            t.Start();
            // <<
        }
        private void PerformMerge_WizardNewStart(object sender, WizardPageEventArgs e)
        {
            AppOptions.RestoreOptions();
            AppOptions.splitOpts.SplitNonCountStatus = new List<SegStatus>();
            AppOptions.splitOpts.SplitNonCountStatus.Add(SegStatus.ApprovedSignOff);
            AppOptions.splitOpts.SplitNonCountStatus.Add(SegStatus.ApprovedTranslation);
        }      
        private void btnOpenFolder_Click(object sender, EventArgs e)
        {
            string myPath = Path.GetDirectoryName(AppOptions.mergeOrigFile);
            System.Diagnostics.Process prc = new System.Diagnostics.Process();
            prc.StartInfo.FileName = myPath;
            prc.Start();
        }

        private void MergeFiles()
        {
            bool isSuccess = false;
            try
            {
                // updateLog(string.Format("\r\nStart Time: {0}", DateTime.Now.ToLongTimeString()));
                updateLog(string.Format(Properties.Resources.msgMerging, AppOptions.mergeOrigFile));
                Application.DoEvents();

                FileMerger _merger = new FileMerger(AppOptions.mergeOrigFile, AppOptions.mergeInPath, AppOptions.mergeInfoFile);
                _merger.OnProgress += new FileMerger.OnProgressDelegate(updateProgress);
                _merger.Merge();

                if (_merger.FilesCount.HasValue)
                    updateLog(string.Format(Properties.Resources.msgMergeScc, _merger.FilesCount.Value));
                else updateLog(Properties.Resources.errMergeNoFiles);

                isSuccess = true;
                // updateLog(string.Format("End Time: {0}\r\n", DateTime.Now.ToLongTimeString()));
            }
            catch (Exception ex)
            {
                updateLog(string.Format(Properties.Resources.errMergeUnexp, ex.Message));
            }

            updateProgress(100);

            if (isSuccess) MessageBox.Show(Properties.Resources.msbMergeFinished,
                                         Properties.Resources.MergeTitle,
                                         MessageBoxButtons.OK,
                                         MessageBoxIcon.Information);
            else MessageBox.Show(Properties.Resources.errMerge,
                                         Properties.Resources.MergeTitle,
                                         MessageBoxButtons.OK,
                                         MessageBoxIcon.Error);

            changeButtonsView(true);
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
