using Sdl.Community.ApplyTMTemplate.Models;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Sdl.Community.ApplyTMTemplate.Services
{
    public class ApplyTMSettingsManager
    {
        public string CachedLocation => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Trados", "Trados Studio\\Studio18\\default.applyTMsettings");

        public void SaveSettings(string location, ApplyTMSettings applyTMsettings)
        {
            XmlSerializer serializer = new (typeof(ApplyTMSettings));
            using StreamWriter writer = new(location);
            serializer.Serialize(writer, applyTMsettings);
        }

        public ApplyTMSettings LoadSettings(string location)
        {
            if (!File.Exists(location)) return null;

            try
            {
                ApplyTMSettings settings;
                XmlSerializer serializer = new XmlSerializer(typeof(ApplyTMSettings));
                using (StreamReader reader = new StreamReader(location))
                {   
                    settings = (ApplyTMSettings)serializer.Deserialize(reader);
                }

                return settings;
            }
            catch (Exception _)
            {
                return null; 
            }
        }

        public async Task SaveCachedSettings(ApplyTMSettings applyTMsettings)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(ApplyTMSettings));
            using StreamWriter writer = new StreamWriter(CachedLocation);
            serializer.Serialize(writer, applyTMsettings);
        }
    }
}
