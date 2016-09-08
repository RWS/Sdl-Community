using System;
using System.Linq;
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
    Icon = "AdvancedDisplayFiltersIcon")]
    [ViewPartLayout(
        typeof(EditorController), Dock = DockType.Right)]
    public class DisplayFilterController : AbstractViewPartController
    {
        private readonly DisplayFilterControl _control = new DisplayFilterControl();
      
        protected override Control GetContentControl()
        {
            return _control;
        }

        protected override void Initialize()
        {
        }
    }
}
