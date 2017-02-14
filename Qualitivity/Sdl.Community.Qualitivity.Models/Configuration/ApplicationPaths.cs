using System;
using System.IO;

namespace Sdl.Community.Structures.Configuration
{
    [Serializable]
    public class ApplicationPaths : ICloneable
    {
        public string ApplicationSettingsPath { get; set; }
       
        public string ApplicationTrackChangesPath { get; set; }
        public string ApplicationTrackChangesReportPath { get; set; }

        public string ApplicationMyDocumentsPath { get; set; }
        public string ApplicationMyDocumentsDatabasePath { get; set; }
        public string ApplicationMyDocumentsDatabaseSettingsPath { get; set; }
        public string ApplicationMyDocumentsDatabaseProjectsPath { get; set; }

        public string ApplicationBackupDatabasePath { get; set; }

        public ApplicationPaths()
        {
            ApplicationSettingsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Qualitivity");
            if (!Directory.Exists(ApplicationSettingsPath))
                Directory.CreateDirectory(ApplicationSettingsPath);
           

            ApplicationTrackChangesPath = Path.Combine(ApplicationSettingsPath, "Track.Changes");
            if (!Directory.Exists(ApplicationTrackChangesPath))
                Directory.CreateDirectory(ApplicationTrackChangesPath);

            ApplicationTrackChangesReportPath = Path.Combine(ApplicationTrackChangesPath, "Reports");
            if (!Directory.Exists(ApplicationTrackChangesReportPath))
                Directory.CreateDirectory(ApplicationTrackChangesReportPath);


            ApplicationMyDocumentsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Qualitivity");
            if (!Directory.Exists(ApplicationMyDocumentsPath))
                Directory.CreateDirectory(ApplicationMyDocumentsPath);
            ApplicationMyDocumentsDatabasePath = Path.Combine(ApplicationMyDocumentsPath, "Database");
            if (!Directory.Exists(ApplicationMyDocumentsDatabasePath))
                Directory.CreateDirectory(ApplicationMyDocumentsDatabasePath);
            ApplicationMyDocumentsDatabaseSettingsPath = Path.Combine(ApplicationMyDocumentsDatabasePath, "Settings.sqlite");
            ApplicationMyDocumentsDatabaseProjectsPath = Path.Combine(ApplicationMyDocumentsDatabasePath, "Projects.sqlite");


      

            ApplicationBackupDatabasePath = Path.Combine(ApplicationMyDocumentsDatabasePath, "Backups");
            if (!Directory.Exists(ApplicationBackupDatabasePath))
                Directory.CreateDirectory(ApplicationBackupDatabasePath);
        }

        public object Clone()
        {
            var applicationPaths = new ApplicationPaths
            {
                ApplicationSettingsPath = ApplicationSettingsPath,
                ApplicationTrackChangesPath = ApplicationTrackChangesPath,
                ApplicationTrackChangesReportPath = ApplicationTrackChangesReportPath,
                ApplicationMyDocumentsPath = ApplicationMyDocumentsPath,
                ApplicationMyDocumentsDatabasePath = ApplicationMyDocumentsDatabasePath,
                ApplicationBackupDatabasePath = ApplicationBackupDatabasePath
            };






            return applicationPaths;
        }
    }
}
