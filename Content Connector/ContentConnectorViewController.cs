using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.ProjectAutomation.Core;
using Sdl.ProjectAutomation.FileBased;
using Sdl.TranslationStudioAutomation.IntegrationApi;
using Sdl.TranslationStudioAutomation.IntegrationApi.Presentation.DefaultLocations;

namespace Sdl.Community.ContentConnector
{
    [View(
        Id = "ContentConnectorView",
        Name = "Content Connector",
        Description = "Create projects from project request content",
        Icon = "CheckForProjects_Icon",
        LocationByType = typeof(TranslationStudioDefaultViews.TradosStudioViewsLocation))]
    public class ContentConnectorViewController : AbstractViewController, INotifyPropertyChanged
    {
        #region private fields
        private readonly Lazy<ContentConnectorViewControl> _control = new Lazy<ContentConnectorViewControl>(() => new ContentConnectorViewControl());
        private ProjectTemplateInfo _selectedProjectTemplate;
        private List<ProjectRequest> _projectRequests;
        private List<ProjectRequest> _selectedProjects;
        private readonly List<bool> _hasTemplateList;
        public static Persistence Persistence = new Persistence();
        private int _percentComplete;
        #endregion private fields

        public event EventHandler ProjectRequestsChanged;

        public ContentConnectorViewController()
        {
            _projectRequests = new List<ProjectRequest>();
            _hasTemplateList = new List<bool>();
        }

        protected override void Initialize(IViewContext context)
        {
            ProjectsController = SdlTradosStudio.Application.GetController<ProjectsController>();
            _control.Value.Controller = this;
        }
        protected override System.Windows.Forms.Control GetContentControl()
        {
            return _control.Value;
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
            ProjectRequests = projectRequest;
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
                    }
                }

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
                                foreach (Tuple<ProjectRequest, FileBasedProject> request in creator.SuccessfulRequests)
                                {
                                    // accept the request
                                    ContentConnector.Instance.RequestAccepted(request.Item1);

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
        }

        public void Contribute()
        {
            System.Diagnostics.Process.Start("https://github.com/sdl/Sdl-Community/tree/master/Content%20Connector");
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
