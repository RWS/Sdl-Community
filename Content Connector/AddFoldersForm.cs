using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using BrightIdeasSoftware;

namespace Sdl.Community.ContentConnector
{
    public partial class AddFoldersForm : Form
    {
        private readonly List<Folder> _folderPathList;
        private readonly Persistence _persistence;
        private  List<ProjectRequest> _projectRequests;
        ContentConnectorViewController _controller;

        public AddFoldersForm()
        {
            InitializeComponent();
            _folderPathList = new List<Folder>();
            _persistence = new Persistence();
            _projectRequests = new List<ProjectRequest>();
            _controller = new ContentConnectorViewController();
        }

        protected override void OnLoad(EventArgs e)
        {
            foldersPathListView.ShowGroups = false;
            foldersPathListView.FullRowSelect = true;
          
            
            pathColumn.AspectGetter = delegate(object rowObject)
            {
                var folderObject = (Folder) rowObject;

                return folderObject.Path;
            };

            deleteColumn.AspectGetter = delegate {
                                                     return "Delete";
            };

            foldersPathListView.CellEditActivation = ObjectListView.CellEditActivateMode.SingleClick;
            foldersPathListView.CellEditStarting += ObjectListView1OnCellEditStarting;

            var imageList = new ImageList();
            var image = Properties.Resources.delete;
            imageList.Images.Add(image);

            foldersPathListView.SmallImageList = imageList;
            deleteColumn.ImageGetter = delegate {
                                                    return 0;
            };

            foldersPathListView.CellToolTipShowing += CellToolTipShowing;
        }

        private void CellToolTipShowing(object sender, ToolTipShowingEventArgs e)
        {
            if (((ObjectListView)sender).HotColumnIndex == 0)
            {
                var index = ((ObjectListView)sender).HotRowIndex;
                var path = _folderPathList[index];
                e.Text = path.Path;
            }
        }

        private void ObjectListView1OnCellEditStarting(object sender, CellEditEventArgs e)
        {
            if (e.Column == deleteColumn)
            {
                e.Cancel = true;
                foldersPathListView.RemoveObject(e.RowObject);

                var folderObject = e.RowObject as Folder;
                var pathToRemove = _folderPathList.First(p => p.Path == folderObject.Path);

                _folderPathList.Remove(pathToRemove);

            }
        }

        private void addBtn_Click(object sender, EventArgs e)
        {
            var folderDialog = new FolderBrowserDialog();
            var result = folderDialog.ShowDialog();

            if (result == DialogResult.OK)
            {

                var folderPath = folderDialog.SelectedPath;

                _folderPathList.Add(new Folder
                {
                    Path = folderPath
                });

                InitializeListView(_folderPathList);

            }

           
        }

        private void InitializeListView(List<Folder> filePathList)
        {
            foldersPathListView.SetObjects(filePathList);
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            _persistence.Save(_folderPathList);
            ContentConnector.Refresh();

            _controller.ProjectRequests = ContentConnector.ProjectRequests;
            _projectRequests = ContentConnector.ProjectRequests;


            Close();
        }
    }
}
