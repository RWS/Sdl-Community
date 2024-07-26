using Sdl.Community.AntidoteVerifier.Antidote_API;
using Sdl.Community.AntidoteVerifier.Utils;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.TranslationStudioAutomation.IntegrationApi;
using Sdl.TranslationStudioAutomation.IntegrationApi.Presentation.DefaultLocations;
using Serilog;

namespace Sdl.Community.AntidoteVerifier
{
	[RibbonGroup("Sdl.Community.AntidoteVerifier", Name ="Antidote Verifier", ContextByType = typeof(EditorController))]
    [RibbonGroupLayout(LocationByType = typeof(TranslationStudioDefaultRibbonTabs.EditorReviewRibbonTabLocation))]
    public class AntidoteVerifierRibbon: AbstractRibbonGroup
    {
       
    }

    [Action("Sdl.Community.AntidoteVerifier.CorrectorAction",
        Name ="Corrector",
        Icon = "boutonCorrecteur",
        Description ="Run Antidote verification")]
    [ActionLayout(typeof(AntidoteVerifierRibbon),40, DisplayType.Large)]
    [ActionLayout(typeof(TranslationStudioDefaultContextMenus.EditorDocumentContextMenuLocation), 3, DisplayType.Default)]
    
    public class AntidoteCorrectorAction: AbstractAction
    {
       
        protected override void Execute()
        {
            Logger.IntializeLogger();
            var editorController = SdlTradosStudio.Application.GetController<EditorController>();
            var editorService = new EditorService(editorController.ActiveDocument);
			var antidotedClient = new AntidoteClient(editorService,true);
			var antidoteApiOle = new AntidoteApiOle(antidotedClient);
            Log.Information("Starting Antidote for correction!");
            antidoteApiOle.CallAntidote(ConstantsUtils.Corrector);
        }
    }

    [Action("Sdl.Community.AntidoteVerifier.DictionaryAction", Name = "Dictionaries", Icon = "dictionary", Description = "Run Antidote dictionary")]
    [ActionLayout(typeof(AntidoteVerifierRibbon), 10, DisplayType.Normal)]
    [ActionLayout(typeof(TranslationStudioDefaultContextMenus.EditorDocumentContextMenuLocation),
        1,
        DisplayType = DisplayType.Default,
        Name = "Dictionaries",
        IsSeparator = false)]
    public class AntidoteDictionaryAction : AbstractAction
    {

        protected override void Execute()
        {
            Logger.IntializeLogger();

			var editorController = SdlTradosStudio.Application.GetController<EditorController>();
			var editorService = new EditorService(editorController.ActiveDocument);
            var antidotedClient = new AntidoteClient(editorService,false);
			var antidoteApiOle = new AntidoteApiOle(antidotedClient);
            Log.Information("Starting Antidote for dictionary!");

            antidoteApiOle.CallAntidote(ConstantsUtils.LastSelectedDictionary);
        }
    }

    [Action("Sdl.Community.AntidoteVerifier.GuideAction", Name = "Guides", Icon = "guide", Description = "Run Antidote guide")]
    [ActionLayout(typeof(AntidoteVerifierRibbon), 10, DisplayType.Normal)]
    [ActionLayout(typeof(TranslationStudioDefaultContextMenus.EditorDocumentContextMenuLocation),
        2,
        Name = "Guides",
        IsSeparator = false)]
    public class AntidoteGuideAction : AbstractAction
    {
        protected override void Execute()
        {
            Logger.IntializeLogger();

            var editorController = SdlTradosStudio.Application.GetController<EditorController>();
            var editorService = new EditorService(editorController.ActiveDocument);

            var antidotedClient = new AntidoteClient(editorService,false);
            var antidoteApiOle = new AntidoteApiOle(antidotedClient);
            Log.Information("Starting Antidote for guide!");

            antidoteApiOle.CallAntidote(ConstantsUtils.LastSelectedGuide);
        }
    }
       
}
