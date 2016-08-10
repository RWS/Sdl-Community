using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using BrightIdeasSoftware;
using Sdl.Community.InSource.Helpers;
using Sdl.Community.InSource.Insights;
using Sdl.Community.InSource.Models;
using Sdl.ProjectAutomation.Core;
using Sdl.ProjectAutomation.FileBased;
using Timer = System.Timers.Timer;

namespace Sdl.Community.InSource
{
    public partial class InSourceViewControl : UserControl
    {
        InSourceViewController _controller;
        private readonly Persistence _persistence;
        private  List<ProjectRequest> _folderPathList;
        private readonly List<ProjectRequest> _selectedFolders;
        private readonly List<ProjectRequest> _watchFolders;
     
        private readonly Timer _timer;
        private int _timeLeft;
        private readonly TimerModel _timerModel;


        public InSourceViewControl()
        {
            InitializeComponent();

            _persistence = new Persistence();
            _projectsListBox.SelectedIndexChanged += new EventHandler(_projectsListBox_SelectedIndexChanged);
            _folderPathList = new List<ProjectRequest>();
            _selectedFolders = new List<ProjectRequest>();
            _watchFolders = new List<ProjectRequest>();
            _timerModel = _persistence.LoadTimerSettings();

        }



        protected override void OnLoad(EventArgs e)
        {
            foldersListView.ShowGroups = false;

            pathColumn.AspectGetter = delegate(object rowObject)
            {
                var folderObject = (ProjectRequest) rowObject;

                return folderObject.Path;
            };

            deleteColumn.AspectGetter = delegate {
                                                     return string.Empty;
            };

            var imageList = new ImageList();
            var image = Properties.Resources.delete;
            imageList.Images.Add(image);

            foldersListView.SmallImageList = imageList;
            deleteColumn.ImageGetter = delegate {
                                                    return 0;
            };
            foldersListView.CellEditActivation = ObjectListView.CellEditActivateMode.SingleClick;
            foldersListView.CellEditStarting += FoldersListView_CellEditStarting;
            foldersListView.CellEditFinishing += FoldersListView_CellEditFinishing;
            foldersListView.CellToolTipShowing += FoldersListView_CellToolTipShowing;

            templateColumn.AspectGetter = delegate(object rowObject)
            {
                var folderObject = (ProjectRequest) rowObject;

                return folderObject.ProjectTemplate;
            };
            InitializeListView(_watchFolders);


            //allows user to select multiple projects in listbox
  

        }


        private void FoldersListView_CellToolTipShowing(object sender, ToolTipShowingEventArgs e)
        {
           
            if (((ObjectListView) sender).HotColumnIndex == 1)
            {
                e.Text = "Deletes a watch folder from list. Please save the changes.";
            }
            if (((ObjectListView) sender).HotColumnIndex == 2)
            {
                e.Text = " Click to select a custom template, and save the changes.";
            }

        }



        /// <summary>
        /// Displays in cell template name selected by user, and save it in json file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FoldersListView_CellEditFinishing(object sender, CellEditEventArgs e)
        {
            if (e.Control is ComboBox)
            {
                var value = ((ComboBox) e.Control).SelectedItem;
                if (e.Column == templateColumn)
                {
                    ((ProjectRequest) e.RowObject).ProjectTemplate = (ProjectTemplateInfo) value;
                    foldersListView.RefreshObject((ProjectRequest) e.RowObject);

                }
               
                _folderPathList = _persistence.Load();
   
                var items = _folderPathList.FindAll(p => p.Path == ((ProjectRequest)e.RowObject).Path);

                foreach (var item in items)
                {
                     item.ProjectTemplate = (ProjectTemplateInfo)value;
                }
                

                Save();
                InitializeListView(_watchFolders);

            }
        }


        /// <summary>
        /// A dropdown will appear when user click on a template cell
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FoldersListView_CellEditStarting(object sender, CellEditEventArgs e)
        {
            try
            {
                if (e.Column == deleteColumn)
                {
                    e.Cancel = true;
                    var confirmDelete = MessageBox.Show(@"Are you sure you want to remove this watch folder path?",
                        @"Confirm",
                        MessageBoxButtons.OKCancel,
                        MessageBoxIcon.Question);

                    if (confirmDelete == DialogResult.OK)
                    {
                        foldersListView.RemoveObject(e.RowObject);

                        var folderObject = e.RowObject as ProjectRequest;
                        

                            var requestToRemove = _folderPathList.FindAll(p => p.Path == folderObject.Path);
                            foreach (var request in requestToRemove)
                            {
                                _folderPathList.Remove(request);
                            }

                            var watchFolderToRemove = _watchFolders.FirstOrDefault(w => w.Path == folderObject.Path);
                            if (watchFolderToRemove != null)
                            {
                                _watchFolders.Remove(watchFolderToRemove);
                            }
                   
                    
                        _persistence.SaveProjectRequestList(_folderPathList);
                        LoadProjectRequests();

                    }

                }

                if (e.Column != templateColumn) return;
                var cb = new ComboBox
                {
                    Bounds = e.CellBounds,
                    Font = ((ObjectListView) sender).Font,
                    DropDownStyle = ComboBoxStyle.DropDownList
                };

                //displays only cumstom templates
                if (_controller.ProjectTemplates.Count() != 0)
                {
                    foreach (ProjectTemplateInfo projectTemplate in _controller.ProjectTemplates)
                    {
                        if (projectTemplate.Name != "Default" && projectTemplate.Name != "SDL Trados")
                        {
                            cb.Items.Add(projectTemplate);
                        }

                    }
                    cb.SelectedIndex = 0;
                    e.Control = cb;

                }
                else
                {
                    MessageBox.Show(@"Please create a custom project template!", @"Warning", MessageBoxButtons.OK);
                    e.Cancel = true;

                }

            }
            catch (Exception exception)
            {
                TelemetryService.Instance.AddException(exception);
            }
        }


        private void InitializeListView(List<ProjectRequest> watchFolders)
        {
            watchFolders = _persistence.Load();
            if (watchFolders != null)
            {
               // var distinct = watchFolders.Select(x => x.Path).Distinct();
                var distinct = watchFolders.GroupBy(x => x.Path).Select(y => y.First());
                foldersListView.SetObjects(distinct);
            }
            
        }

        internal InSourceViewController Controller
        {
            get { return _controller; }
            set
            {
                _controller = value;

                if (_controller != null)
                {
                    _controller.ProjectRequestsChanged += new EventHandler(_controller_ProjectRequestsChanged);
                }

                _progressBar.DataBindings.Add("Value", _controller, "PercentComplete");

                LoadProjectRequests();
            }
        }

        public void ClearMessages()
        {
            _resultsTextBox.Text = "";
        }

        public void ReportMessage(FileBasedProject fileBasedProject, string message)
        {

            _resultsTextBox.AppendText("\r\n" + message);
        }


        private void LoadProjectRequests()
        {
            _projectsListBox.Items.Clear();

            if (_controller.ProjectRequests != null)
            {
                foreach (ProjectRequest projectRequest in _controller.ProjectRequests)
                {

                    _projectsListBox.Items.Add(projectRequest);
                }
            }

            LoadFileList();
        }

        private void LoadFileList()
        {
            _filesListView.Items.Clear();

            ProjectRequest selectedProject = _projectsListBox.SelectedItem as ProjectRequest;
            if (selectedProject != null)
            {
                if (selectedProject.Files != null)
                {
                    foreach (string file in selectedProject.Files)
                    {
                        string fileName = Path.GetFileName(file);
                        _filesListView.Items.Add(fileName);
                    }
                }
                
            }
        }

        void _controller_ProjectRequestsChanged(object sender, EventArgs e)
        {
            LoadProjectRequests();
        }

        void _projectsListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadFileList();
        }


        private void addBtn_Click(object sender, EventArgs e)
        {

            if (_persistence.Load() != null)
            {
                _folderPathList = _persistence.Load();
            }

            //extended folder browse dialog for adding a text box where you can paste the path
            //var folderDialog = new FolderBrowseDialogExtended
            //{
            //    Description = "Select folders",
            //    ShowEditBox = true,
            //    ShowFullPathInEditBox = true
            //};
            var folderDialog = new FolderSelectDialog();
            

            if (folderDialog.ShowDialog())
            {

                var folderPath = folderDialog.FileName;
                var watchFolder = new ProjectRequest
                {
                    Path = folderPath
                };
                _watchFolders.Add(watchFolder);
                var folders = Directory.GetDirectories(folderPath);

                if (folders.Length != 0)
                {
                    foreach (var directory in folders)
                    {

                        var directoryInfo = new DirectoryInfo(directory);

                        if (_folderPathList != null)
                        {
                            if (directoryInfo.Name != "AcceptedRequests")
                            {
                                var selectedFolder = new ProjectRequest
                                {
                                    Name = directoryInfo.Name,
                                    Path = folderPath,
                                    Files = Directory.GetFiles(directory, "*", SearchOption.AllDirectories),
                                };
                                if (!_folderPathList.Contains(selectedFolder))
                                {
                                    _folderPathList.Add(selectedFolder);
                                }
                            }

                        }
                    }
                }
                else
                {
                    SetWatchFolder(folderPath);
                }
                foldersListView.SetObjects(_watchFolders);
                Save();
            }

        }

        private void SetWatchFolder(string path)
        {
            var directoryInfo = new DirectoryInfo(path);

            if (_folderPathList != null)
            {
                if (directoryInfo.Name != "AcceptedRequests")
                {
                    var selectedFolder = new ProjectRequest
                    {
                        Name = directoryInfo.Name,
                        Path = path,
                        Files = Directory.GetFiles(path, "*", SearchOption.AllDirectories)
                    };
                    if (!_folderPathList.Contains(selectedFolder))
                    {
                        _folderPathList.Add(selectedFolder);
                    }
                }
            }
        }

        private void Save()
        {
            _persistence.SaveProjectRequestList(_folderPathList);
            InitializeListView(_watchFolders);

            InSource.Refresh();
            _controller.ProjectRequests = _folderPathList;

            LoadProjectRequests();
        }
        private void saveBtn_Click(object sender, EventArgs e)
        {
            _persistence.SaveProjectRequestList(_folderPathList);
            InitializeListView(_watchFolders);

            InSource.Refresh();
            _controller.ProjectRequests = _folderPathList;
            
            LoadProjectRequests();
        }

        private void _projectsListBox_Click(object sender, EventArgs e)
        {
            _selectedFolders.Clear();
            var selectedItems = _projectsListBox.SelectedItems;

            foreach (var item in selectedItems)
            {
                if (!_selectedFolders.Contains((ProjectRequest)item))
                {
                    _selectedFolders.Add((ProjectRequest)item);
                }
            }
            
            _controller.SelectedProjects = _selectedFolders;

        }
    }
}
