using Sdl.Community.DeepLMTProvider.Client;
using Sdl.Community.DeepLMTProvider.Helpers;
using Sdl.Community.DeepLMTProvider.Helpers.GlossaryReadersWriters;
using Sdl.Community.DeepLMTProvider.Service;
using Sdl.Community.DeepLMTProvider.UI;
using Sdl.Community.DeepLMTProvider.ViewModel;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.TranslationStudioAutomation.IntegrationApi.Presentation.DefaultLocations;
using System;

namespace Sdl.Community.DeepLMTProvider.Studio.Actions
{
    [RibbonGroup(nameof(DeepLRibbonGroup), Name = "DeepL Glossaries")]
    [RibbonGroupLayout(LocationByType = typeof(TranslationStudioDefaultRibbonTabs.AddinsRibbonTabLocation))]
    public class DeepLRibbonGroup : AbstractRibbonGroup
    {
    }

    [Action(nameof(ImportGlossaryAction), Icon = "deepLIcon", Name = "View/Import glossaries",
        Description = "View glossaries and import new ones")]
    [ActionLayout(typeof(DeepLRibbonGroup), 10, DisplayType.Large)]
    public class ImportGlossaryAction : AbstractAction
    {
        public ImportGlossaryAction()
        {
            Enabled = false;
            DeepLTranslationProviderClient.ApiKeyChanged +=
                (_, _) => Enabled = DeepLTranslationProviderClient.ApiKey != null;
        }

        protected override void Execute()
        {
            var glossaryImportWindowViewModel = new GlossariesWindowViewModel(new DeepLGlossaryClient(),
                new MessageService(), new GlossaryBrowserService(new OpenFileDialogWrapper()),
                new GlossaryReaderWriterService(new GlossaryReaderWriterFactory()));
            var glossaryImportWindow = new GlossariesWindow
            {
                DataContext = glossaryImportWindowViewModel
            };

            glossaryImportWindow.ShowDialog();
        }
    }
}