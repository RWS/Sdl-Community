using Sdl.Community.DeepLMTProvider.Client;
using Sdl.Community.DeepLMTProvider.Helpers;
using Sdl.Community.DeepLMTProvider.Helpers.GlossaryReadersWriters;
using Sdl.Community.DeepLMTProvider.UI;
using Sdl.Community.DeepLMTProvider.ViewModel;
using System;
using System.IO;
using System.Security.AccessControl;

namespace Sdl.Community.DeepLMTProvider.Service
{
    public static class GlossaryBackupService
    {
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

        public static void ViewModel_ManageGlossaries()
        {
            var glossaryReaderWriterFactory = new GlossaryReaderWriterFactory();
            var messageService = new MessageService();
            var glossaryImportExportService = new GlossaryImportExportService(new DialogWrapper());
            var glossaryReaderWriterService = new GlossaryReaderWriterService(glossaryReaderWriterFactory);
            var deepLGlossaryClient = new DeepLGlossaryClient();

            var glossariesWindowViewModel =
                new GlossariesWindowViewModel(
                    deepLGlossaryClient,
                    messageService,
                    glossaryImportExportService,
                    glossaryReaderWriterService,
                    new ProcessStarter(),
                    new EditGlossaryService(glossaryReaderWriterService, glossaryImportExportService, messageService));

            var (success, glossaryWriter, _) = glossaryReaderWriterFactory.CreateGlossaryWriter(GlossaryReaderWriterService.Format.CSV);
            if (!success) messageService.ShowWarning("Backup service could not be initialized.\nUse glossary manager carefully!", "Manage glossaries");

            var backupPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "Trados AppStore",
                $@"DeepLLogs\Backup");

            var backupPathInfo
                = new DirectoryInfo(backupPath);

            if (!backupPathInfo.Exists) backupPathInfo.Create();

            if (!UserHasWritePermission(backupPathInfo)) messageService.ShowWarning($"User not authorized to write to backup location {backupPath}");

            glossariesWindowViewModel.ShouldBackUp += glossary =>
            {
                var backupDirectory = $@"{backupPath}\{GetDateTimeToString(DateTime.Now)}";
                Directory.CreateDirectory(backupDirectory);

                glossaryWriter.WriteGlossary(glossary,
                    $@"{backupPath}\{GetDateTimeToString(DateTime.Now)}\{glossary.Name}_{glossary.SourceLanguage}-{glossary.TargetLanguage}.csv");
            };

            var glossariesWindow = new GlossariesWindow { DataContext = glossariesWindowViewModel };
            glossariesWindow.ShowDialog();
        }
    }
}