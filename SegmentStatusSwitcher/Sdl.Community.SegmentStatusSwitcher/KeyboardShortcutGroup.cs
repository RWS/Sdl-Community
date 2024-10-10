using System.Collections.Generic;
using System.Linq;
using Sdl.Core.Globalization;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.TranslationStudioAutomation.IntegrationApi;
using Sdl.TranslationStudioAutomation.IntegrationApi.Presentation.DefaultLocations;

namespace Sdl.Community.SegmentStatusSwitcher
{
    [RibbonGroup("Segment Status Switcher", Name = "Segment Status Switcher", Description = "Segment Status Switcher", ContextByType = typeof(EditorController))]
    [RibbonGroupLayout(LocationByType = typeof(TranslationStudioDefaultRibbonTabs.AddinsRibbonTabLocation))]
    public class KeyboardShortcutGroup : AbstractRibbonGroup
    {
        public static EditorController GetEditorController()
        {
            return SdlTradosStudio.Application.GetController<EditorController>();
        }

        public static void ChangeSegmentStatus(ConfirmationLevel confirmationLevel)
        {
            var activeDocument = GetEditorController().ActiveDocument;
	        if (activeDocument == null) return;

	        var selectedSegmentPairs = activeDocument.GetSelectedSegmentPairs().ToList();
	        AddSegmentPair(selectedSegmentPairs, activeDocument.ActiveSegmentPair);

	        foreach (var segmentPair in selectedSegmentPairs)
	        {
                activeDocument.ChangeConfirmationLevelOnSegment(segmentPair, confirmationLevel);
	        }
        }

	    private static void AddSegmentPair(List<ISegmentPair> selectedSegmentPairs, ISegmentPair newSegmentPair)
	    {
			if (newSegmentPair == null) return;
		    if (selectedSegmentPairs.All(sp => sp.Properties.Id != newSegmentPair.Properties.Id))
		    {
			    selectedSegmentPairs.Add(newSegmentPair);
		    }
	    }

	    [Action("NotTranslatedStatusAction", Name = "Not Translated", Description = "Not Translated", Icon = "ConfirmationLevel_Unspecified")]
        [ActionLayout(typeof(KeyboardShortcutGroup), DisplayType = DisplayType.ImageOnly)]
        public class NotTranslatedStatusAction : AbstractAction
        {        
            protected override void Execute()
            {
                ChangeSegmentStatus(ConfirmationLevel.Unspecified);               
            }
        }

        [Action("DraftStatusAction", Name = "Draft", Description = "Draft", Icon = "ConfirmationLevel_Draft")]
        [ActionLayout(typeof(KeyboardShortcutGroup), DisplayType = DisplayType.ImageOnly)]
        public class DraftStatusAction : AbstractAction
        {

            protected override void Execute()
            {
                ChangeSegmentStatus(ConfirmationLevel.Draft);
            }
        }

        [Action("TranslatedStatusAction", Name = "Translated", Description = "Translated", Icon = "ConfirmationLevel_Translated")]
        [ActionLayout(typeof(KeyboardShortcutGroup), DisplayType = DisplayType.ImageOnly)]
        public class TranslatedStatusAction : AbstractAction
        {
            protected override void Execute()
            {
                ChangeSegmentStatus(ConfirmationLevel.Translated);
            }
        }

        [Action("TranslationRejectedStatusAction", Name = "Translation Rejected", Description = "Translation Rejected", Icon = "ConfirmationLevel_RejectedTranslation")]
        [ActionLayout(typeof(KeyboardShortcutGroup), DisplayType = DisplayType.ImageOnly)]
        public class TranslationRejectedStatusAction : AbstractAction
        {
            protected override void Execute()
            {
                ChangeSegmentStatus(ConfirmationLevel.RejectedTranslation);
            }
        }

        [Action("TranslationApprovedStatusAction", Name = "Translation Approved", Description = "Translation Approved", Icon = "ConfirmationLevel_ApprovedTranslation")]
        [ActionLayout(typeof(KeyboardShortcutGroup), DisplayType = DisplayType.ImageOnly)]
        public class TranslationApprovedStatusAction : AbstractAction
        {          
            protected override void Execute()
            {
                ChangeSegmentStatus(ConfirmationLevel.ApprovedTranslation);
            }
        }

        [Action("SignOffRejectedStatusAction", Name = "Sign-off Rejected", Description = "Sign-off Rejected", Icon = "ConfirmationLevel_RejectedSignOff")]
        [ActionLayout(typeof(KeyboardShortcutGroup), DisplayType = DisplayType.ImageOnly)]
        public class SignOffRejectedStatusAction : AbstractAction
        {
            protected override void Execute()
            {
                ChangeSegmentStatus(ConfirmationLevel.RejectedSignOff);
            }
        }

        [Action("SignedOffStatusAction", Name = "Signed Off", Description = "Signed Off", Icon = "ConfirmationLevel_ApprovedSignOff")]
        [ActionLayout(typeof(KeyboardShortcutGroup), DisplayType = DisplayType.ImageOnly)]
        public class SignedOffStatusAction : AbstractAction
        {           
            protected override void Execute()
            {
                ChangeSegmentStatus(ConfirmationLevel.ApprovedSignOff);
            }
        }
    }
}