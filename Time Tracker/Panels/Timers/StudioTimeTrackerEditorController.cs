using System;
using System.Windows.Forms;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.TranslationStudioAutomation.IntegrationApi;

namespace Sdl.Community.Studio.Time.Tracker.Panels.Timers
{
    [ViewPart(
       Id = "StudioTimeTrackerEditorController",
       Name = "Activity Tracker",
       Description = "Activity Tracker",
       Icon = "StudioTimeTrackerTimerController_Icon"
       )]
    [ViewPartLayout(typeof(EditorController), Dock = DockType.Right)]
    public class StudioTimeTrackerEditorController : AbstractViewPartController
    {


        protected override Control GetContentControl()
        {
            return Control.Value;
        }

        protected override void Initialize()
        {
   
        }

       

        private static readonly Lazy<StudioTimeTrackerEditorControl> Control = new Lazy<StudioTimeTrackerEditorControl>(() => new StudioTimeTrackerEditorControl());      
    }
}
