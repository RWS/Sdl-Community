using System;
using System.Windows.Forms;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.TranslationStudioAutomation.IntegrationApi;

namespace Sdl.Community.Productivity.UI
{
    [ViewPart(
       Id = "GamificationReportViewPart",
       Name = "Community Productivity",
       Description = "Community Productivity",
       Icon = "icon"
       )]
    [ViewPartLayout(typeof(EditorController), Dock = DockType.Right, GroupStyle = GroupStyle.Tab, Width = 300,MinWidth = 200,MinHeight = 150,Pinned = false)]
    public class ProductivityReportViewPart : AbstractViewPartController
    {
        private static readonly Lazy<ProductivityControl> _control = new Lazy<ProductivityControl>(() => new ProductivityControl());

        protected override Control GetContentControl()
        {
            return _control.Value;
        }


        protected override void Initialize()
        {
        }

        
    }

   
}
