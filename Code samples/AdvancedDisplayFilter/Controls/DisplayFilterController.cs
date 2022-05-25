using System.Windows.Forms;
using Sdl.Desktop.IntegrationApi;
using Sdl.TranslationStudioAutomation.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;

namespace Sdl.Community.AdvancedDisplayFilter.Controls
{
	[ViewPart(
    Id = "CommunityAdvancedDisplayFilterViewPart",
    Name = "AdvancedDisplayFilterViewPart_Name",
    Description = "AdvancedDisplayFilterViewPart_Description",
    Icon = "AdvancedDisplayFilter_Icon")]
    [ViewPartLayout(
        typeof(EditorController), Dock = DockType.Right)]
    public class DisplayFilterController : AbstractViewPartController
    {
        protected override Control GetContentControl()
        {
	        return CommunityApplicationInitializer.DisplayFilterControl; 
        }

        protected override void Initialize()
        {
        }
    }
}
