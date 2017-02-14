using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Sdl.Community.Qualitivity.Panels.Main;
using Sdl.Community.Structures.Projects.Activities;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;

namespace Sdl.Community.Qualitivity.Panels.Activity_Records
{
    

    [ViewPart(
        Id = "QualitivityViewActivityRecordsController",
        Name = "Activity Records",
        Description = "Activity Records",
        Icon = "QualitivityViewActivityRecordsController_Icon"
        )]
    [ViewPartLayout(Dock = DockType.Bottom, ZIndex = 1, LocationByType = typeof(QualitivityViewController))]
    public class QualitivityViewActivityRecordsController : AbstractViewPartController
    {
        protected override Control GetContentControl()
        {
            return Control.Value;
        }

        protected override void Initialize()
        {
            
        }

        public static readonly Lazy<QualitivityViewActivityRecordsControl> Control = new Lazy<QualitivityViewActivityRecordsControl>(() => new QualitivityViewActivityRecordsControl());


        public static void UpdateReportsArea(List<Activity> activities)
        {


            Control.Value.Activities = activities;

            Control.Value.UpdateActivityReport();

           


        }

        
    }
}