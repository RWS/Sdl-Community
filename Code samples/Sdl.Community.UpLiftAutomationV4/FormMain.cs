using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Sdl.Community.FragmentAlignmentAutomation
{
    public partial class FormMain : Form
    {
        public FormMain()
        {
            InitializeComponent();

            listViewMain.DragDrop += listViewMain_DragDrop;
            listViewMain.DragOver += listViewMain_DragOver;
            listViewMain.KeyUp += listViewMain_KeyUp;

            toolStripButtonStartProcessing.Click += toolStripButtonStartProcessing_Click;
        }

        private void listViewMain_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Delete) return;

            foreach (ListViewItem item in listViewMain.SelectedItems)            
                 item.Remove();               
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            Text = Application.ProductName + " ("+Application.ProductVersion+ ") [BETA]";
            AutomationProcessor = new Processor();
            AutomationProcessor.OnProgressChanged += processor_OnProgressChanged;
        }

        private void toolStripButtonStartProcessing_Click(object sender, EventArgs e)
        {
            StartProcessing();
        }


        private ListViewItem CurrentViewItem { get; set; }

        private ProcessingModel ProcessingModel
        {
            get
            {
                return CurrentViewItem.Tag as ProcessingModel;
            }
        }


        private Processor AutomationProcessor { get; set; }

        private async void StartProcessing()
        {
            try
            {
                SetUICrap_StartProcessing();

                foreach (ListViewItem item in listViewMain.Items)
                {
                    CurrentViewItem = item;

                    // yuk! quick 'n dirty feedback for UI
                    textBoxMain.Text += "\r\n\r\nProcessing: " + ProcessingModel.File.FullName;
                    textBoxMain.Text += "\r\nOutput: " + ProcessorUtil.GetOutputTmFullPath(ProcessingModel.File);


                    await Run(ProcessingModel.File.FullName, ProcessorUtil.GetOutputTmFullPath(ProcessingModel.File));


                    StatusBarProgressBar.Value++;
                    StatusBarPercentage.Text =
                        ProcessorUtil.GetPercentage(StatusBarProgressBar.Maximum, StatusBarProgressBar.Value) + "%";
                }
            }
            finally
            {
                SetUICrap_EndProcessing();
            }
        }


   
        private bool IsInGrid(FileInfo tmFileInfo)
        {
            return (from ListViewItem item in listViewMain.Items select item.Tag as ProcessingModel)
                .Any(model => model.File.FullName == tmFileInfo.FullName);
        }
        
        private async Task Run(string tmIn, string tmOut)
        {
            try
            {
                AutomationProcessor.SetProcessingVariables(tmIn, tmOut);
                await AutomationProcessor.Run();
            }
            catch (Exception e)
            {
                ProcessingModel.ProgressArgs.Description = e.Message;
                ProcessingModel.HasError = true;
                ProcessingModel.Progress = ProcessingModel.ProgressType.IsComplete;


                UpdateListViewItems(ProcessingModel.ProgressArgs);
            }
        }

        private void processor_OnProgressChanged(object sender, ProgressEventArgs e)
        {
            ProcessingModel.ProgressArgs = e;

            UpdateListViewItems(e);
        }

        private static void LoadAboutInfo()
        {
            var f = new AboutBoxMain();
            f.ShowDialog();
        }

        private void UpdateListViewItems(ProgressEventArgs e)
        {
            // yuk!!!  should really look at a binding approach

            if (listViewMain.InvokeRequired)
            {
                listViewMain.Invoke(new MethodInvoker(delegate
                {
                    listViewMain.Items[CurrentViewItem.Index].SubItems[2].Text =
                        ProcessorUtil.GetPercentage(e.TotalUnits, e.CurrentProgress) + "%";
                    listViewMain.Items[CurrentViewItem.Index].SubItems[3].Text = ProcessorUtil.GetProgressMessage(e);

                }));
            }
            else
            {
                CurrentViewItem.SubItems[2].Text = ProcessorUtil.GetPercentage(e.TotalUnits, e.CurrentProgress) + "%";
                CurrentViewItem.SubItems[3].Text = ProcessorUtil.GetProgressMessage(e);
            }
        }


        #region  |  Helpers and form events  |

        private void AddTmFileToGrid(FileInfo tmFileInfo)
        {
            if (IsInGrid(tmFileInfo))
                return;

            var item = listViewMain.Items.Add(tmFileInfo.FullName);
            item.SubItems.Add(ProcessorUtil.GetOutputTmFullPath(tmFileInfo));
            item.SubItems.Add("0%");
            item.SubItems.Add("Waiting...");
            item.Tag = new ProcessingModel
            {
                File = tmFileInfo
            };
        }

        private void SetUICrap_StartProcessing()
        {
            // yuk! quick 'n dirty feedback for UI

            textBoxMain.Text = string.Empty;
            textBoxMain.Text += "Start Processing\r\n";
            textBoxMain.Text += DateTime.Now + "\r\n";


            listViewMain.Enabled = false;
            toolStripMain.Enabled = false;
            aboutToolStripMenuItem.Enabled = false;

            StatusBarStatusLabel.Text = "Processing...";
            StatusBarProgressBar.Value = 0;
            StatusBarProgressBar.Maximum = listViewMain.Items.Count;
            StatusBarPercentage.Text =
                ProcessorUtil.GetPercentage(StatusBarProgressBar.Maximum, StatusBarProgressBar.Value) + "%";
        }

        private void SetUICrap_EndProcessing()
        {
            // yuk! quick 'n dirty feedback for UI

            textBoxMain.Text += "\r\n\r\n" + DateTime.Now + "\r\n";
            textBoxMain.Text += "End Processing\r\n";

            listViewMain.Enabled = true;
            toolStripMain.Enabled = true;
            aboutToolStripMenuItem.Enabled = true;

            StatusBarStatusLabel.Text = "Ready ";
        }

        private static void listViewMain_DragOver(object sender, DragEventArgs e)
        {
            e.Effect = e.Data.GetDataPresent(DataFormats.FileDrop)
                ? DragDropEffects.Copy
                : DragDropEffects.None;
        }

        private void listViewMain_DragDrop(object sender, DragEventArgs e)
        {
            var objectList = e.Data.GetData(DataFormats.FileDrop, false) as string[];
            if (objectList == null)
                return;

            foreach (var fileInfo in objectList
                .Where(path => !string.IsNullOrEmpty(path))
                .Select(path => new FileInfo(path))
                .Where(fileInfo => fileInfo.Exists)
                .Where(fileInfo => fileInfo.Extension.ToLower() == ".sdltm"))
            {
                AddTmFileToGrid(fileInfo);
            }
        }


        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        

        private void toolStripButtonAbout_Click(object sender, EventArgs e)
        {
            LoadAboutInfo();
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LoadAboutInfo();
        }
        #endregion
    }
}