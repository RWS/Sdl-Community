using System;
using System.IO;
using System.Xml.Serialization;

namespace Sdl.Community.Studio.Time.Tracker.Structures
{
    public class SettingsSerializer
    {
        
        public static void SaveSettings(Settings settings)
        {
            FileStream stream = null;
            try
            {
                var serializer = new XmlSerializer(typeof(Settings));
                stream = new FileStream(settings.ApplicationSettingsFullPath, FileMode.Create, FileAccess.Write);
                serializer.Serialize(stream, settings);
            }
            finally
            {
                if (stream != null)
                    stream.Close();
            }
        }
        public static Settings ReadSettings()
        {
            var settings = new Settings();
            if (!File.Exists(settings.ApplicationSettingsFullPath))
            {
                SaveSettings(settings);
            }

            FileStream stream = null;
            try
            {
                var serializer = new XmlSerializer(typeof(Settings));
                stream = new FileStream(settings.ApplicationSettingsFullPath, FileMode.Open);
                settings = (Settings)serializer.Deserialize(stream);

                if (settings == null)
                    settings = new Settings();

                #region  |  set up some default setttings  |


                if (settings.ActivitiesTypes.Count <= 0)
                {
                    var activityType = new ActivityType
                    {
                        Id = Guid.NewGuid().ToString(),
                        Name = "Translation",
                        Description = "Translation Activities",
                        Currency = settings.DefaultCurrency
                    };

                    settings.ActivitiesTypes.Add(activityType);
                    activityType = new ActivityType
                    {
                        Id = Guid.NewGuid().ToString(),
                        Name = "Review",
                        Description = "Revision Activities",
                        Currency = settings.DefaultCurrency
                    };

                    settings.ActivitiesTypes.Add(activityType);
                    activityType = new ActivityType
                    {
                        Id = Guid.NewGuid().ToString(),
                        Name = "SignOff",
                        Description = "Sign-Off Activities",
                        Currency = settings.DefaultCurrency
                    };

                    settings.ActivitiesTypes.Add(activityType);
                    
                }


                if (settings.BackupFolder.Trim() == string.Empty)
                {
                    var myDocumentsFolderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Studio.Time.Tracker");
                    if (!Directory.Exists(myDocumentsFolderPath))
                        Directory.CreateDirectory(myDocumentsFolderPath);
                    settings.BackupFolder = Path.Combine(myDocumentsFolderPath, "Backups");
                    if (!Directory.Exists(settings.BackupFolder))
                        Directory.CreateDirectory(settings.BackupFolder);

                    settings.BackupLastDate = DateTime.Now;
                    settings.BackupEvery = 1;
                    settings.BackupEveryType = 1;//weeks
                }

                #endregion


                

                return settings;
            }
            finally
            {
                if (stream != null)
                    stream.Close();


                #region  |  check/create backup  |

                try
                {
                    if (settings != null)
                    {
                        var backupLastDate = settings.BackupLastDate;
                        backupLastDate = settings.BackupEveryType == 0 
                            ? backupLastDate.AddDays(settings.BackupEvery) 
                            : backupLastDate.AddDays(settings.BackupEvery * 7);


                        if (backupLastDate < DateTime.Now)
                        {
                            if (settings.BackupFolder.Trim() != string.Empty && Directory.Exists(settings.BackupFolder))
                            {
                                var backupFolderYear = Path.Combine(settings.BackupFolder.Trim(), DateTime.Now.Year.ToString());
                                if (!Directory.Exists(backupFolderYear))
                                    Directory.CreateDirectory(backupFolderYear);

                                var backupFolderMonth = Path.Combine(backupFolderYear, DateTime.Now.Month.ToString().PadLeft(2, '0'));
                                if (!Directory.Exists(backupFolderMonth))
                                    Directory.CreateDirectory(backupFolderMonth);

                                var newFilePath = Path.Combine(backupFolderMonth, @"Studio.Time.Tracker.settings."
                                                                                  + DateTime.Now.Year + "." + DateTime.Now.Month.ToString().PadLeft(2, '0') + "." + DateTime.Now.Day.ToString().PadLeft(2, '0') + ".xml");

                                settings.BackupLastDate = DateTime.Now;

                                File.Copy(settings.ApplicationSettingsFullPath, newFilePath, true);

                                SaveSettings(settings);
                            }
                            else
                            {
                                throw new Exception("Unable to locate the backup folder: '" + settings.BackupFolder.Trim() + "' ");
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Error creating backup!\r\n\r\n" + ex.Message);
                }
                #endregion
            }
        }

    }
}
