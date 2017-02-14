using System;
using System.Collections.Generic;
using Sdl.Community.Structures.DQF;
using Sdl.Community.Structures.iProperties;
using Sdl.Community.Structures.Profile;
using Sdl.Community.Structures.QualityMetrics;
using Sdl.Community.Structures.Rates;

namespace Sdl.Community.Structures.Configuration
{
    [Serializable]
    public class Settings : ICloneable
    {

        public string Guid { get; set; }

        public ApplicationPaths ApplicationPaths { get; set; }
        public GeneralSettings GeneralSettings { get; set; }
        public BackupSettings BackupSettings { get; set; }
        public TrackingSettings TrackingSettings { get; set; }
        public ViewSettings ViewSettings { get; set; }
        public DqfSettings DqfSettings { get; set; }
        public QualityMetricGroupSettings QualityMetricGroupSettings { get; set; }
        public QualityMetricGroup QualityMetricGroup { get; set; }

        public List<LanguageRateGroup> LanguageRateGroups { get; set; }
        public UserProfile UserProfile { get; set; }
        public List<CompanyProfile> CompanyProfiles { get; set; }

        public Settings()
        {
            Guid = string.Empty;

            ApplicationPaths = new ApplicationPaths();
            SettingsInitialization.Initialize_GeneralSettings(this);
            SettingsInitialization.Initialize_BackupSettings(this);
            SettingsInitialization.Initialize_TrackerSettings(this);
            SettingsInitialization.Initialize_ViewSettings(this);

            DqfSettings = new DqfSettings();
            LanguageRateGroups = new List<LanguageRateGroup>();
            UserProfile = new UserProfile();
            CompanyProfiles = new List<CompanyProfile>();
            QualityMetricGroupSettings = new QualityMetricGroupSettings();
            QualityMetricGroup = new QualityMetricGroup();
        }

        public GeneralProperty GetGeneralProperty(string name)
        {
            var property = GeneralSettings.GeneralProperties.Find(e => e.Name == name) ?? new GeneralProperty();
            return property;
        }
        public GeneralProperty GetTrackingProperty(string name)
        {
            var property = TrackingSettings.TrackingProperties.Find(e => e.Name == name) ?? new GeneralProperty();
            return property;
        }
        public GeneralProperty GetBackupProperty(string name)
        {
            var property = BackupSettings.BackupProperties.Find(e => e.Name == name) ?? new GeneralProperty();
            return property;
        }
        public ViewProperty GetViewProperty(string name)
        {
            var property = ViewSettings.ViewProperties.Find(e => e.Name == name) ?? new ViewProperty();
            return property;
        }

        public object Clone()
        {
            var settings = new Settings
            {
                Guid = Guid,
                ApplicationPaths = (ApplicationPaths)ApplicationPaths.Clone(),
                GeneralSettings = (GeneralSettings)GeneralSettings.Clone(),
                BackupSettings = (BackupSettings)BackupSettings.Clone(),
                DqfSettings = (DqfSettings)DqfSettings.Clone(),
                TrackingSettings = (TrackingSettings)TrackingSettings.Clone(),
                ViewSettings = (ViewSettings)ViewSettings.Clone(),
                UserProfile = (UserProfile)UserProfile.Clone(),
                QualityMetricGroupSettings = (QualityMetricGroupSettings)QualityMetricGroupSettings.Clone(),
                QualityMetricGroup = (QualityMetricGroup)QualityMetricGroup.Clone()
            };




            try
            {
                settings.CompanyProfiles = new List<CompanyProfile>();
                foreach (var cpi in CompanyProfiles)
                    settings.CompanyProfiles.Add((CompanyProfile)cpi.Clone());
            }
            catch (Exception ex)
            {
                throw new Exception("Clients: " + ex.Message);
            }


            try
            {
                settings.LanguageRateGroups = new List<LanguageRateGroup>();
                foreach (var priceGroup in LanguageRateGroups)
                    settings.LanguageRateGroups.Add((LanguageRateGroup)priceGroup.Clone());
            }
            catch (Exception ex)
            {
                throw new Exception("Rates: " + ex.Message);
            }


            return settings;

        }
    }
}
