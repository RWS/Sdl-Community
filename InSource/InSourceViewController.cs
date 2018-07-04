using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Input;
using Sdl.Community.InSource.Notifications;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.Desktop.IntegrationApi.Notifications;
using Sdl.ProjectAutomation.Core;
using Sdl.ProjectAutomation.FileBased;
using Sdl.TranslationStudioAutomation.IntegrationApi;
using Sdl.TranslationStudioAutomation.IntegrationApi.Presentation.DefaultLocations;

namespace Sdl.Community.InSource
{
    [View(
        Id = "InSourceView",
        Name = "InSource!",
        Description = "Create projects from project request content",
        Icon = "InSource_large",
        LocationByType = typeof(TranslationStudioDefaultViews.TradosStudioViewsLocation))]
    public class InSourceViewController : AbstractViewController, INotifyPropertyChanged
    {
        #region private fields
       
        private static readonly Lazy<InSourceViewControl> _control = new Lazy<InSourceViewControl>(() => new InSourceViewControl());
        private static readonly Lazy<TimerControl> _timerControl = new Lazy<TimerControl>();

        private ProjectTemplateInfo _selectedProjectTemplate;
        private List<ProjectRequest> _projectRequests;
        private List<ProjectRequest> _selectedProjects;
        private readonly List<bool> _hasTemplateList;
        private readonly List<bool> _hasFiles; 
        public static Persistence Persistence = new Persistence();
        private int _percentComplete;
	    private ICommand _clearCommand;
	    private readonly NotificationsGroup _notificationGroup;
		#endregion private fields

		public event EventHandler ProjectRequestsChanged;

        public InSourceViewController()
        {
            _projectRequests = new List<ProjectRequest>();
            _hasTemplateList = new List<bool>();
            _hasFiles = new List<bool>();
			_notificationGroup = new NotificationsGroup(Guid.NewGuid());
		}

        protected override void Initialize(IViewContext context)
        {
            ProjectsController = SdlTradosStudio.Application.GetController<ProjectsController>();
            _control.Value.Controller = this;
            _timerControl.Value.CheckForProjectsRequestEvent += CheckForProjectsEvent;
        }

        private void CheckForProjectsEvent(object sender, EventArgs e)
        {
            CheckForProjects();
        }

        private ProjectsController ProjectsController
        {
            get;
            set;
        }

        public IEnumerable<ProjectTemplateInfo> ProjectTemplates
        {
            get
            {
                return ProjectsController.GetProjectTemplates();
            }
        }

        public ProjectTemplateInfo SelectedProjectTemplate
        {
            get
            {
                return _selectedProjectTemplate;
            }
            set
            {
                _selectedProjectTemplate = value;
                OnPropertyChanged("SelectedProjectTemplate");
            }
        }

        public List<ProjectRequest> ProjectRequests
        {
            get
            {
                return _projectRequests;
            }
            set
            {
                _projectRequests = value;
                OnPropertyChanged("ProjectRequests");

                OnProjectRequestsChanged();
            }
        }
        public List<ProjectRequest> SelectedProjects
        {
            get
            {
                return _selectedProjects;
            }
            set
            {
                _selectedProjects = value;
                OnPropertyChanged("SelectedProjects");
            
            }
        }

        protected override Control GetExplorerBarControl()
        {
            return _timerControl.Value;
        }

        protected override Control GetContentControl()
        {
            return _control.Value;
        }

        public int PercentComplete 
        {
            get
            {
                return _percentComplete;
            }
            set
            {
                _percentComplete = value;
                OnPropertyChanged("PercentComplete");
            }
        }

        public List<FileBasedProject> Projects
        {
            get;
            set;
        }

	    public void CheckForProjects()
	    {
			var projectRequest = Persistence.Load();
			_notificationGroup.RemoveGroup();
		    var newProjectRequestList = new List<ProjectRequest>();
		    if (projectRequest != null)
		    {
			    var watchFoldersList = GetWatchFolders(projectRequest);
			    foreach (var warchFolder in watchFoldersList)
			    {
				    newProjectRequestList.AddRange(GetNewDirectories(warchFolder, projectRequest));
				}
			    Persistence.SaveProjectRequestList(newProjectRequestList);
			    ProjectRequests = newProjectRequestList;
			    var notificationsList = new List<Notification>();

				foreach (var newProjectRequest in ProjectRequests)
			    {
					var notification = new Notification
					{
						Title = newProjectRequest.Name,	
						Details = new List<string> { "Project request path",Path.Combine(newProjectRequest.Path,newProjectRequest.Name)}
					};
				    newProjectRequest.NotificationId = notification.Id;
					_clearCommand = new RelayCommand<Notification>(n =>
				    {
					    CreateProjectFromNotification(notification);
				    });
				    notification.SetCommand(_clearCommand, "Create new project", "Tooltip");
					notificationsList.Add(notification);
				}
				_notificationGroup.Add(notificationsList);
				_notificationGroup.Publish();
		    }

	    }
	    private void CreateProjectFromNotification(object notificationObject)
	    {
		    var notification = notificationObject as Notification;
		    if (notification != null)
		    {
			    var project = ProjectRequests.FirstOrDefault(n => n.NotificationId.Equals(notification.Id));
			    CreateProjectsFromNotifications(project);
				_notificationGroup.Remove(notification.Id);
		    }
	    }

		private List<string> GetWatchFolders(List<ProjectRequest> projectRequest)
        {
            var watchFoldersPath = projectRequest.GroupBy(x => x.Path).Select(y => y.First());;
            var foldersPath = new List<string>();
            foreach (var watch in watchFoldersPath)
            {
                foldersPath.Add(watch.Path);
            }

            return foldersPath;
        }

        private List<ProjectRequest> GetNewDirectories(string watchFolderPath,List<ProjectRequest> projectRequests )
        {
            //get the template for watch folder
            var templateForWatchFolder =
                projectRequests.Where(s => s.Path == watchFolderPath).Select(t => t.ProjectTemplate).FirstOrDefault();
           
            var projectRequestList = new List<ProjectRequest>();
            var subdirectories = Directory.GetDirectories(watchFolderPath);
            if (subdirectories.Length != 0)
            {
                foreach (var subdirectory in subdirectories)
                {
                    var dirInfo = new DirectoryInfo(subdirectory);
                 
                    var projectRequest=CreateProjectRequest(templateForWatchFolder, dirInfo, watchFolderPath);
                    //that means we don't have anoter folder besides "Accepted request", and we need to set the curent directory as project request
                    if (projectRequest.Name != null)
                    {
                        projectRequestList.Add(projectRequest);
                    }
                    else
                    {
                        var infoDir = new DirectoryInfo(watchFolderPath);
                        var request = CreateRequestFromCurrentDirectory(templateForWatchFolder, infoDir, watchFolderPath);
                        projectRequestList.Add(request);
                    }

                }
            }
            else
            {
                var dirInfo = new DirectoryInfo(watchFolderPath);
           
               var projectRequest= CreateProjectRequest(templateForWatchFolder, dirInfo, watchFolderPath);
                projectRequestList.Add(projectRequest);
            }
           

            return projectRequestList;
            
        }

        private ProjectRequest CreateRequestFromCurrentDirectory(ProjectTemplateInfo templateInfo,
            DirectoryInfo directory, string path)
        {
            var projectRequest = new ProjectRequest();
            if (directory.Name != "AcceptedRequests")
            {

                projectRequest.Name = directory.Name;
                projectRequest.Path = path;
                projectRequest.ProjectTemplate = templateInfo;
                projectRequest.Files = Directory.GetFiles(directory.FullName, "*", SearchOption.TopDirectoryOnly);
            }
            return projectRequest;
        }

        private ProjectRequest CreateProjectRequest(ProjectTemplateInfo templateInfo, DirectoryInfo directory,
            string path)
        {
            var projectRequest = new ProjectRequest();
            if (directory.Name != "AcceptedRequests")
            {

                projectRequest.Name = directory.Name;
                projectRequest.Path = path;
                projectRequest.ProjectTemplate = templateInfo;
                projectRequest.Files = Directory.GetFiles(directory.FullName, "*", SearchOption.AllDirectories);
            }
            return projectRequest;
        }

	    public void CreateProjectsFromNotifications(ProjectRequest projectFromNotifications)
	    {
			ProjectCreator creator;
			var worker = new BackgroundWorker
			{
				WorkerReportsProgress = true
			};
		    worker.DoWork += (sender, e) =>
		    {
			    creator = new ProjectCreator(projectFromNotifications, projectFromNotifications.ProjectTemplate);
			    creator.Execute();
		    };

			    worker.RunWorkerCompleted += (sender, e) =>
			    {

				    if (e.Error != null)
				    {
					    MessageBox.Show(e.Error.ToString());
				    }
				    else
				    {
					    InSource.Instance.RequestAccepted(projectFromNotifications);
						MessageBox.Show("Project "+ projectFromNotifications.Name +" was created");
					}
			    };
			    worker.RunWorkerAsync();
		}

        public void CreateProjects()
        {
            _control.Value.ClearMessages();

            ProjectCreator creator = null;
            BackgroundWorker worker = new BackgroundWorker();
            worker.WorkerReportsProgress = true;

            if (SelectedProjects == null || SelectedProjects.Count == 0)
            {
                MessageBox.Show(@"Please select a project");
            }
            else
            {
                if (SelectedProjects != null && SelectedProjects.Count != 0)
                {
                    foreach (var selectedProject in SelectedProjects)
                    {
                        if (selectedProject.ProjectTemplate != null)
                        {
                            _hasTemplateList.Add(true);
                        }
                        else
                        {
                            _hasTemplateList.Add(false);
                        }

                        if (HasFiles(selectedProject.Path))
                        {
                            _hasFiles.Add(true);
                        }
                        else
                        {
                            _hasFiles.Add(false);
                        }
                    }
                }
                if (!_hasFiles.Contains(true))
                {
                    if (!_hasTemplateList.Contains(false))
                    {
                        if (SelectedProjects != null && (SelectedProjects.Count != 0 && SelectedProjects != null))
                        {

                            worker.DoWork += (sender, e) =>
                            {
                                if (SelectedProjects.Count != 0 && SelectedProjects != null)
                                {
                                    creator = new ProjectCreator(SelectedProjects, SelectedProjectTemplate);
                                }
                                else
                                {
                                    creator = new ProjectCreator(ProjectRequests, SelectedProjectTemplate);
                                }

                                creator.ProgressChanged +=
                                    (sender2, e2) => { worker.ReportProgress(e2.ProgressPercentage); };
                                creator.MessageReported += (sender2, e2) => { ReportMessage(e2.Project, e2.Message); };
                                creator.Execute();
                            };
                            worker.ProgressChanged += (sender, e) =>
                            {
                                PercentComplete = e.ProgressPercentage;
                            };
                            worker.RunWorkerCompleted += (sender, e) =>
                            {

                                if (e.Error != null)
                                {
                                    MessageBox.Show(e.Error.ToString());
                                }
                                else
                                {
	                                foreach (
		                                Tuple<ProjectRequest, FileBasedProject> request in creator.SuccessfulRequests)
	                                {
		                                // accept the request
		                                InSource.Instance.RequestAccepted(request.Item1);

		                                // remove the request from the list of requests
		                                ProjectRequests.Remove(request.Item1);

		                                OnProjectRequestsChanged();
	                                }
                                }
                            };
                            worker.RunWorkerAsync();

                        }
                    }
                    else
                    {
                        MessageBox.Show(@"Please choose a custom template");
                        _hasTemplateList.Clear();
                    }

                }
                else
                {
                    MessageBox.Show(
                        @"Watch folders should contain only folders, please put the files in a directory, and after that click CHECK PROJECT REQUESTS BUTTON ");
                    _hasFiles.Clear();
                }
            }
        }

        private bool HasFiles(string path)
        {
            var hasFiles = Directory.EnumerateFiles(path).Any();
            return hasFiles;
        }

        public void Contribute()
        {
            System.Diagnostics.Process.Start("https://github.com/sdl/Sdl-Community/tree/master/InSource");
        }
        private void ReportMessage(FileBasedProject fileBasedProject, string message)
        {
            _control.Value.BeginInvoke(new Action(() => _control.Value.ReportMessage(fileBasedProject, message)));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private void OnProjectRequestsChanged()
        {
            if (ProjectRequestsChanged != null)
            {
                ProjectRequestsChanged(this, EventArgs.Empty);
            }
        }
    }
}
