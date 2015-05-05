using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Sdl.Community.Jobs.Services;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.TranslationStudioAutomation.IntegrationApi.Presentation.DefaultLocations;
using Sdl.Community.Jobs.UI;

namespace Sdl.Community.Jobs
{
    [View(Id="Community.Jobs", Name="Jobs",Description = "This is where you can find tranlation jobs.", Icon="icon", LocationByType = typeof(TranslationStudioDefaultViews.TradosStudioViewsLocation))]
    public class JobsView : AbstractViewController
    {
        protected static JobService Service = new JobService();

        private readonly Lazy<JobsControl> _viewControl = new Lazy<JobsControl>(() => new JobsControl(Service));
        private readonly Lazy<SearchCriteriaControl> _searchControl = new Lazy<SearchCriteriaControl>(() => new SearchCriteriaControl(Service));

        public JobsView()
        {
            
        }


        protected override Control GetContentControl()
        {
            return _viewControl.Value;
        }

        protected override Control GetExplorerBarControl()
        {
            return _searchControl.Value;
        }


        protected override void Initialize(IViewContext context)
        {
            
            ActivationChanged += JobsView_ActivationChanged;
        }

        void JobsView_ActivationChanged(object sender, ActivationChangedEventArgs e)
        {
         //   System.Diagnostics.Debugger.Launch();
        }
    }
}
