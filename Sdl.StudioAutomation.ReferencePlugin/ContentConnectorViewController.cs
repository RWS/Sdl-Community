using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.TranslationStudioAutomation.IntegrationApi.Presentation;
using Sdl.Desktop.IntegrationApi;
using System.ComponentModel;
using Sdl.ProjectAutomation.Core;
using Sdl.ProjectAutomation.FileBased;
using Sdl.TranslationStudioAutomation.IntegrationApi;
using System.Windows.Forms;
using Sdl.TranslationStudioAutomation.IntegrationApi.Presentation.DefaultLocations;

namespace StudioIntegrationApiSample
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
        private int _percentComplete;
        #endregion private fields

        public event EventHandler ProjectRequestsChanged;

        public ContentConnectorViewController()
        {
            _projectRequests = new List<ProjectRequest>();
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
            ContentConnector.Refresh();
            ProjectRequests = ContentConnector.ProjectRequests;
        }

        public void CreateProjects()
        {
            _control.Value.ClearMessages();

            ProjectCreator creator = null;
            BackgroundWorker worker = new BackgroundWorker();
            worker.WorkerReportsProgress = true;
            worker.DoWork += (sender, e) =>
                {
                    creator = new ProjectCreator(ProjectRequests, SelectedProjectTemplate);
                    creator.ProgressChanged += (sender2, e2) => { worker.ReportProgress(e2.ProgressPercentage); };
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
