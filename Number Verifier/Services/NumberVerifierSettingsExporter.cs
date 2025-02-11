using Sdl.Community.NumberVerifier.DTOs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Sdl.Community.NumberVerifier.Services
{
    public class NumberVerifierSettingsExporter
    {
        public void ExportSettings(string filePath, NumberVerifierSettingsDTO settings)
        {
            if (settings == null)
                throw new ArgumentNullException(nameof(settings), "Settings cannot be null.");

            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(NumberVerifierSettingsDTO));
                using (StreamWriter writer = new StreamWriter(filePath))
                {
                    serializer.Serialize(writer, settings);
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Failed to export settings.", ex);
            }
        }
    }
}
