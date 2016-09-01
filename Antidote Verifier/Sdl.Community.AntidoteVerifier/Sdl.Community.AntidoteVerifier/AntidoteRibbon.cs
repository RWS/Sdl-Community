using Sdl.Community.AntidoteVerifier.Antidote_API;
using Sdl.Community.AntidoteVerifier.Utils;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.TranslationStudioAutomation.IntegrationApi;
using Sdl.TranslationStudioAutomation.IntegrationApi.Presentation.DefaultLocations;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sdl.Community.AntidoteVerifier
{
    [RibbonGroup("Sdl.Community.AntidoteVerifier", Name ="Antidote Verifier", ContextByType = typeof(EditorController))]
    [RibbonGroupLayout(LocationByType = typeof(TranslationStudioDefaultRibbonTabs.EditorAdvancedRibbonTabLocation))]
    public class AntidoteVerifierRibbon: AbstractRibbonGroup
    {
    }

    [Action("Sdl.Community.AntidoteVerifier.CorrectorAction", Name ="Corrector", Icon = "boutonCorrecteur", Description ="Run Antidote verification")]
    [ActionLayout(typeof(AntidoteVerifierRibbon), 40, DisplayType.Large)]
    public class AntidoteCorrectorAction: AbstractAction
    {
        protected override void Execute()
        {
            Logger.IntializeLogger();
            EditorController editorController = SdlTradosStudio.Application.GetController<EditorController>();
            EditorService editorService = new EditorService(editorController.ActiveDocument);
            AntidoteClient antidotedClient = new AntidoteClient(editorService);
            AntidoteApiOle antidoteApiOle = new AntidoteApiOle(antidotedClient);
            Log.Information("Starting Antidote for correction!");
            antidoteApiOle.CallAntidote(ConstantsUtils.Corrector);
        }
    }

    [Action("Sdl.Community.AntidoteVerifier.DictionaryAction", Name = "Dictionary", Icon = "dictionary", Description = "Run Antidote dictionary")]
    [ActionLayout(typeof(AntidoteVerifierRibbon), 10, DisplayType.Normal)]
    public class AntidoteDictionaryAction : AbstractAction
    {
        protected override void Execute()
        {
            Logger.IntializeLogger();

            EditorController editorController = SdlTradosStudio.Application.GetController<EditorController>();
            EditorService editorService = new EditorService(editorController.ActiveDocument);
            AntidoteClient antidotedClient = new AntidoteClient(editorService);
            AntidoteApiOle antidoteApiOle = new AntidoteApiOle(antidotedClient);
            Log.Information("Starting Antidote for dictionary!");

            antidoteApiOle.CallAntidote(ConstantsUtils.LastSelectedDictionary);
        }
    }

    [Action("Sdl.Community.AntidoteVerifier.GuideAction", Name = "Guide", Icon = "guide", Description = "Run Antidote guide")]
    [ActionLayout(typeof(AntidoteVerifierRibbon), 10, DisplayType.Normal)]
    public class AntidoteGuideAction : AbstractAction
    {
        protected override void Execute()
        {
            Logger.IntializeLogger();

            EditorController editorController = SdlTradosStudio.Application.GetController<EditorController>();
            EditorService editorService = new EditorService(editorController.ActiveDocument);

            AntidoteClient antidotedClient = new AntidoteClient(editorService);
            AntidoteApiOle antidoteApiOle = new AntidoteApiOle(antidotedClient);
            Log.Information("Starting Antidote for guide!");

            antidoteApiOle.CallAntidote(ConstantsUtils.LastSelectedGuide);
        }
    }
}
