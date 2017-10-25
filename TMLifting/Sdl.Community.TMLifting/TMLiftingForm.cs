using Sdl.Community.TMLifting.Helpers;
using Sdl.Community.TMLifting.TranslationMemory;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Sdl.LanguagePlatform.TranslationMemoryApi;
using Sdl.Community.GroupShareKit;
using System.Threading.Tasks;
using Sdl.Community.GroupShareKit.Clients;
using System.Reflection;
using System.Net.Http;
using Newtonsoft.Json;
using Sdl.Community.GroupShareKit.Models.Response.TranslationMemory;

namespace Sdl.Community.TMLifting
{
    public partial class TMLiftingForm : UserControl
    {
        private readonly TranslationMemoryHelper _tmHelper;
        private readonly BackgroundWorker _bw;
		private readonly BackgroundWorker _bwGS;
        private readonly Stopwatch _stopWatch;
        private readonly StringBuilder _elapsedTime;
		private TranslationMemory.ServerBasedTranslationMemory _sbTMs;
		private readonly List<Panel> _listPanel;
		//private readonly 

		//private readonly BindingSource _bs;

		public TMLiftingForm()
        {
            InitializeComponent();
            _tmHelper = new TranslationMemoryHelper();
            _stopWatch = new Stopwatch();
            _elapsedTime = new StringBuilder();
            _bw = new BackgroundWorker { WorkerReportsProgress = true, WorkerSupportsCancellation = true };
			_bwGS = new BackgroundWorker { WorkerReportsProgress = true, WorkerSupportsCancellation = true };
			_sbTMs = new TranslationMemory.ServerBasedTranslationMemory();
			_listPanel = new List<Panel>();

			//_sbLrc.Controls.
			//_bs = new BindingSource();
		}

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            _bw.DoWork += bw_DoWork;
            _bw.RunWorkerCompleted += bw_RunWorkerCompleted;
            _bw.ProgressChanged += bw_ProgressChanged;
            reIndexCheckBox.Checked = true;


			//_bwGS.DoWork += bwGS_DoWork;
			//_bwGS.RunWorkerCompleted += bwGS_RunWorkerCompleted;
			//_bwGS.ProgressChanged += bwGS_ProgressChanged;
			
		}

		//private void bwGS_ProgressChanged(object sender, ProgressChangedEventArgs e)
		//{
		//	progressBar1.Value = e.ProgressPercentage;
		//	label1.Text = "Processing....." + progressBar1.Value.ToString() + "%";
		//}

		//private void bwGS_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		//{
		//	dataGridView1.DataSource = _bs;
		//}

		//private async void bwGS_DoWork(object sender, DoWorkEventArgs e)
		//{
		//	try
		//	{
		//		var sbTMs = await TranslationMemory.ServerBasedTranslationMemory.CreateAsync();
		//		foreach (var item in sbTMs.ServerBasedTMDetails)
		//		{
		//			_bs.DataSource = typeof(TranslationMemoryDetails);
		//			_bs.Add(item);					
		//		}


		//	}
		//	catch (Exception ex)
		//	{
		//		MessageBox.Show(ex.ToString());
		//	}
		//}

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
                                  $"{timeSpan.Hours:00}:{timeSpan.Minutes:00}: {timeSpan.Seconds:00}.{timeSpan.Milliseconds / 10:00} hours.";
                }
                else
                {
                    elapsedTime = " Process time " +
                                  $"{timeSpan.Minutes:00}: {timeSpan.Seconds:00}.{timeSpan.Milliseconds / 10:00} minutes.";
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
            if (chkLoadStudioTMs.Checked)
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
                                     where tmInfo.IsStudioTm
                                     select item).ToList();

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
            rtbStatus.AppendText(@"Process will be canceled");
        }

		//private async void button1_Click(object sender, EventArgs e)
		//{

		//}

		private async void btnOkServerBased_Click(object sender, EventArgs e)
		{
			//var a = StudioPlatform.Studio.ActiveWindow.ServiceContext.GetService<IServerConnectionService>().GetUserCredentials(new Uri("http://gs2017dev.sdl.com"), false);
			_sbTMs.GetUserCredentials();

			_sbTMs = await TranslationMemory.ServerBasedTranslationMemory.CreateAsync(userNameTxtBox.Text, passwordTxtBox.Text, serverNameTxtBox.Text);
			//gridServerBasedTMs.DataSource = sbTMs.ServerBasedTMDetails.Select(tm => new
			//{ Name = tm.Name, Location = tm.Location, Description = tm.Description, FuzzyIndexes = tm.FuzzyIndexes, TmId = tm.TranslationMemoryId }).ToList();
			var x = gridServerBasedTMs;
			gridServerBasedTMs.DataSource = _sbTMs.ServerBasedTMDetails;
			//gridServerBasedTMs.Visible = true;
			//for (int i = 0; i < gridServerBasedTMs.ColumnCount; i++)
			//{
			//	if (gridServerBasedTMs.Columns[i].Name == "TmId" )
			//	{
			//		gridServerBasedTMs.Columns[i].Visible = false;
			//	}
			//}
			
			//gridServerBasedTMs.Columns[4].Visible = false;
			//selectedRow.Cells["Tmid"].
			//var y = gridServerBasedTMs.Rows;
		}

		private async void reindexBtn_Click(object sender, EventArgs e)
		{
			var selectedRowIndex = gridServerBasedTMs.SelectedCells[0].RowIndex;
			var selectedRow = gridServerBasedTMs.Rows[selectedRowIndex].DataBoundItem as TranslationMemoryDetails;
			//var tmid = selectedRow.Cells["TranslationMemoryId"].Value.ToString();
			//gridServerBasedTMs.
			//selectedRow.DataGridView.
			var x = await _sbTMs.GroupShareClient.TranslationMemories.Reindex(selectedRow.TranslationMemoryId, new FuzzyRequest());
		}
	}
}