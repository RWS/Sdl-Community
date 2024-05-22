using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using NLog;
using Sdl.Community.InSource.Helpers;
using Sdl.Community.InSource.Interfaces;
using Sdl.Community.InSource.Notifications;
using Sdl.Community.InSource.Service;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.Desktop.IntegrationApi.Interfaces;
using Sdl.Desktop.IntegrationApi.Notifications.Events;
using Sdl.ProjectAutomation.Core;
using Sdl.ProjectAutomation.FileBased;
using Sdl.TranslationStudioAutomation.IntegrationApi;
using Sdl.TranslationStudioAutomation.IntegrationApi.Presentation.DefaultLocations;

namespace Sdl.Community.InSource
{
	[View(
		Id = "InSourceView",
		Name = "Trados InSource!",
		Description = "Create projects from project request content",
		Icon = "InSource_large",
		LocationByType = typeof(TranslationStudioDefaultViews.TradosStudioViewsLocation))]
	public class InSourceViewController : AbstractViewController, INotifyPropertyChanged, IDisposable
	{
		private static readonly Lazy<InSourceViewControl> Control = new Lazy<InSourceViewControl>(() => new InSourceViewControl());
		private static readonly Lazy<TimerControl> TimerControl = new Lazy<TimerControl>();
		private ProjectTemplateInfo _selectedProjectTemplate;
		private List<ProjectRequest> _projectRequests;
		private List<ProjectRequest> _selectedProjects;
		private readonly List<bool> _hasTemplateList;
		private readonly List<bool> _hasFiles;
		private int _percentComplete;
		private IStudioNotificationCommand _createProjectCommand;
		private readonly IStudioEventAggregator _eventAggregator;
		private readonly InSourceNotificationGroup _notificationGroup;
		private readonly string NotificationGroupId = "b0261aa3-b6a5-4f69-8f94-3713784ce8ef";
		private ProjectsController _projectsController;
		private IMessageBoxService _messageBoxService;
		private BackgroundWorker _worker;

		public static Persistence Persistence = new Persistence();
		public static readonly Logger _logger = LogManager.GetCurrentClassLogger();


		public event EventHandler ProjectRequestsChanged;
		public event PropertyChangedEventHandler PropertyChanged;

		public InSourceViewController()
		{
			_projectRequests = new List<ProjectRequest>();
			_messageBoxService = new MessageBoxService();
			_hasTemplateList = new List<bool>();
			_hasFiles = new List<bool>();
			_eventAggregator = SdlTradosStudio.Application.GetService<IStudioEventAggregator>();

			_notificationGroup = new InSourceNotificationGroup(NotificationGroupId)
			{
				Title = "InSource Notifications"
			};
		}

		public IEnumerable<ProjectTemplateInfo> ProjectTemplates => _projectsController.GetProjectTemplates();

		public ProjectTemplateInfo SelectedProjectTemplate
		{
			get => _selectedProjectTemplate;
			set
			{
				_selectedProjectTemplate = value;
				OnPropertyChanged(nameof(SelectedProjectTemplate));
			}
		}

		public List<ProjectRequest> ProjectRequests
		{
			get => _projectRequests;
			set
			{
				_projectRequests = value;
				OnPropertyChanged(nameof(ProjectRequests));

				OnProjectRequestsChanged();
			}
		}

		public List<ProjectRequest> SelectedProjects
		{
			get => _selectedProjects;
			set
			{
				_selectedProjects = value;
				OnPropertyChanged(nameof(SelectedProjects));
			}
		}

		public int PercentComplete
		{
			get => _percentComplete;
			set
			{
				_percentComplete = value;
				OnPropertyChanged(nameof(PercentComplete));
			}
		}

		public List<FileBasedProject> Projects { get; set; }

		protected override void Initialize(IViewContext context)
		{
			_projectsController = SdlTradosStudio.Application.GetController<ProjectsController>();
			Control.Value.Controller = this;
			TimerControl.Value.CheckForProjectsRequestEvent += CheckForProjectsEvent;
		}

		protected override IUIControl GetExplorerBarControl()
		{
			return TimerControl.Value;
		}

		protected override IUIControl GetContentControl()
		{
			return Control.Value;		
		}

		public override void Dispose()
		{
			TimerControl.Value.CheckForProjectsRequestEvent -= CheckForProjectsEvent;
		}

		public void CheckForProjects()
		{
			try
			{
				PublishNotifications();

				var projectRequest = Persistence.Load();
				SetWatchFoldersProjects(projectRequest);
			}
			catch (Exception e)
			{
				_logger.Error($"CheckForProjects method: {e.Message}\n {e.StackTrace}");
			}
		}

		public void CreateSudioNotification(ProjectRequest newProjectRequest, string newProjectPath)
		{
			var notification = new InSourceNotification(newProjectRequest.NotificationId)
			{
				Title = newProjectRequest.Name,
				AlwaysVisibleDetails = new List<string>
				{
					"Project request path",
					newProjectPath
				},
				IsActionVisible = true,
				AllowsUserToDismiss = true
			};

			CreateProjectNotificationCommand(notification);
			ClearNotificationAction(notification);

			_notificationGroup.Notifications.Add(notification);
		}

		public FileBasedProject CreateProjectsFromNotifications(ProjectRequest projectRequest, InSourceNotification notification)
		{
			var waitWindow = new WaitWindow();
			waitWindow.Show();
			FileBasedProject project = null;

			InitializeBackgroundWorker();
			_worker.DoWork += (sender, e) =>
			{
				var projectCreator = new ProjectCreator(projectRequest, projectRequest.ProjectTemplate, _messageBoxService);
				project = projectCreator.Execute();
			};

			_worker.RunWorkerCompleted += (sender, e) =>
			{
				if (e.Error != null)
				{
					_messageBoxService.ShowInformation(e.Error.ToString(), string.Empty);
				}
				else
				{
					if (project != null)
					{
						InSource.Instance.RequestAccepted(projectRequest);
						//Remove the created project from project request (this will refresh the list from view part)
						ProjectRequests.Remove(projectRequest);
						ClearNotification(notification);

						OnProjectRequestsChanged();
						waitWindow.Close();
						_messageBoxService.ShowInformation(string.Format(PluginResources.ProjectSuccessfullyCreated_Message, projectRequest.Name), string.Empty);
					}
					waitWindow.Close();
					_worker.Dispose();
				}
			};
			_worker.RunWorkerAsync();
			return project;
		}
		
		public void Contribute()
		{
			System.Diagnostics.Process.Start("https://github.com/sdl/Sdl-Community/tree/master/InSource");
		}

		public void CreateProjects()
		{
			try
			{
				Control.Value.ClearMessages();

				ProjectCreator creator = null;
				InitializeBackgroundWorker();

				if (SelectedProjects == null || SelectedProjects.Count == 0)
                {
                    _messageBoxService.ShowInformation(PluginResources.ProjectSelection_Message,
                        PluginResources.InSourceViewController_CreateProjects_Create_Projects);
                }
				else
				{
					if (SelectedProjects != null && SelectedProjects.Count != 0)
					{
						foreach (var selectedProject in SelectedProjects)
						{
							_hasTemplateList.Add(selectedProject.ProjectTemplate != null);

							_hasFiles.Add(HasFiles(selectedProject.Path));
						}
					}
					if (!_hasFiles.Contains(true))
					{
						if (!_hasTemplateList.Contains(false))
						{
							if (SelectedProjects != null && (SelectedProjects.Count != 0 && SelectedProjects != null))
							{
								_worker.DoWork += (sender, e) =>
								{
									creator = SelectedProjects.Count != 0 && SelectedProjects != null
										? new ProjectCreator(SelectedProjects, SelectedProjectTemplate, _messageBoxService)
										: new ProjectCreator(ProjectRequests, SelectedProjectTemplate, _messageBoxService);

									creator.ProgressChanged += (sender2, e2) => { _worker.ReportProgress(e2.ProgressPercentage); };
									creator.MessageReported += (sender2, e2) => { ReportMessage(e2.Message); };
									creator.Execute();
								};
								_worker.ProgressChanged += (sender, e) =>
								{
									PercentComplete = e.ProgressPercentage;
								};
								_worker.RunWorkerCompleted += (sender, e) =>
								{

									if (e.Error != null)
									{
										_messageBoxService.ShowInformation(e.Error.ToString(), PluginResources.InSourceViewController_CreateProjects_Create_Projects);
									}
									else if(creator != null)
									{
										foreach (var request in creator.SuccessfulRequests)
										{
											// accept the request
											InSource.Instance.RequestAccepted(request.Item1);

											// remove the request from the list of requests
											ProjectRequests.Remove(request.Item1);

											//remove notification for project created from the View part
											var notification =
												(InSourceNotification)_notificationGroup.Notifications.FirstOrDefault(n => n.Id.Equals(request.Item1.NotificationId));
											if (notification != null)
											{
												ClearNotification(notification);
											}
											OnProjectRequestsChanged();
										}
									}
									_worker.Dispose();
								};
								_worker.RunWorkerAsync();
							}
						}
						else
						{
							_messageBoxService.ShowInformation(PluginResources.CustomTemplateSelection_Message, PluginResources.InSourceViewController_CreateProjects_Create_Projects);
							_hasTemplateList.Clear();
						}
					}
					else
					{
						_messageBoxService.ShowInformation(PluginResources.WatchFoldersSelection_Message, PluginResources.InSourceViewController_CreateProjects_Create_Projects);
						_hasFiles.Clear();
					}
				}
			}
			catch (Exception e)
			{
				_logger.Error($"InSourceViewController->CreateProjects method: {e.Message}\n {e.StackTrace}");
			}
		}

		public void ClearNotification(InSourceNotification notification)
		{
			_eventAggregator.Publish(new RemoveStudioNotificationFromGroupEvent(NotificationGroupId, notification.Id));
		}

		private void SetStudioNotifications()
		{
			// Add the notifications to Studio Notifications panel
			var groupEvent = new AddStudioGroupNotificationEvent(_notificationGroup);
			_eventAggregator.Publish(groupEvent);

			var showNotification = new ShowStudioNotificationsViewEvent(true, true);
			_eventAggregator.Publish(showNotification);
		}

		private void PublishNotifications()
		{
			// remove the existing group notification to avoid notifications duplication
			var removeNotificationGroup = new RemoveStudioGroupNotificationEvent(_notificationGroup.Key);
			_eventAggregator.Publish(removeNotificationGroup);

			// clear existing notifications and publish the notification group
			_notificationGroup.Notifications.Clear();
			var addNotificationEvent = new AddStudioGroupNotificationEvent(_notificationGroup);
			_eventAggregator.Publish(addNotificationEvent);

			// set the notification view part focus
			var showNotification = new ShowStudioNotificationsViewEvent(true, true);
			_eventAggregator.Publish(showNotification);
		}

		private void SetWatchFoldersProjects(List<ProjectRequest> projectRequest)
		{
			if (projectRequest != null)
			{
				var watchFoldersList = GetWatchFolders(projectRequest);
				ProjectRequests.Clear();
				foreach (var warchFolder in watchFoldersList)
				{
					ProjectRequests.AddRange(GetNewDirectories(warchFolder, projectRequest));
				}
				if (ProjectRequests?.Count > 0)
				{
					Persistence.SaveProjectRequestList(ProjectRequests);

					foreach (var newProjectRequest in ProjectRequests)
					{
						var newProjectPath = Path.Combine(newProjectRequest.Path, newProjectRequest.Name);
						CreateSudioNotification(newProjectRequest, newProjectPath);
					}
				}
				OnProjectRequestsChanged();
				SetStudioNotifications();
			}
		}

		private void CreateProjectNotificationCommand(InSourceNotification notification)
		{
			Action createProjectAction = () => CreateProjectFromNotification(notification);
			_createProjectCommand = new InSourceCommand(createProjectAction)
			{
				CommandText = PluginResources.CreateProjectText,
				CommandToolTip = PluginResources.CreateNewProjectText
			};
			notification.Action = _createProjectCommand;
		}

		private void ClearNotificationAction(InSourceNotification notification)
		{
			Action dismissAction = () => ClearNotification(notification);
			var clearNotificationCommand = new InSourceCommand(dismissAction);
			notification.ClearNotificationAction = clearNotificationCommand;
		}

		private void CheckForProjectsEvent(object sender, EventArgs e)
		{
			CheckForProjects();
		}

		private void CreateProjectFromNotification(InSourceNotification notification)
		{
			try
			{
				if (notification != null)
				{
					var project = ProjectRequests.FirstOrDefault(n => n.NotificationId.Equals(notification.Id));

					CreateProjectsFromNotifications(project, notification);
				}
			}
			catch (Exception e)
			{
				_logger.Error($"CreateProjectFromNotification method: {e.Message}\n {e.StackTrace}");
			}
		}

		private List<string> GetWatchFolders(List<ProjectRequest> projectRequest)
		{
			var watchFoldersPath = projectRequest.GroupBy(x => x.Path).Select(y => y.First()); ;
			var foldersPath = new List<string>();
			foreach (var watch in watchFoldersPath)
			{
				var hasFiles = Directory.GetFiles(watch.Path, "*.*", SearchOption.AllDirectories).Any();
				//if the watch folder doesn't have file a new notification will be created for each watch folder, so we add only the folders which has files
				if (hasFiles)
				{
					foldersPath.Add(watch.Path);
				}
			}
			return foldersPath;
		}

		private List<ProjectRequest> GetNewDirectories(string watchFolderPath, List<ProjectRequest> projectRequests)
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

					if (dirInfo.Name != "AcceptedRequests")
					{
						var projectRequest = CreateProjectRequest(templateForWatchFolder, dirInfo, watchFolderPath);
						projectRequestList.Add(projectRequest);
					}
				}
			}
			else
			{
				var dirInfo = new DirectoryInfo(watchFolderPath);

				var projectRequest = CreateProjectRequest(templateForWatchFolder, dirInfo, watchFolderPath);
				projectRequestList.Add(projectRequest);
			}
			return projectRequestList;
		}

		private ProjectRequest CreateProjectRequest(ProjectTemplateInfo templateInfo, DirectoryInfo directory, string path)
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

		private bool HasFiles(string path)
		{
			var hasFiles = Directory.EnumerateFiles(path).Any();
			return hasFiles;
		}

		private void ReportMessage(string message)
		{
			Control.Value.BeginInvoke(new Action(() => Control.Value.ReportMessage(message)));
		}

		private void OnPropertyChanged(string propertyName)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		private void OnProjectRequestsChanged()
		{
			ProjectRequestsChanged?.Invoke(this, EventArgs.Empty);
		}

		private void InitializeBackgroundWorker()
		{
			_worker = new BackgroundWorker
			{
				WorkerReportsProgress = true
			};
		}
	}
}