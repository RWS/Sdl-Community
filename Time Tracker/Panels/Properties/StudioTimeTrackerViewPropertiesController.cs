using System;
using System.Windows.Forms;
using Sdl.Community.Studio.Time.Tracker.Panels.Main;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;

namespace Sdl.Community.Studio.Time.Tracker.Panels.Properties
{
    

    [ViewPart(
        Id = "StudioTimeTrackerPropertiesController",
        Name = "Properties",
        Description = "Properties",
        Icon = "StudioTimeTrackerPropertiesController_Icon"
        )]
    [ViewPartLayout(typeof(StudioTimeTrackerViewController), Dock = DockType.Right)]
    public class StudioTimeTrackerViewPropertiesController : AbstractViewPartController
    {
        protected override Control GetContentControl()
        {
            return Control.Value;
        }

        protected override void Initialize()
        {
            
        }

        public  Lazy<StudioTimeTrackerViewPropertiesControl> Control = new Lazy<StudioTimeTrackerViewPropertiesControl>(() => new StudioTimeTrackerViewPropertiesControl());
    }
}