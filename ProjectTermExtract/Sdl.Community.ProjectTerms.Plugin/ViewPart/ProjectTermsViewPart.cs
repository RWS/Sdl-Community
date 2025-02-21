using Sdl.Desktop.IntegrationApi;
using System;
using Sdl.Desktop.IntegrationApi.Interfaces;

namespace Sdl.Community.ProjectTerms.Plugin.ViewPart
{

    public class ProjectTermsViewPart : AbstractViewPartController
    {
        private readonly Lazy<ProjectTermsViewPartControl> control = new Lazy<ProjectTermsViewPartControl>(() => new ProjectTermsViewPartControl());

        protected override IUIControl GetContentControl()
        {
	        StudioContext.RaiseControllersAvailableEvent();
            return control.Value;
        }

        protected override void Initialize() { }

        internal void GenerateWordCloud(ProjectTermsViewModel viewModel)
        {
            control.Value.ViewModel = viewModel;
            control.Value.GenerateWordCloud();
        }
    }
}
