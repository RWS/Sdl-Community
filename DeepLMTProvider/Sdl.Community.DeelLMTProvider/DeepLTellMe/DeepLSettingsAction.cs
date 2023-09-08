using Newtonsoft.Json;
using Sdl.Community.DeepLMTProvider.Client;
using Sdl.Community.DeepLMTProvider.Helpers;
using Sdl.Community.DeepLMTProvider.Helpers.GlossaryReadersWriters;
using Sdl.Community.DeepLMTProvider.Model;
using Sdl.Community.DeepLMTProvider.Service;
using Sdl.Community.DeepLMTProvider.UI;
using Sdl.Community.DeepLMTProvider.ViewModel;
using Sdl.TellMe.ProviderApi;
using Sdl.TranslationStudioAutomation.IntegrationApi;
using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace Sdl.Community.DeepLMTProvider.DeepLTellMe
{
    public class DeepLSettingsAction : AbstractTellMeAction
    {
        public DeepLSettingsAction()
        {
            Name = "DeepL MT Provider options";
        }

        public override string Category => "DeepL results";

        public override Icon Icon => PluginResources.Settings;

        public override bool IsAvailable => true;

        private DeepLWindowViewModel ViewModel { get; set; }

        public override void Execute()
        {
            var currentProject = SdlTradosStudio.Application.GetController<ProjectsController>().CurrentProject;

            if (currentProject == null)
            {
                MessageBox.Show(@"No project is set as active");
            }
            else
            {
                //TODO: Don't forget to fix this
                var settings = currentProject.GetTranslationProviderConfiguration();

                if (!settings.Entries.Any(entry =>
                    entry.MainTranslationProvider.Uri.OriginalString.Contains("deepltranslationprovider")))
                {
                    MessageBox.Show(
                        @"DeepL is not set on this project\nPlease set it in project settings before using TellMe to access it");
                }
                else
                {
                    var translationProvider = settings.Entries.FirstOrDefault(entry =>
                        entry.MainTranslationProvider.Uri.OriginalString.Contains("deepltranslationprovider"));
                    if (translationProvider != null)
                    {
                        var uri = translationProvider.MainTranslationProvider.Uri;
                        var state = translationProvider.MainTranslationProvider.State;
                        var options = new DeepLTranslationOptions(uri, state);

                        ViewModel = new DeepLWindowViewModel(options, new DeepLGlossaryClient(), new MessageService());
                        ViewModel.ManageGlossaries += ViewModel_ManageGlossaries;

                        var dialog = new DeepLWindow(ViewModel);

                        dialog.ShowDialog();

                        if (dialog.DialogResult.HasValue && dialog.DialogResult.Value)
                        {
                            translationProvider.MainTranslationProvider.Uri = options.Uri;
                            translationProvider.MainTranslationProvider.State = JsonConvert.SerializeObject(options);
                            currentProject.UpdateTranslationProviderConfiguration(settings);
                        }
                    }
                }
            }
        }

        private static string GetDateTimeToString(DateTime dateTime)
        {
            var value = dateTime.Year +
                        dateTime.Month.ToString().PadLeft(2, '0') +
                        dateTime.Day.ToString().PadLeft(2, '0') +
                        "-" +
                        dateTime.Hour.ToString().PadLeft(2, '0') +
                        dateTime.Minute.ToString().PadLeft(2, '0') +
                        dateTime.Second.ToString().PadLeft(2, '0');
            return value;
        }

        private static bool UserHasWritePermission(DirectoryInfo directoryInfo)
        {
            try
            {
                // Attempt to create a temporary file in the directory
                string tempFilePath = Path.Combine(directoryInfo.FullName, Guid.NewGuid().ToString("N") + ".tmp");
                using (FileStream fs = File.Create(tempFilePath)) { }

                // Delete the temporary file
                File.Delete(tempFilePath);

                return true; // User has write permission
            }
            catch (UnauthorizedAccessException)
            {
                return false; // User does not have write permission
            }
        }

        private void ViewModel_ManageGlossaries()
        {
            var glossaryReaderWriterFactory = new GlossaryReaderWriterFactory();
            var messageService = new MessageService();

            var glossariesWindowViewModel = new GlossariesWindowViewModel(new DeepLGlossaryClient(),
                messageService, new GlossaryImportExportService(new DialogWrapper()),
                new GlossaryReaderWriterService(glossaryReaderWriterFactory), new ProcessStarter(),
                new EditGlossaryService());

            var (success, glossaryWriter, _) = glossaryReaderWriterFactory.CreateGlossaryWriter(GlossaryReaderWriterService.Format.CSV);
            if (!success) messageService.ShowWarning("Backup service could not be initialized.\nUse glossary manager carefully!", "Manage glossaries");

            var backupPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "Trados AppStore",
                $@"DeepLLogs\Backup");

            if (!UserHasWritePermission(new DirectoryInfo(backupPath))) messageService.ShowWarning($"User not authorized to write to backup location {backupPath}");

            glossariesWindowViewModel.ShouldBackUp += glossary =>
            {
                var backupDirectory = $@"{backupPath}\{GetDateTimeToString(DateTime.Now)}";
                Directory.CreateDirectory(backupDirectory);

                glossaryWriter.WriteGlossary(glossary,
                    $@"{backupPath}\{GetDateTimeToString(DateTime.Now)}\{glossary.Name}_{glossary.SourceLanguage}-{glossary.TargetLanguage}.csv");
            };

            var glossariesWindow = new GlossariesWindow { DataContext = glossariesWindowViewModel };
            glossariesWindow.ShowDialog();

            ViewModel.LoadLanguagePairSettings();
        }
    }
}