using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using BrightIdeasSoftware;
using NLog;
using Sdl.Community.InSource.Helpers;
using Sdl.Community.InSource.Interfaces;
using Sdl.Community.InSource.Notifications;
using Sdl.Community.InSource.Service;
using Sdl.Desktop.IntegrationApi.Interfaces;
using Sdl.ProjectAutomation.Core;

namespace Sdl.Community.InSource
{
	public partial class InSourceViewControl : UserControl, IUIControl
	{
		InSourceViewController _controller;
		private readonly Persistence _persistence;
		private List<ProjectRequest> _folderPathList;
		private readonly List<ProjectRequest> _selectedFolders;
		private readonly List<ProjectRequest> _watchFolders;
		private readonly Logger _logger = LogManager.GetCurrentClassLogger();
		private readonly IMessageBoxService _messageBoxService;

		public InSourceViewControl()
		{
			InitializeComponent();
			_persistence = new Persistence();
			_projectsListBox.SelectedIndexChanged += _projectsListBox_SelectedIndexChanged;
			_folderPathList = new List<ProjectRequest>();
			_selectedFolders = new List<ProjectRequest>();
			_watchFolders = new List<ProjectRequest>();
			_messageBoxService = new MessageBoxService();
		}

		public void ClearMessages()
		{
			_resultsTextBox.Text = string.Empty;
		}

		public void ReportMessage(string message)
		{
			_resultsTextBox.Text = $"{_resultsTextBox.Text} \r\n {message}";
		}		

		protected override void OnLoad(EventArgs e)
		{
			foldersListView.ShowGroups = false;

			pathColumn.AspectGetter = delegate (object rowObject)
			{
				var folderObject = (ProjectRequest)rowObject;

				return folderObject.Path;
			};

			deleteColumn.AspectGetter = delegate
			{
				return string.Empty;
			};

			SetImages();

			deleteColumn.ImageGetter = delegate
			{
				return 0;
			};

			SetFolderListViewEvents();

			templateColumn.AspectGetter = delegate (object rowObject)
			{
				var folderObject = (ProjectRequest)rowObject;
				return folderObject.ProjectTemplate;
			};
			InitializeListView(_watchFolders);
		}

		private void FoldersListView_CellToolTipShowing(object sender, ToolTipShowingEventArgs e)
		{
			if (((ObjectListView)sender).HotColumnIndex == 1)
			{
				e.Text = "Deletes a watch folder from list. Please save the changes.";
			}
			if (((ObjectListView)sender).HotColumnIndex == 2)
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
				var value = ((ComboBox)e.Control).SelectedItem;
				if (e.Column == templateColumn)
				{
					((ProjectRequest)e.RowObject).ProjectTemplate = (ProjectTemplateInfo)value;
					foldersListView.RefreshObject((ProjectRequest)e.RowObject);
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
					
					var confirmDelete = _messageBoxService.AskForConfirmation(PluginResources.RemoveWatchFolder_Message);
					if (confirmDelete)
					{						
						RemoveWatchFolders(e);
					}
				}

				if (e.Column != templateColumn) return;
				var cb = new ComboBox
				{
					Bounds = e.CellBounds,
					Font = ((ObjectListView)sender).Font,
					DropDownStyle = ComboBoxStyle.DropDownList
				};

				DisplayCustomTemplates(cb, e);
			}
			catch (Exception exception)
			{
				_logger.Error($"FoldersListView_CellEditStarting: {exception.Message}\n {exception.StackTrace}");
			}
		}

		// Displays only the cumstom templates (except 'Default' and 'SDL Trados' templates which are not custom)
		private void DisplayCustomTemplates(ComboBox cb, CellEditEventArgs e)
		{
			if (_controller?.ProjectTemplates == null || _controller?.ProjectTemplates.Count() <= 0)
			{
				_messageBoxService.ShowWarningMessage(PluginResources.ImportCustomTemplate_Message, string.Empty);
				e.Cancel = true;
			}
			else
			{
				foreach (var projectTemplate in _controller.ProjectTemplates)
				{
					if (!projectTemplate.Name.Equals("Default") && !projectTemplate.Name.Equals("SDL Trados"))
					{
						cb.Items.Add(projectTemplate);
					}
				}
				if (cb.Items.Count > 0)
				{
					cb.SelectedIndex = 0;
				}
				else
				{
					_messageBoxService.ShowWarningMessage(PluginResources.ImportCustomTemplate_Message, string.Empty);
					e.Cancel = true;
				}
				e.Control = cb;
			}
		}

		private void RemoveWatchFolders(CellEditEventArgs e)
		{
			foldersListView.RemoveObject(e.RowObject);

			var folderObject = e.RowObject as ProjectRequest;

			var requestToRemove = _folderPathList?.FindAll(p => p.Path == folderObject.Path);

			if (requestToRemove != null)
			{
				foreach (var request in requestToRemove)
				{
					_folderPathList.Remove(request);
					var notification = GetUINotification(request);
					_controller.ClearNotification(notification);
				}
			}

			var watchFolderToRemove = _watchFolders?.FirstOrDefault(w => w.Path == folderObject.Path);
			if (watchFolderToRemove != null)
			{
				_watchFolders.Remove(watchFolderToRemove);
			}

			_persistence.SaveProjectRequestList(_folderPathList);
			LoadProjectRequests();
		}

		private void InitializeListView(List<ProjectRequest> watchFolders)
		{
			watchFolders = _persistence.Load();
			if (watchFolders != null)
			{
				var distinct = watchFolders.GroupBy(x => x.Path).Select(y => y.First());
				foldersListView.SetObjects(distinct);
			}
		}

		internal InSourceViewController Controller
		{
			get => _controller;
			set
			{
				_controller = value;

				if (_controller != null)
				{
					_controller.ProjectRequestsChanged += _controller_ProjectRequestsChanged;
				}
				_progressBar.DataBindings.Add("Value", _controller, "PercentComplete");

				LoadProjectRequests();
			}
		}		

		private void LoadProjectRequests()
		{
			_projectsListBox.Items.Clear();

			if (_controller.ProjectRequests != null)
			{
				foreach (var projectRequest in _controller.ProjectRequests)
				{
					if (!projectRequest.Path.Contains(projectRequest.Name))
					{
						_projectsListBox.Items.Add(projectRequest);
					}
				}
			}
			LoadFileList();
		}

		private InSourceNotification GetUINotification(ProjectRequest projectRequest)
		{
			return new InSourceNotification(projectRequest.NotificationId)
			{
				Title = projectRequest.Name,
				AlwaysVisibleDetails = new List<string>
				{
					"Project request path",
					projectRequest.Path
				},
				IsActionVisible = true,
				AllowsUserToDismiss = true
			};
		}

		private void LoadFileList()
		{
			_filesListView.Items.Clear();

			var selectedProject = _projectsListBox.SelectedItem as ProjectRequest;
			if (selectedProject?.Files != null)
			{
				foreach (var file in selectedProject.Files)
				{
					var fileName = Path.GetFileName(file);
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
							else
							{
								SetWatchFolder(folderPath);
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

		private void SetFolderListViewEvents()
		{
			foldersListView.CellEditActivation = ObjectListView.CellEditActivateMode.SingleClick;
			foldersListView.CellEditStarting += FoldersListView_CellEditStarting;
			foldersListView.CellEditFinishing += FoldersListView_CellEditFinishing;
			foldersListView.CellToolTipShowing += FoldersListView_CellToolTipShowing;
		}

		private void SetImages()
		{
			var imageList = new ImageList();
			var image = PluginResources.trash_alt;
			imageList.Images.Add(image);

			foldersListView.SmallImageList = imageList;
		}

		private void btn_ClearMessages_Click(object sender, EventArgs e)
		{
			_resultsTextBox.Text = string.Empty;
			_progressBar.Value = 0;
		}
	}
}