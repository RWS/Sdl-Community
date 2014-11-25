using Sdl.Community.ReindexTms.TranslationMemory;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;

namespace Sdl.Community.ReindexTms
{
    public partial class ReindexForm : Form
    {
        private readonly TranslationMemoryHelper _tmHelper;
        private readonly BackgroundWorker _bw;

        public ReindexForm()
        {
            InitializeComponent();
            _tmHelper = new TranslationMemoryHelper();
            _bw = new BackgroundWorker {WorkerReportsProgress = true, WorkerSupportsCancellation = true};
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            _bw.DoWork += bw_DoWork;
            _bw.RunWorkerCompleted += bw_RunWorkerCompleted;
            _bw.ProgressChanged += bw_ProgressChanged;
            
        }

        void bw_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            rtbStatus.Text =e.UserState.ToString();
        }

        void bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            btnReindex.Enabled = true;
        }

        void bw_DoWork(object sender, DoWorkEventArgs e)
        {
            var tms = e.Argument as List<TranslationMemoryInfo>;
            
            if (tms == null) return;

            _tmHelper.Reindex(tms, _bw);

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
            using(var folderDialog = new FolderBrowserDialog())
            {
                var dialogResult = folderDialog.ShowDialog();
                if (dialogResult != DialogResult.OK || string.IsNullOrEmpty(folderDialog.SelectedPath)) return;
                List<TranslationMemoryInfo> tms= _tmHelper.LoadTmsFromPath(folderDialog.SelectedPath);
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

       
    }
}
