using PostEdit.Compare;
using Sdl.Community.PostEdit.Compare.Core;
using System;
using System.IO;
using System.Xml.Serialization;

namespace Sdl.Community.PostEdit.Compare
{
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