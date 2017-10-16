using System;
using System.Windows.Forms;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.TranslationStudioAutomation.IntegrationApi.Presentation.DefaultLocations;
using Sdl.TranslationStudioAutomation.IntegrationApi;

namespace Sdl.Community.xmlReader
{
    [RibbonGroup("Sdl.Community.xmlReaderRibbonButton", Name = "", ContextByType = typeof(ProjectsController))]
    [RibbonGroupLayout(LocationByType = typeof(TranslationStudioDefaultRibbonTabs.HomeRibbonTabLocation))]
    public class XmlReaderRibbonGroup : AbstractRibbonGroup
    {
    }

    [Action("Sdl.Community.xmlReader.GenerateXmlReaderAction", Name = "xml Reader", Icon = "logo", Description = "Read xml files and export them as Excel...")]
    [ActionLayout(typeof(XmlReaderRibbonGroup), 250, DisplayType.Large)]
    class GenerateXmlReaderAction : AbstractAction
    {
        protected override void Execute()
        {
            MessageBox.Show(@"hello!", "info");
        }
    }
}
