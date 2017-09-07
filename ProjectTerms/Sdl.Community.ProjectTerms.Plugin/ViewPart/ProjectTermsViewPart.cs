using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.TranslationStudioAutomation.IntegrationApi;
using System;

namespace Sdl.Community.ProjectTerms.Plugin.ViewPart
{
    [ViewPart(
       Id = "CodingBreeze.ProjectsTermsViewPart",
       Name = "Project Terms Cloud",
       Description = "Show contents of the project terms in a word cloud.",
       Icon = "wordcloud")]
    [ViewPartLayout(Dock = DockType.Bottom, LocationByType = typeof(ProjectsController))]
    public class ProjectTermsViewPart : AbstractViewPartController
    {
        private readonly Lazy<ProjectTermsViewPartControl> control = new Lazy<ProjectTermsViewPartControl>(() => new ProjectTermsViewPartControl());

        protected override System.Windows.Forms.Control GetContentControl()
        {
            return control.Value;
        }

        protected override void Initialize() { }

        internal void GenerateWordCloud(ProjectTermsViewModel viewModel)
        {
            control.Value.GenerateWordCloud(viewModel);
        }

        internal void ResetCloud()
        {
            control.Value.Resetcloud();
        }
    }
}
