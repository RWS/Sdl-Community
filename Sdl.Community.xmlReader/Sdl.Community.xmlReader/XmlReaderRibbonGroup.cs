using System;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.TranslationStudioAutomation.IntegrationApi.Presentation.DefaultLocations;
using Sdl.TranslationStudioAutomation.IntegrationApi;
using Sdl.Community.XmlReader.WPF;

namespace Sdl.Community.XmlReader
{
    [RibbonGroup("Sdl.Community.XmlReaderRibbonButton", Name = "", ContextByType = typeof(ProjectsController))]
    [RibbonGroupLayout(LocationByType = typeof(TranslationStudioDefaultRibbonTabs.HomeRibbonTabLocation))]
    public class XmlReaderRibbonGroup : AbstractRibbonGroup
    {
    }

    [Action("Sdl.Community.XmlReader.GenerateXmlReaderAction", Name = "Xml Reader", Icon = "logo", Description = "Read xml files and export them as Excel...")]
    [ActionLayout(typeof(XmlReaderRibbonGroup), 250, DisplayType.Large)]
    class GenerateXmlReaderAction : AbstractAction
    {
        protected override void Execute()
        {
            var xmlReaderWindow = new MainWindow();
            xmlReaderWindow.ShowDialog();
        }
    }
}
