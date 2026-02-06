using System.Windows.Forms;
using Sdl.Desktop.IntegrationApi;
using Sdl.TranslationStudioAutomation.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.Desktop.IntegrationApi.Interfaces;

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
        protected override IUIControl GetContentControl()
        {
	        return CommunityApplicationInitializer.DisplayFilterControl; 
        }

        protected override void Initialize()
        {
        }
    }
}
