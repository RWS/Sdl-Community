using System.Diagnostics;
using System.Linq;
using CustomViewExample.Services;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.TranslationStudioAutomation.IntegrationApi;
using Sdl.TranslationStudioAutomation.IntegrationApi.Presentation.DefaultLocations;
using System.Windows;

namespace CustomViewExample.Ribbon.Actions
{
    [RibbonGroup("CustomViewExample_DocumentActionsGroup", "Document Actions")]
    [RibbonGroupLayout(LocationByType = typeof(TranslationStudioDefaultRibbonTabs.EditorReviewRibbonTabLocation))]
    public class DocumentActionsGroup : AbstractRibbonGroup
    {
    }

    [Action(Id = "CustomViewExample_ActivateFirstDocument",
        Name = "Activate First Document",
        Description = "Activates the first tracked document and shows GetDocuments() and event diagnostics",
        Icon = "wordLight_yellow", ContextByType = typeof(EditorController))]
    [ActionLayout(typeof(DocumentActionsGroup), 3, DisplayType.Large, "Activate First Document", true)]
    internal class ActivateFirstDocumentAction : AbstractAction
    {
        protected override void Execute()
        {
            var service = EditorControllerService.Instance;

            // Diagnostics: compare GetDocuments() against the documents tracked from events
            var controllerDocuments = service.EditorController.GetDocuments()?.ToList();
            var controllerCount = controllerDocuments?.Count ?? 0;
            var controllerNullCount = controllerDocuments?.Count(d => d == null) ?? 0;

            var eventCounts = string.Join("\n", service.EventCounts.Select(c => "  " + c.Key + ": " + c.Value));
            var diagnostics = "Tracked documents: " + service.Documents.Count
                              + "\nGetDocuments(): " + controllerCount + " (" + controllerNullCount + " null)"
                              + "\n\nEvents fired:\n" + eventCounts;

            Trace.WriteLine("[CustomViewExample] ActivateFirstDocument: GetDocuments() returned "
                            + controllerCount + " entries (" + controllerNullCount + " null), tracking "
                            + service.Documents.Count + " documents");

            var documents = service.Documents;
            if (!documents.Any())
            {
                MessageBox.Show("There are no tracked documents to activate.\n\n" + diagnostics,
                    "Activate First Document");
                return;
            }

            var document = documents.First();
            service.Activate(document);

            MessageBox.Show("Requested activation of: " + document.Files.First().Name + "\n\n" + diagnostics,
                "Activate First Document");
        }
    }

    [Action(Id = "CustomViewExample_ActivateNextDocument",
        Name = "Activate Next Document",
        Description = "Cycles activation to the next tracked document to exercise Activate(document)",
        ContextByType = typeof(EditorController))]
    [ActionLayout(typeof(DocumentActionsGroup), 0, DisplayType.Normal, "Activate Next Document", true)]
    internal class ActivateNextDocumentAction : AbstractAction
    {
        protected override void Execute()
        {
            var service = EditorControllerService.Instance;

            var documents = service.Documents;
            if (!documents.Any())
            {
                MessageBox.Show("There are no tracked documents to activate.", "Activate Next Document");
                return;
            }

            // Cycle to the next tracked document, so repeated clicks exercise Activate(document)
            // with a different instance each time
            var activeIndex = documents.ToList().IndexOf(service.ActiveDocument);
            var document = documents[(activeIndex + 1) % documents.Count];

            service.Activate(document);

            MessageBox.Show("Requested activation of: " + document.Files.First().Name
                            + "\nTracked documents: " + documents.Count,
                "Activate Next Document");
        }
    }
}
