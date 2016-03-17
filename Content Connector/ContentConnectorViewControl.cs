using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using BrightIdeasSoftware;
using Sdl.ProjectAutomation.Core;
using Sdl.ProjectAutomation.FileBased;

namespace Sdl.Community.ContentConnector
{
    public partial class ContentConnectorViewControl : UserControl
    {
        ContentConnectorViewController _controller;
        private readonly Persistence _persistence;
        private  List<ProjectRequest> _folderPathList;
        private readonly List<ProjectRequest> _selectedFolders; 

        public ContentConnectorViewControl()
        {
            InitializeComponent();

            _persistence = new Persistence();
            _projectsListBox.SelectedIndexChanged += new EventHandler(_projectsListBox_SelectedIndexChanged);
            _folderPathList = new List<ProjectRequest>();
            _selectedFolders = new List<ProjectRequest>();
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

            templateColumn.AspectGetter = delegate(object rowObject)
            {
                var folderObject = (ProjectRequest) rowObject;

                return folderObject.ProjectTemplate;
            };
            InitializeListView(_folderPathList);


            //allows user to select multiple projects in listbox
            _projectsListBox.SelectionMode = SelectionMode.MultiSimple;

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
                if (_folderPathList.Count == 0)
                {
                    _folderPathList = _persistence.Load();
                }

                var item = _folderPathList.FirstOrDefault(p => p.Path == ((ProjectRequest)e.RowObject).Path);

                if (item != null) item.ProjectTemplate = (ProjectTemplateInfo) value;
                _persistence.Save(_folderPathList);
                InitializeListView(_folderPathList);

            }
        }


        /// <summary>
        /// A dropdown will appear when user click on a template cell
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FoldersListView_CellEditStarting(object sender, BrightIdeasSoftware.CellEditEventArgs e)
        {
            if (e.Column == deleteColumn)
            {
                e.Cancel = true;
                foldersListView.RemoveObject(e.RowObject);

                var folderObject = e.RowObject as ProjectRequest;
                try
                {
                    var pathToRemove = _folderPathList.First(p => p.Path == folderObject.Path);

                    _folderPathList.Remove(pathToRemove);
                }
                catch(Exception ex) { }
               
            }

            if (e.Column != templateColumn) return;
            var cb = new ComboBox
            {
                Bounds = e.CellBounds,
                Font = ((ObjectListView) sender).Font,
                DropDownStyle = ComboBoxStyle.DropDownList
            };

            //displays only cumstom templates
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


        private void InitializeListView(List<ProjectRequest> filePathList)
        {
            filePathList = _persistence.Load();

            foldersListView.SetObjects(filePathList);
        }

        internal ContentConnectorViewController Controller
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

                //if (_projectsListBox.Items.Count > 0)
                //{
                //    _projectsListBox.SelectedIndex = 0;
                //}
            }

            LoadFileList();
        }

        private void LoadFileList()
        {
            _filesListView.Items.Clear();

            ProjectRequest selectedProject = _projectsListBox.SelectedItem as ProjectRequest;
            if (selectedProject != null)
            {
                foreach (string file in selectedProject.Files)
                {
                    string fileName = Path.GetFileName(file);
                    _filesListView.Items.Add(fileName);
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
            var folderDialog = new FolderBrowseDialogExtended
            {
               Description = "Select folders",
               ShowEditBox = true,
               ShowFullPathInEditBox = true
            };
           
            var result = folderDialog.ShowDialog();
                
                if (result == DialogResult.OK)
                {

                    var folderPath = folderDialog.SelectedPath;

                    foreach (var directory in Directory.GetDirectories(folderPath))
                    {
                        var directoryInfo = new DirectoryInfo(directory);

                        if (_folderPathList != null)
                        {
                        _folderPathList.Add(new ProjectRequest
                        {
                            Name = directoryInfo.Name,
                            Path = folderPath,
                            Files = Directory.GetFiles(directory, "*", SearchOption.AllDirectories)
                        });
                    }
                       
                    }

                    foldersListView.SetObjects(_folderPathList);
                }
            

        }

        private void saveBtn_Click(object sender, EventArgs e)
        {
            _persistence.Save(_folderPathList);
            InitializeListView(_folderPathList);

            ContentConnector.Refresh();
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
