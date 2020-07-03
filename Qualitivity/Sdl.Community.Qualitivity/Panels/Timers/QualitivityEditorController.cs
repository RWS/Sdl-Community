using System;
using System.Windows.Forms;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.Desktop.IntegrationApi.Interfaces;
using Sdl.TranslationStudioAutomation.IntegrationApi;

namespace Sdl.Community.Qualitivity.Panels.Timers
{
    [ViewPart(
       Id = "QualitivityEditorController",
       Name = "Activity Tracker",
       Description = "Activity Tracker",
       Icon = "QualitivityTimerController_Icon"
       )]
    [ViewPartLayout(typeof(EditorController), Dock = DockType.Right)]
    public class QualitivityEditorController : AbstractViewPartController
    {


        protected override IUIControl GetContentControl()
        {
            return Control.Value;
        }

        protected override void Initialize()
        {
   
        }



        private static readonly Lazy<QualitivityEditorControl> Control = new Lazy<QualitivityEditorControl>(() => new QualitivityEditorControl());      
    }
}
