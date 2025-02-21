using System;
using System.Windows.Forms;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.Desktop.IntegrationApi.Interfaces;

namespace Sdl.Community.PostEdit.Versions
{
    

    [ViewPart(
        Id = "PostEditComparePartController",
        Name = "Project Version Details",
        Description = "Project Version Details",
        Icon = "PostEditProjectVersionDetails_Icon"
        )]
    [ViewPartLayout(typeof(PostEditCompareViewController), Dock = DockType.Right)]
    public class PostEditCompareViewPartController : AbstractViewPartController
    {
        protected override IUIControl GetContentControl()
        {
            return Control.Value;
        }

        protected override void Initialize()
        {
            
        }

        public Lazy<PostEditCompareViewPartControl> Control = new Lazy<PostEditCompareViewPartControl>(() => new PostEditCompareViewPartControl());
    }
}