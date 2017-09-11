using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.TranslationStudioAutomation.IntegrationApi;

namespace Sdl.Community.ProjectTerms.Plugin.ViewPart
{
    [ViewPart(
        Id = "CodingBreeze.ProjectsTermsViewPart",
        Name = "Project Terms Cloud",
        Description = "Show contents of the project terms in a word cloud.",
        Icon = "wordcloud")]
    [ViewPartLayout(Dock = DockType.Bottom, LocationByType = typeof(FilesController))]
    public class ProjectTermsViewPartFiles : ProjectTermsViewPart
    {
    }
}
