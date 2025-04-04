using Newtonsoft.Json;
using System;
using System.IO;

namespace Sdl.Community.PostEdit.Compare.Core.Helper
{
    public static class ErrorHandlerSettingsSerializer
    {
        private static string ErrorHandlerSettingsFile => Path.Combine(SharedStrings.PostEditCompareSettingsFolder, "ErrorHandlerSettings.json");

        

        public static ErrorHandlerSettings ReadSettings()
        {
            if (!File.Exists(ErrorHandlerSettingsFile))
                File.WriteAllText(ErrorHandlerSettingsFile, JsonConvert.SerializeObject(new ErrorHandlerSettings()));

            var settings = File.ReadAllText(ErrorHandlerSettingsFile);
            return JsonConvert.DeserializeObject<ErrorHandlerSettings>(settings);
        }

        public static void WriteSettings(ErrorHandlerSettings settings)
        {
            File.WriteAllText(ErrorHandlerSettingsFile, JsonConvert.SerializeObject(settings));
        }
    }

    public class ErrorHandlerSettings
    {
        public bool ExplicitErrors { get; set; }
    }
}