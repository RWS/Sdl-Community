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
using Sdl.Desktop.IntegrationApi.Interfaces;
using System.Threading.Tasks;

namespace Sdl.Community.TMLifting
{
	public delegate void AddServerBasedTMsDetails(string userName, string password, string uri);
    public partial class TMLiftingForm : UserControl,IUIControl
    {
        private readonly TranslationMemoryHelper _tmHelper;
        private readonly BackgroundWorker _bw;
        private readonly Stopwatch _stopWatch;
        private readonly StringBuilder _elapsedTime;
		private LoginPage _currentInstance = null;
		private List<ReindexOperationStatus> _reIndexOperationStatus;

		public TMLiftingForm()
        {
            InitializeComponent();
            _tmHelper = new TranslationMemoryHelper();
            _stopWatch = new Stopwatch();
            _elapsedTime = new StringBuilder();
			_reIndexOperationStatus = new List<ReindexOperationStatus>();
			_bw = new BackgroundWorker { WorkerReportsProgress = true, WorkerSupportsCancellation = true };
		}

        protected override void OnLoad(EventArgs e)
        {
			base.OnLoad(e);
            _bw.DoWork += bw_DoWork;
            _bw.RunWorkerCompleted += bw_RunWorkerCompleted;
            _bw.ProgressChanged += bw_ProgressChanged;
            reIndexCheckBox.Checked = true;
			tabControlTMLifting.SelectedTab = tabControlTMLifting.TabPages["tabPageFileBasedTM"];
			tabControlTMLifting.SelectedIndexChanged += TabControlTMLifting_SelectedIndexChanged;
			groupBoxProgress.Visible = false;
		}

		private void TabControlTMLifting_SelectedIndexChanged(object sender, EventArgs e)
		{
			if(tabControlTMLifting.SelectedTab == tabControlTMLifting.TabPages["tabPageServerBasedTM"])
			{
				cleanBtn.Text = "Refresh";
				if (_currentInstance == null && !gridServerBasedTMs.Visible)
				{
					_currentInstance = new LoginPage();
					_currentInstance._addDetailsCallback = new AddServerBasedTMsDetails(AddDetailsCallback);
					_currentInstance.FormClosed += InstanceHasBeenClosed;
					_currentInstance.Show();
				}
				else
				{
					if (_currentInstance != null && !gridServerBasedTMs.Visible)
					{
						_currentInstance = new LoginPage();
						_currentInstance.FormClosed += InstanceHasBeenClosed;
						_currentInstance.BringToFront();
					}
						
				}
				btnBrowse.Enabled = false;
				chkLoadStudioTMs.Enabled = false;
				upLiftCheckBox.Enabled = false;
				cancelBtn.Enabled = false;
				reIndexCheckBox.Checked = true;
				reIndexCheckBox.Enabled = false;
			}
			else
			{
				btnBrowse.Enabled = true;
				chkLoadStudioTMs.Enabled = true;
				upLiftCheckBox.Enabled = true;
				cancelBtn.Enabled = false;
				reIndexCheckBox.Checked = true;
				reIndexCheckBox.Enabled = true;
				cleanBtn.Text = "Clean";
			}
		}
		private void InstanceHasBeenClosed(object sender, FormClosedEventArgs e)
		{
			_currentInstance = null;
		}
		private void AddDetailsCallback(string userName, string password, string uri)
		{
			try
			{
				Properties.Settings.Default.UserName = userName;
				Properties.Settings.Default.Password = password;
				Properties.Settings.Default.Uri = uri;
				Properties.Settings.Default.Save();
				
				var uriServer = new Uri(Properties.Settings.Default.Uri);
				var translationProvider = new TranslationProviderServer(uriServer, false,
											Properties.Settings.Default.UserName,
											Properties.Settings.Default.Password);
				var translationMemories = translationProvider.GetTranslationMemories();
				var tmDetails = new List<TranslationMemoryDetails>();

				foreach (var tm in translationMemories)
				{
					tmDetails.Add(new TranslationMemoryDetails
					{
						Id = tm.Id,
						Name = tm.Name,
						Description = tm.Description,
						CreatedOn = tm.CreationDate,
						Location = tm.ParentResourceGroupPath,
						Size = tm.GetTranslationUnitCount()
					});
				}
				var sortedBindingList = new SortableBindingList<TranslationMemoryDetails>(tmDetails);
				gridServerBasedTMs.DataSource = sortedBindingList;

				gridServerBasedTMs.Columns["Id"].Visible = false;
				gridServerBasedTMs.Columns["Name"].Visible = true;
				gridServerBasedTMs.Columns["Description"].Visible = true;
				gridServerBasedTMs.Columns["CreatedOn"].Visible = true;
				gridServerBasedTMs.Columns["Location"].Visible = true;
				gridServerBasedTMs.Columns["Size"].Visible = true;
				gridServerBasedTMs.Columns["Status"].Visible = true;
				gridServerBasedTMs.AllowUserToAddRows = false;
				gridServerBasedTMs.ReadOnly = true;
				gridServerBasedTMs.Visible = true;
				connectToServerBtn.Text = "Logout";
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}
		}

		private void gridServerBasedTMs_RowStateChanged(object sender, DataGridViewRowStateChangedEventArgs e)
		{
			btnReindex.Enabled = (e.StateChanged == DataGridViewElementStates.Selected);
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
                                  $"{timeSpan.Hours:00}:{timeSpan.Minutes:00}: {timeSpan.Seconds:00}.{timeSpan.Milliseconds / 10:00} hours.";
                }
                else
                {
                    elapsedTime = " Process time " +
                                  $"{timeSpan.Minutes:00}: {timeSpan.Seconds:00}.{timeSpan.Milliseconds / 10:00} minutes.";
                }

                _elapsedTime.Append("Process time:" + elapsedTime);
				rtbStatus.Clear();
                rtbStatus.AppendText(elapsedTime);
            }
            else
            {
                _stopWatch.Stop();
				rtbStatus.Clear();
				rtbStatus.AppendText("Process canceled.");
            }
			groupBoxProgress.Visible = false;
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

        private async void btnReindex_Click(object sender, EventArgs e)
        {			
			if (tabControlTMLifting.SelectedTab == tabControlTMLifting.TabPages["tabPageFileBasedTM"])
			{
				var tms = lstTms.Items.OfType<TranslationMemoryInfo>().ToList();
				if (tms.Capacity > 0)
				{
					groupBoxProgress.Visible = true;
					labelMessage.Text = "In progress, please wait... ";
					progressBarFileBased.Style = ProgressBarStyle.Marquee;
					btnReindex.Enabled = false;
					_bw.RunWorkerAsync(tms);
					cancelBtn.Enabled = true;
				}
			}
			else
			{
				foreach (dynamic row in gridServerBasedTMs.SelectedRows)
				{
					var selectedRow = gridServerBasedTMs.Rows[row.Index].DataBoundItem as TranslationMemoryDetails;
					var uriServer = new Uri(Properties.Settings.Default.Uri);
					var translationProvider = new TranslationProviderServer(uriServer, false,
												Properties.Settings.Default.UserName,
												Properties.Settings.Default.Password);
					var selectedTm = translationProvider.GetTranslationMemory(selectedRow.Id, TranslationMemoryProperties.None);
					var reindexOperation = new ScheduledReindexOperation()
					{
						TranslationMemory = selectedTm
					};


					await Task.Run(() => reindexOperation.Queue()); 
					await Task.Run(() => reindexOperation.Refresh()); 

					_reIndexOperationStatus.Add(new ReindexOperationStatus { RowIndex = row.Index, ReindexOperation = reindexOperation });
					gridServerBasedTMs.Rows[row.Index].Cells["Status"].Value = reindexOperation.Status;
				}
			}
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
				if (lstTms.Items.Count > 0)
				{
					btnReindex.Enabled = true;
				}
				else
				{
					rtbStatus.AppendText(Constants.NoLocalTmMsg);
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
				if (lstTms.Items.Count > 0)
				{
					btnReindex.Enabled = true;
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
			if (lstTms.Items.Count > 0)
			{
				btnReindex.Enabled = true;
			}
		}

        private void cleanBtn_Click(object sender, EventArgs e)
        {
			if (tabControlTMLifting.SelectedTab == tabControlTMLifting.TabPages["tabPageServerBasedTM"])
			{
				var reIndexOperationStatus = _reIndexOperationStatus;
				foreach (var state in reIndexOperationStatus)
				{
					state.ReindexOperation.Refresh();
					var newReindexStatus = state.ReindexOperation.Status;
					gridServerBasedTMs.Rows[state.RowIndex].Cells["Status"].Value = newReindexStatus;
				}
				_reIndexOperationStatus = _reIndexOperationStatus.Where(s => s.ReindexOperation.Status != ScheduledOperationStatus.Completed).ToList();
			}
			else
			{
				lstTms.Items.Clear();
				rtbStatus.Text = string.Empty;
				btnReindex.Enabled = false;
			}			
        }

		private void cancelBtn_Click(object sender, EventArgs e)
        {
            _bw.CancelAsync();
			rtbStatus.Clear();
			rtbStatus.AppendText(@"Process will be canceled.");
			labelMessage.Text = "Process will be canceled, please wait... ";
			cancelBtn.Enabled = false;
		}

		private void connectToServer_Click(object sender, EventArgs e)
		{
			if (connectToServerBtn.Text == "Logout")
			{
				groupBoxTM.Controls.Remove(gridServerBasedTMs);
				connectToServerBtn.Text = "Connect";
			}
			else
			{
				if (_currentInstance == null)
				{
					_currentInstance = new LoginPage();
					_currentInstance._addDetailsCallback = new AddServerBasedTMsDetails(AddDetailsCallback);
					_currentInstance.FormClosed += InstanceHasBeenClosed;
					_currentInstance.Show();
					groupBoxTM.Controls.Add(gridServerBasedTMs);
					if (gridServerBasedTMs.ColumnCount != 0 && !gridServerBasedTMs.Columns.Contains("Status"))
					{
						gridServerBasedTMs.Columns.Add("Status", "Status");
						gridServerBasedTMs.Columns["Status"].Visible = true;
					}
					gridServerBasedTMs.Visible = false;					
				}
			}
		}

		private void gridServerBasedTMs_CellContentClick(object sender, DataGridViewCellEventArgs e)
		{
			btnReindex.Enabled = false;
		}
	}
}