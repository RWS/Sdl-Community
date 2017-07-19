using Sdl.Desktop.IntegrationApi;
using Sdl.FileTypeSupport.Framework.IntegrationApi;
using Sdl.ProjectAutomation.Core;
using System.Windows.Forms;

namespace Sdl.Community.ProjectTerms.Plugin
{
    public partial class ProjectTermsBatchTaskSettingsControl : UserControl, ISettingsAware<ProjectTermsBatchTaskSettings>
    {
        private ProjectTermsViewModel viewModel;

        public ProjectTermsBatchTaskSettings Settings { get; set; }

        public ProjectTermsViewModel ViewModel
        {
            get { return viewModel; }
            set { viewModel = value; }
        }

        public ProjectTermsBatchTaskSettingsControl()
        {
            InitializeComponent();

            ViewModel = new ProjectTermsViewModel();
        }

        public void ExtractProjectFileTerms(ProjectFile projectFile, IMultiFileConverter multiFileConverter)
        {
            ViewModel.ExtractProjectFileTerms(projectFile, multiFileConverter);
        }

        public void ExtractProjectTerms()
        {
            ViewModel.ExtractProjectTerms();
        }
    }
}
