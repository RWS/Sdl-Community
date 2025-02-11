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
    public class NumberVerifierSettingsImporter
    {
        public NumberVerifierSettingsDTO ImportSettings(string filePath)
        {
            try
            {
                using (var reader = new StreamReader(filePath))
                {
                    var serializer = new XmlSerializer(typeof(NumberVerifierSettingsDTO));
                    return serializer.Deserialize(reader) as NumberVerifierSettingsDTO;
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Failed to import settings.", ex);
            }
        }
    }

}
