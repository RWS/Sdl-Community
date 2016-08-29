using System;
using System.Dynamic;
using System.IO;
using System.Reflection;
using Sdl.Community.InvoiceAndQuotes.Integration.Properties;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.DefaultLocations;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.TranslationStudioAutomation.IntegrationApi;

namespace Sdl.Community.InvoiceAndQuotes.Integration
{
    [RibbonGroup("Sdl.Community.InvoiceAndQuotes", Name = "Invoice and quotes")]
    [RibbonGroupLayout(LocationByType = typeof(StudioDefaultRibbonTabs.AddinsRibbonTabLocation))]
    class InvoiceAndQuotesRibbon : AbstractRibbonGroup
    {
    }

    [Action("Sdl.Community.InvoiceAndQuotes", Name = "Invoice and quotes", Icon = "Invoice", Description = "Invoice and quotes")]
    [ActionLayout(typeof(InvoiceAndQuotesRibbon), 20, DisplayType.Large)]
    class InvoiceAndQuotesViewPart:AbstractAction
    {
        protected override void Execute()
        {
            var templateFolderPath = CreateTemplateFolder();
            AddFilesToTemplateFolder(templateFolderPath);

            var quotes = new StudioInQuote();
           
            quotes.ShowDialog();
        }



        private string CreateTemplateFolder()
        {
            var templateFolderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), @"SDL Community\Invoice\Office Templates");
            if (!Directory.Exists(templateFolderPath))
            {
                Directory.CreateDirectory(templateFolderPath);
            }

            return templateFolderPath;
        }

        private void AddFilesToTemplateFolder(string folderPath)
        {
            if (!File.Exists(Path.Combine(folderPath, "Grouped AnalysisTemplate.docx")))
            {
                File.WriteAllBytes(Path.Combine(folderPath, "Grouped AnalysisTemplate.docx"), Resources.Grouped_AnalysisTemplate);
            }

            if (!File.Exists(Path.Combine(folderPath, "Grouped AnalysisTemplate.xlsx")))
            {
                File.WriteAllBytes(Path.Combine(folderPath, "Grouped AnalysisTemplate.xlsx"), Resources.Grouped_AnalysisTemplate1);
            }

            if (!File.Exists(Path.Combine(folderPath, "Simple Excel Template.xlsx")))
            {
                File.WriteAllBytes(Path.Combine(folderPath, "Simple Excel Template.xlsx"), Resources.Simple_Excel_Template);
            }

            if (!File.Exists(Path.Combine(folderPath, "Simple Word Template.docx")))
            {
                File.WriteAllBytes(Path.Combine(folderPath, "Simple Word Template.docx"), Resources.Simple_Word_Template);
            }

            if (!File.Exists(Path.Combine(folderPath, "Standard Lines Template.docx")))
            {
                File.WriteAllBytes(Path.Combine(folderPath, "Standard Lines Template.docx"), Resources.Standard_Lines_Template);
            }

            if (!File.Exists(Path.Combine(folderPath, "Standard Lines Template.xlsx")))
            {
                File.WriteAllBytes(Path.Combine(folderPath, "Standard Lines Template.xlsx"), Resources.Standard_Lines_Template1);
            }
        }
    }
}
