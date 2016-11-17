using Sdl.Community.ReindexTms.TranslationMemory;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Sdl.Community.ReindexTms.Helpers;

namespace Sdl.Community.ReindexTms
{
    public partial class ReindexForm : UserControl
    {
        private readonly TranslationMemoryHelper _tmHelper;
        private readonly BackgroundWorker _bw;
        private readonly Stopwatch _stopWatch;
        private readonly StringBuilder _elapsedTime;

        public ReindexForm()
        {
            InitializeComponent();
            _tmHelper = new TranslationMemoryHelper();
            _stopWatch = new Stopwatch();
            _elapsedTime = new StringBuilder();
            _bw = new BackgroundWorker {WorkerReportsProgress = true, WorkerSupportsCancellation = true};
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            _bw.DoWork += bw_DoWork;
            _bw.RunWorkerCompleted += bw_RunWorkerCompleted;
            _bw.ProgressChanged += bw_ProgressChanged;
            reIndexCheckBox.Checked = true;


        }

        void bw_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {

            rtbStatus.Text = e.UserState.ToString();

        }

        void bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {

            if (!e.Cancelled)
            {
                btnReindex.Enabled = true;
                _stopWatch.Stop();
                string elapsedTime;
                var timeSpan = _stopWatch.Elapsed;
                if (timeSpan.Hours != 00)
                {
                    elapsedTime = " Process time " +
                                  $"{timeSpan.Hours:00}:{timeSpan.Minutes:00}: {timeSpan.Seconds:00}.{timeSpan.Milliseconds/10:00} hours.";
                }
                else
                {
                    elapsedTime = " Process time " +
                                  $"{timeSpan.Minutes:00}: {timeSpan.Seconds:00}.{timeSpan.Milliseconds/10:00} minutes.";
                }


                _elapsedTime.Append("Process time:" + elapsedTime);
                rtbStatus.AppendText(elapsedTime);
            }
            else
            {
                _stopWatch.Stop();
                rtbStatus.AppendText("Process canceled.");
            }
            

        }

        void bw_DoWork(object sender, DoWorkEventArgs e)
        {
            _stopWatch.Start();
            var tms = e.Argument as List<TranslationMemoryInfo>;

            if (tms == null) return;

            _tmHelper.Process(tms, _bw, reIndexCheckBox.Checked, upLiftCheckBox.Checked);
            var bw = sender as BackgroundWorker;
            if (bw != null && bw.CancellationPending)
            {
                e.Cancel = true;
                btnReindex.Enabled = true;
            }
        }

        private void btnReindex_Click(object sender, EventArgs e)
        {
            btnReindex.Enabled = false;

            var tms = lstTms.Items.OfType<TranslationMemoryInfo>().ToList();

            _bw.RunWorkerAsync(tms);
            
        }

        private void chkLoadStudioTMs_CheckedChanged(object sender, EventArgs e)
        {
            if(chkLoadStudioTMs.Checked)
            {
                var tms = _tmHelper.LoadLocalUserTms();

                foreach (var tm in tms)
                {
                    lstTms.Items.Add(tm);
                }
            }
            else
            {
                var toRemoveItems = (from object item in lstTms.Items
                                     let tmInfo = item as TranslationMemoryInfo
                                     where tmInfo.IsStudioTm select item).ToList();

                foreach (var item in toRemoveItems)
                {
                    lstTms.Items.Remove(item);
                }
            }
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            var folderDialog = new FolderSelectDialog();

            if (folderDialog.ShowDialog())
            {
                List<TranslationMemoryInfo> tms = _tmHelper.LoadTmsFromPath(folderDialog.FileName);
                foreach (var tm in tms)
                {
                    lstTms.Items.Add(tm);
                }
            }
        }

        private void lstTms_DragOver(object sender, DragEventArgs e)
        {
            //// Determine whether string data exists in the drop data. If not, then 
            //// the drop effect reflects that the drop cannot occur. 
            if (!e.Data.GetDataPresent(DataFormats.FileDrop, false))
            {

                e.Effect = DragDropEffects.None;
                return;
            }

            object data = e.Data.GetData(DataFormats.FileDrop, false);

            if (data == null)
            {
                e.Effect = DragDropEffects.None;
                return;
            }

            var paths = data as string[];

            if (paths == null)
            {
                e.Effect = DragDropEffects.None;
                return;
            }

            e.Effect = DragDropEffects.Copy;
        }

        private void lstTms_DragDrop(object sender, DragEventArgs e)
        {
            object data = e.Data.GetData(DataFormats.FileDrop, false);

            if (data == null) return;

            var paths = data as string[];

            if (paths == null) return;

            List<TranslationMemoryInfo> tms = _tmHelper.LoadTmsFromPath(paths);
            foreach (var tm in tms)
            {
                lstTms.Items.Add(tm);
            }
        }

        private void cleanBtn_Click(object sender, EventArgs e)
        {
            lstTms.Items.Clear();
            rtbStatus.Text = string.Empty;
        }

        private void cancelBtn_Click(object sender, EventArgs e)
        {
            _bw.CancelAsync();
            rtbStatus.Text = string.Empty;
            rtbStatus.Text = @"Process will be canceled";
           
        }
    }
}
