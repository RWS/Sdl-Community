using System;
using System.Collections.Generic;
using System.IO;

namespace Sdl.Community.Studio.Time.Tracker.Structures
{
    [Serializable]
    public class Settings: ICloneable
    {


       

        public string ApplicationSettingsPath { get; set; }
        public string ApplicationSettingsFullPath { get; set; }

        public UserProfileInfo User { get; set; }
        public List<ClientProfileInfo> Clients { get; set; }

        public List<ActivityType> ActivitiesTypes { get; set; }
        public List<TrackerProject> TrackerProjects { get; set; }


        public string DefaultCurrency { get; set; }
        public string DefaultFilterProjectStatus { get; set; }//In progress, Completed
        public string DefaultFilterGroupBy { get; set; }//Client name, Project name, Date (year/month)


        public DateTime BackupLastDate { get; set; }
        public int BackupEvery { get; set; }
        public int BackupEveryType { get; set; } //0=days; 1=weeks
        public string BackupFolder { get; set; }
        
        public bool TrackerConfirmActivities { get; set; }


        public Settings()
        {
            ApplicationSettingsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Studio.Time.Tracker");
            if (!Directory.Exists(ApplicationSettingsPath))
                Directory.CreateDirectory(ApplicationSettingsPath);
            ApplicationSettingsFullPath = Path.Combine(ApplicationSettingsPath, "Studio.Time.Tracker.settings.xml");


            var myDocumentsFolderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Studio.Time.Tracker");
            if (!Directory.Exists(myDocumentsFolderPath))
                Directory.CreateDirectory(myDocumentsFolderPath);
            BackupFolder = Path.Combine(myDocumentsFolderPath, "Backups");
            if (!Directory.Exists(BackupFolder))
                Directory.CreateDirectory(BackupFolder);

            BackupLastDate = DateTime.Now;
            BackupEvery = 1;
            BackupEveryType = 1;//weeks
           

            User = new UserProfileInfo();
            ActivitiesTypes = new List<ActivityType>();
            Clients = new List<ClientProfileInfo>();

            
            TrackerProjects = new List<TrackerProject>();

            DefaultCurrency = "EUR";
            DefaultFilterProjectStatus = "Show all projects";
            DefaultFilterGroupBy = "Client name";

            TrackerConfirmActivities = true;
        }

        public object Clone()
        {
            var settings = new Settings
            {
                BackupEvery = BackupEvery,
                BackupEveryType = BackupEveryType,
                BackupFolder = BackupFolder,
                BackupLastDate = BackupLastDate,
                ApplicationSettingsFullPath = ApplicationSettingsFullPath,
                ApplicationSettingsPath = ApplicationSettingsPath,
                User = User,
                Clients = new List<ClientProfileInfo>()
            };





            foreach (var cpi in Clients)
                settings.Clients.Add((ClientProfileInfo)cpi.Clone());


            settings.ActivitiesTypes = new List<ActivityType>();
            foreach (var a in ActivitiesTypes)
                settings.ActivitiesTypes.Add((ActivityType)a.Clone());

            settings.TrackerProjects = new List<TrackerProject>();
            foreach (var tp in TrackerProjects)
                settings.TrackerProjects.Add((TrackerProject)tp.Clone());

            settings.DefaultCurrency = DefaultCurrency;
            settings.DefaultFilterProjectStatus = DefaultFilterProjectStatus;
            settings.DefaultFilterGroupBy = DefaultFilterGroupBy;


            settings.TrackerConfirmActivities = TrackerConfirmActivities;

            return settings;


        }
    }
}
