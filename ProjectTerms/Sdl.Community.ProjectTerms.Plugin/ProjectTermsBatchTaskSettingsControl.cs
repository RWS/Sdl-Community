using Sdl.Desktop.IntegrationApi;
using System;
using System.Windows.Forms;

namespace Sdl.Community.ProjectTerms.Plugin
{
    public partial class ProjectTermsBatchTaskSettingsControl : UserControl, ISettingsAware<ProjectTermsBatchTaskSettings>
    {
        public ProjectTermsBatchTaskSettings Settings { get; set; }
    }
}
