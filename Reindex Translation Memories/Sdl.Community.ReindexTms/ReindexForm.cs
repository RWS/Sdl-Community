using Sdl.Community.ReindexTms.TranslationMemory;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Sdl.Community.ReindexTms
{
    public partial class ReindexForm : Form
    {
        private TranslationMemoryHelper tmHelper;
        private BackgroundWorker bw;
        public ReindexForm()
        {
            InitializeComponent();
            tmHelper = new TranslationMemoryHelper();
            bw = new BackgroundWorker() { WorkerReportsProgress = true, WorkerSupportsCancellation = true };
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            bw.DoWork += bw_DoWork;
            bw.RunWorkerCompleted += bw_RunWorkerCompleted;
            bw.ProgressChanged += bw_ProgressChanged;
            
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

            tmHelper.Reindex(tms, bw);

        }

        private void btnReindex_Click(object sender, EventArgs e)
        {
            btnReindex.Enabled = false;

            List<TranslationMemoryInfo> tms = new List<TranslationMemoryInfo>();

            foreach (var item in lstTms.Items)
            {
                TranslationMemoryInfo tmInfo = item as TranslationMemoryInfo;
                if(tmInfo != null)
                {
                    tms.Add(tmInfo);
                }
            }

            bw.RunWorkerAsync(tms);
            
        }

        private void chkLoadStudioTMs_CheckedChanged(object sender, EventArgs e)
        {
            if(chkLoadStudioTMs.Checked)
            {
                List<TranslationMemoryInfo> tms = tmHelper.LoadLocalUserTms();

                foreach (var tm in tms)
                {
                    lstTms.Items.Add(tm);
                }
            }
            else
            {
                List<object> toRemoveItems = new List<object>();
                foreach (var item in lstTms.Items)
                {
                    TranslationMemoryInfo tmInfo = item as TranslationMemoryInfo;
                    if(tmInfo.IsStudioTm)
                    {
                        toRemoveItems.Add(item);
                    }
                }

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
                if(dialogResult == System.Windows.Forms.DialogResult.OK && !string.IsNullOrEmpty(folderDialog.SelectedPath))
                {
                   List<TranslationMemoryInfo> tms= tmHelper.LoadTmsFromPath(folderDialog.SelectedPath);
                    foreach (var tm in tms)
                    {
                        lstTms.Items.Add(tm);
                    }
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

            string[] paths = data as string[];

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

            string[] paths = data as string[];

            if (paths == null) return;

            List<TranslationMemoryInfo> tms = tmHelper.LoadTmsFromPath(paths);
            foreach (var tm in tms)
            {
                lstTms.Items.Add(tm);
            }
        }

       
    }
}
