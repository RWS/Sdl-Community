using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using Sdl.Community.PostEdit.Compare.Core;


namespace PostEdit.Compare
{


    [Serializable]
    public class SettingsCore : ICloneable
    {
        public SettingsCore()
        {
            ApplicationSettingsPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "PostEdit.Compare");


            if (!Directory.Exists(ApplicationSettingsPath))
                Directory.CreateDirectory(ApplicationSettingsPath);

            ApplicationSettingsFullPath = Path.Combine(ApplicationSettingsPath, "PostEdit.Compare.settings.xml");


            ComparePanelSettingsFullPath = Path.Combine(ApplicationSettingsPath, "PostEdit.Compare.Panel.settings.xml");
            ComparisonComparePanelSettingsFullPath = Path.Combine(ApplicationSettingsPath,
                "PostEdit.Comparison.Projects.Panel.settings.xml");
            EventsLogPanelSettingsFullPath = Path.Combine(ApplicationSettingsPath,
                "PostEdit.Events.Log.Panel.Settings.xml");

            ComparisonLogReportFullPath = Path.Combine(ApplicationSettingsPath, "PostEdit.Comparison.Log.Report.xml");


            AllowEndUserDocking = true;

            ShowEmptyFolders = true;
            ShowEqualFiles = true;
            ShowOrphanFilesLeft = true;
            ShowOrphanFilesRight = true;
            ShowDifferencesFilesLeft = true;
            ShowDifferencesFilesRight = true;

            ViewListViewColumnName = true;
            ViewListViewColumnType = false;
            ViewListViewColumnSize = true;
            ViewListViewColumnModified = true;

            ActivateFilters = true;


            FilterSettings = new List<Settings.FilterSetting>();
            ComparisonProjects = new List<Settings.ComparisonProject>();
            FolderViewerFoldersLeft = new List<string>();
            FolderViewerFoldersRight = new List<string>();
            PriceGroups = new List<Settings.PriceGroup>();
            ComparisonLogEntries = new List<Settings.ComparisonLogEntry>();

            FolderViewerFoldersMaxEntries = 15;
            ComparisonLogMaxEntries = 1000;

            AutomaciallyExpandComparisonFolders = false;


            EventsLogTrackCompare = true;
            EventsLogTrackReports = true;
            EventsLogTrackProjects = false;
            EventsLogTrackFiles = true;
            EventsLogTrackFilters = false;

            ReportsAutoSave = true;
            ReportsCreateMonthlySubFolders = true;
            ReportsAutoSaveFullPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                "PostEdit.Compare");
            ReportsAutoSaveFullPath = Path.Combine(ReportsAutoSaveFullPath, "Reports");

            if (!Directory.Exists(ReportsAutoSaveFullPath))
                Directory.CreateDirectory(ReportsAutoSaveFullPath);

            ReportViewerSettings = new Settings();



            LicenseBaseUrl = @"http://posteditcompare.bramito.net/";
            ProxyHostName = string.Empty;
            ProxyPort = string.Empty;
            ProxyUserName = string.Empty;
            ProxyPassword = string.Empty;

        }

        public object Clone()
        {
            return (SettingsCore)this.MemberwiseClone();
        }


        public string LicenseBaseUrl { get; set; }
        public string ProxyHostName { get; set; }
        public string ProxyPort { get; set; }
        public string ProxyUserName { get; set; }
        public string ProxyPassword { get; set; }


        public string ApplicationSettingsPath { get; set; }
        public string ApplicationSettingsFullPath { get; set; }

        public string ComparePanelSettingsFullPath { get; set; }
        public string ComparisonComparePanelSettingsFullPath { get; set; }
        public string EventsLogPanelSettingsFullPath { get; set; }


        public string ComparisonLogReportFullPath { get; set; }

        public bool ShowEmptyFolders { get; set; }
        public bool ShowEqualFiles { get; set; }
        public bool ShowOrphanFilesLeft { get; set; }
        public bool ShowOrphanFilesRight { get; set; }
        public bool ShowDifferencesFilesLeft { get; set; }
        public bool ShowDifferencesFilesRight { get; set; }




        public bool ViewListViewColumnName { get; set; }
        public bool ViewListViewColumnType { get; set; }
        public bool ViewListViewColumnSize { get; set; }
        public bool ViewListViewColumnModified { get; set; }



        public bool ActivateFilters { get; set; }

        public bool AllowEndUserDocking { get; set; }


        public List<Settings.FilterSetting> FilterSettings { get; set; }
        public List<Settings.ComparisonProject> ComparisonProjects { get; set; }
        public List<string> FolderViewerFoldersLeft { get; set; }
        public List<string> FolderViewerFoldersRight { get; set; }
        public List<Settings.PriceGroup> PriceGroups { get; set; }
        public List<Settings.ComparisonLogEntry> ComparisonLogEntries { get; set; }

        public int FolderViewerFoldersMaxEntries { get; set; }
        public int ComparisonLogMaxEntries { get; set; }


        public bool AutomaciallyExpandComparisonFolders { get; set; }



        public Settings ReportViewerSettings { get; set; }



        public bool EventsLogTrackCompare { get; set; }
        public bool EventsLogTrackReports { get; set; }
        public bool EventsLogTrackProjects { get; set; }
        public bool EventsLogTrackFiles { get; set; }
        public bool EventsLogTrackFilters { get; set; }


        public bool ReportsAutoSave { get; set; }
        public bool ReportsCreateMonthlySubFolders { get; set; }
        public string ReportsAutoSaveFullPath { get; set; }

    }










    public class SettingsSerializer
    {
        #region  |  settings serialization  |

        public static void SaveSettings(SettingsCore settings)
        {
            FileStream stream = null;
            try
            {
                var serializer = new XmlSerializer(typeof(SettingsCore));
                stream = new FileStream(settings.ApplicationSettingsFullPath, FileMode.Create, FileAccess.Write);
                serializer.Serialize(stream, settings);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (stream != null)
                    stream.Close();
            }
        }

        public static SettingsCore ReadSettings()
        {
            var settings = new SettingsCore();
            if (!File.Exists(settings.ApplicationSettingsFullPath))
                SaveSettings(settings);

            FileStream stream = null;
            try
            {
                var serializer = new XmlSerializer(typeof(SettingsCore));
                stream = new FileStream(settings.ApplicationSettingsFullPath, FileMode.Open);
                settings = (SettingsCore)serializer.Deserialize(stream) ?? new SettingsCore();

                if (settings.FilterSettings.Count != 0)
                    return settings;

                var filterSetting = new Settings.FilterSetting
                {
                    Name = "SDLXLIFF files",
                    IsDefault = true
                };
                filterSetting.FilterNamesInclude.Add("*\\.sdlxliff$");
                settings.FilterSettings.Add(filterSetting);

                return settings;
            }
            catch
            {
                try
                {
                    // try once to recover from the exception by creating
                    // a new settings file.
                    if (File.Exists(settings.ApplicationSettingsFullPath))
                        File.Delete(settings.ApplicationSettingsFullPath);
                    return ReadSettings();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            finally
            {
                if (stream != null)
                    stream.Close();
            }
        }

        #endregion
    }
}
