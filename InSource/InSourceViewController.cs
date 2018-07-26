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
using Sdl.Desktop.IntegrationApi.Interfaces;
using Sdl.Desktop.IntegrationApi.Notifications;
using Sdl.Desktop.IntegrationApi.Notifications.Events;
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
       
        private static readonly Lazy<InSourceViewControl> Control = new Lazy<InSourceViewControl>(() => new InSourceViewControl());
        private static readonly Lazy<TimerControl> TimerControl = new Lazy<TimerControl>();

        private ProjectTemplateInfo _selectedProjectTemplate;
        private List<ProjectRequest> _projectRequests;
        private List<ProjectRequest> _selectedProjects;
        private readonly List<bool> _hasTemplateList;
        private readonly List<bool> _hasFiles; 
        public static Persistence Persistence = new Persistence();
        private int _percentComplete;
	    private IStudioNotificationCommand _createProjectCommand;
	    private IStudioEventAggregator _eventAggregator;
		private const string NotificationGroupId = "b0261aa3-b6a5-4f69-8f94-3713784ce8ef";
		private List<string> _foldersRequestPath;
	    private InSourceNotificationGroup _notificationGroup;
		#endregion private fields

		public event EventHandler ProjectRequestsChanged;

	    public InSourceViewController()
	    {
		    _projectRequests = new List<ProjectRequest>();
		    _hasTemplateList = new List<bool>();
		    _hasFiles = new List<bool>();
		    _eventAggregator = SdlTradosStudio.Application.GetService<IStudioEventAggregator>();

		 //   Action action = CreateProjectFromNotification;

			//_createProjectCommand = new InSourceCommand(action)
			//{
			//	CommandText = "Create project"
			//};

			_notificationGroup = new InSourceNotificationGroup(NotificationGroupId)
		    {
			    Title = "InSource Notifications"
		    };


		 //   _notificationGroup.Notifications.Add(new InSourceNotification(new Guid())
		 //   {
			//    Title = "Second notification title",
			//    AlwaysVisibleDetails = new List<string>
			//    {
			//	    "Third notification  description",
			//	    "Forth notification description"
			//    },
			//	Action = _createProjectCommand,
			//	IsActionVisible = true,
			//	LinkAction = _createProjectCommand,
			//	IsLinkVisible = true

			//});

		 //   // add group notification
		 //   var addTestGroup = new AddStudioGroupNotificationEvent(_notificationGroup);
		 //   _eventAggregator.Publish(addTestGroup);

		 //   var showNotification = new ShowStudioNotificationsViewEvent(showNotifications: true, setFocus: true);
		 //   _eventAggregator.Publish(showNotification);
		    _foldersRequestPath = new List<string>();
	    }

	    protected override void Initialize(IViewContext context)
        {
            ProjectsController = SdlTradosStudio.Application.GetController<ProjectsController>();
            Control.Value.Controller = this;
            TimerControl.Value.CheckForProjectsRequestEvent += CheckForProjectsEvent;
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

        public IEnumerable<ProjectTemplateInfo> ProjectTemplates => ProjectsController.GetProjectTemplates();

	    public ProjectTemplateInfo SelectedProjectTemplate
        {
            get => _selectedProjectTemplate;
		    set
            {
                _selectedProjectTemplate = value;
                OnPropertyChanged("SelectedProjectTemplate");
            }
        }

        public List<ProjectRequest> ProjectRequests
        {
            get => _projectRequests;
	        set
            {
                _projectRequests = value;
                OnPropertyChanged("ProjectRequests");

                OnProjectRequestsChanged();
            }
        }
        public List<ProjectRequest> SelectedProjects
        {
            get => _selectedProjects;
	        set
            {
                _selectedProjects = value;
                OnPropertyChanged("SelectedProjects");
            
            }
        }

        protected override Control GetExplorerBarControl()
        {
            return TimerControl.Value;
        }

        protected override Control GetContentControl()
        {
            return Control.Value;
        }

        public int PercentComplete 
        {
            get => _percentComplete;
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

			    foreach (var newProjectRequest in ProjectRequests)
			    {
				    var newProjectPath = Path.Combine(newProjectRequest.Path, newProjectRequest.Name);

					var notification = new  InSourceNotification(newProjectRequest.NotificationId)
				    {
					    Title = newProjectRequest.Name,
					    AlwaysVisibleDetails = new List<string>
					    {
						    "Project request path",
						    newProjectPath
					    },
					    IsActionVisible = true
				    };
				   
				    Action action = ()=>CreateProjectFromNotification(notification);
				    _createProjectCommand = new InSourceCommand(action)
				    {
					    CommandText = "Create project"
				    };
				    notification.Action = _createProjectCommand;
					_notificationGroup.Notifications.Add(notification);
					
				}
			    var addTestGroup = new AddStudioGroupNotificationEvent(_notificationGroup);
			    _eventAggregator.Publish(addTestGroup);

			    var showNotification = new ShowStudioNotificationsViewEvent(true,true);
			    _eventAggregator.Publish(showNotification);

				//   var notificationsList = new List<Notification>();

				//foreach (var newProjectRequest in ProjectRequests)
				//{
				//	//to avoid duplication of notifications we save the path of the project in a list
				//	//if the path is already there we don't create another notification
				//	//in this version of Notification api there is a issue: RemoveGroup() is not working
				//	var newProjectPath = Path.Combine(newProjectRequest.Path, newProjectRequest.Name);
				//	//if the new diewctory does not contain files don't create a notification
				//	if (Directory.GetFiles(newProjectPath).Any())
				//	{
				//		if (!_foldersRequestPath.Contains(newProjectPath))
				//		{
				//			var notification = new Notification
				//			{
				//				Title = newProjectRequest.Name,
				//				Details = new List<string> { "Project request path", newProjectPath }
				//			};
				//			newProjectRequest.NotificationId = notification.Id;
				//			_clearCommand = new RelayCommand<Notification>(n =>
				//			{
				//				CreateProjectFromNotification(notification);
				//			});
				//			notification.SetCommand(_clearCommand, "Create new project", "Tooltip");
				//			notificationsList.Add(notification);
				//			_foldersRequestPath.Add(newProjectPath);
				//		}
				//	}
				//}
				//_notificationGroup.Add(notificationsList);
				//_notificationGroup.Publish();
			}

	    }
	    private void CreateProjectFromNotification(InSourceNotification notification)
	    {
			//  var notification = notificationObject as Notification;
			if (notification != null)
			{
				var project = ProjectRequests.FirstOrDefault(n => n.NotificationId.Equals(notification.Id));
				//CreateProjectsFromNotifications(project);
				//_notificationGroup.Remove(notification.Id);
				_notificationGroup.Notifications.Remove(notification);
				
				var removeTestGroup = new RemoveStudioGroupNotificationEvent(NotificationGroupId);
				_eventAggregator.Publish(removeTestGroup);
				//_eventAggregator.Publish(_notificationGroup);

				_notificationGroup = new InSourceNotificationGroup(NotificationGroupId);
				var notification1 = new InSourceNotification(Guid.NewGuid())
				{
					Title = "dd",
					AlwaysVisibleDetails = new List<string>
					{
						"Project request path",
						"aaa"
					},
					IsActionVisible = true
				};

				Action action = () => CreateProjectFromNotification(notification1);
				_createProjectCommand = new InSourceCommand(action)
				{
					CommandText = "Create project"
				};
				notification1.Action = _createProjectCommand;
				_notificationGroup.Notifications.Add(notification1);
				var addTestGroup = new AddStudioGroupNotificationEvent(_notificationGroup);
				_eventAggregator.Publish(addTestGroup);
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

	                if (dirInfo.Name !="AcceptedRequests")
	                {
						var projectRequest = CreateProjectRequest(templateForWatchFolder, dirInfo, watchFolderPath);
		                projectRequestList.Add(projectRequest);
					}

					//This will be uncommented after testing proper the application 
                    //var projectRequest=CreateProjectRequest(templateForWatchFolder, dirInfo, watchFolderPath);
                    ////that means we don't have anoter folder besides "Accepted request", and we need to set the curent directory as project request
                    //if (projectRequest.Name != null)
                    //{
                    //    projectRequestList.Add(projectRequest);
                    //}
                    //else
                    //{
                    //    var infoDir = new DirectoryInfo(watchFolderPath);
                    //    var request = CreateRequestFromCurrentDirectory(templateForWatchFolder, infoDir, watchFolderPath);
                    //    projectRequestList.Add(request);
                    //}

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
				projectRequest.NotificationId = Guid.NewGuid();
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
						//Remove the created project from project request
						//this will refresh the list from view part
					    ProjectRequests.Remove(projectFromNotifications);

					    OnProjectRequestsChanged();
						MessageBox.Show("Project "+ projectFromNotifications.Name +" was created");
					}
			    };
			    worker.RunWorkerAsync();
		}

        public void CreateProjects()
        {
            Control.Value.ClearMessages();

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
            Control.Value.BeginInvoke(new Action(() => Control.Value.ReportMessage(fileBasedProject, message)));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

        private void OnProjectRequestsChanged()
        {
			ProjectRequestsChanged?.Invoke(this, EventArgs.Empty);
		}
    }
}
