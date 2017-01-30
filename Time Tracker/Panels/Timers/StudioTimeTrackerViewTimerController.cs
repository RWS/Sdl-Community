using System;
using System.Windows.Forms;
using Sdl.Community.Studio.Time.Tracker.Panels.Main;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;

namespace Sdl.Community.Studio.Time.Tracker.Panels.Timers
{
    

    [ViewPart(
        Id = "StudioTimeTrackerTimeTrackerController",
        Name = "Activity Tracker",
        Description = "Activity Tracker",
        Icon = "StudioTimeTrackerTimerController_Icon"
        )]
    [ViewPartLayout(typeof(StudioTimeTrackerViewController), Dock = DockType.Right)]
    public class StudioTimeTrackerViewTimerController : AbstractViewPartController
    {
        protected override Control GetContentControl()
        {
            return Control.Value;
        }

        protected override void Initialize()
        {
            
        }

        public Lazy<StudioTimeTrackerViewTimerControl> Control = new Lazy<StudioTimeTrackerViewTimerControl>(() => new StudioTimeTrackerViewTimerControl());
    }
}