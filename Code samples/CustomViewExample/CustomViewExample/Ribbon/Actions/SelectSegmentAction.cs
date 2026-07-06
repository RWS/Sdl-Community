using System.Diagnostics;
using CustomViewExample.Services;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.TranslationStudioAutomation.IntegrationApi;
using Sdl.TranslationStudioAutomation.IntegrationApi.Presentation.DefaultLocations;
using System.Windows;

namespace CustomViewExample.Ribbon.Actions
{
    [RibbonGroup("CustomViewExample_SegmentActionsGroup", "Segment Actions")]
    [RibbonGroupLayout(LocationByType = typeof(TranslationStudioDefaultRibbonTabs.EditorReviewRibbonTabLocation))]
    public class SegmentActionsGroup : AbstractRibbonGroup
    {
    }

    /// <summary>
    /// Selects a segment in the active document via SetActiveSegmentPair, using the document
    /// instance tracked by <see cref="EditorControllerService"/>.
    /// Known issue: SetActiveSegmentPair returns false and nothing happens, with any file
    /// and any segment id.
    /// </summary>
    internal abstract class SelectSegmentAction : AbstractAction
    {
        protected abstract string SegmentId { get; }

        protected override void Execute()
        {
            var service = EditorControllerService.Instance;

            var document = service.ActiveDocument ?? service.EditorController.ActiveDocument;
            if (document == null)
            {
                MessageBox.Show("There is no active document.", "Select Segment " + SegmentId);
                return;
            }

            var activeFile = document.ActiveFile;

            var result = document.SetActiveSegmentPair(activeFile, SegmentId, true);

            Trace.WriteLine("[CustomViewExample] SelectSegment: SetActiveSegmentPair(\""
                            + activeFile?.Name + "\", \"" + SegmentId + "\") returned " + result);

            MessageBox.Show("SetActiveSegmentPair for segment " + SegmentId
                            + " in \"" + activeFile?.Name + "\" returned: " + result,
                "Select Segment " + SegmentId);
        }
    }

    [Action(Id = "CustomViewExample_SelectSegment1",
        Name = "Select Segment 1",
        Description = "Sets segment 1 as the active segment pair in the active document",
        Icon = "wordLight_yellow", ContextByType = typeof(EditorController))]
    [ActionLayout(typeof(SegmentActionsGroup), 3, DisplayType.Large, "Select Segment 1", true)]
    internal class SelectSegment1Action : SelectSegmentAction
    {
        protected override string SegmentId => "1";
    }

    [Action(Id = "CustomViewExample_SelectSegment2",
        Name = "Select Segment 2",
        Description = "Sets segment 2 as the active segment pair in the active document",
        ContextByType = typeof(EditorController))]
    [ActionLayout(typeof(SegmentActionsGroup), 0, DisplayType.Normal, "Select Segment 2", true)]
    internal class SelectSegment2Action : SelectSegmentAction
    {
        protected override string SegmentId => "2";
    }

    [Action(Id = "CustomViewExample_SelectSegment3",
        Name = "Select Segment 3",
        Description = "Sets segment 3 as the active segment pair in the active document",
        ContextByType = typeof(EditorController))]
    [ActionLayout(typeof(SegmentActionsGroup), 0, DisplayType.Normal, "Select Segment 3", true)]
    internal class SelectSegment3Action : SelectSegmentAction
    {
        protected override string SegmentId => "3";
    }
}
