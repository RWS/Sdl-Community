using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sdl.Utilities.SplitSDLXLIFF.Lib;
using System.IO;

namespace Sdl.Utilities.SplitSDLXLIFF
{
    public class AppOptions
    {
        public static bool isMerge = false;

        /// <summary>
        /// string - file name,
        /// bool - separate file (true), project file (false)
        /// </summary>
        public static Dictionary<string, bool> splitInFiles = new Dictionary<string, bool>();

        public static string splitOutPath = "";
        public static SplitOptions splitOpts = new SplitOptions();
        public static string segmentIDs = "";

        public static string mergeOrigFile = "";
        public static string mergeInPath = "";
        public static string mergeInfoFile = "";

        public static void RestoreOptions()
        {
            isMerge = false;
            splitInFiles = new Dictionary<string, bool>();
            splitOpts = new SplitOptions();
            splitOutPath = segmentIDs = mergeOrigFile =
                mergeInPath = mergeInfoFile = string.Empty;
        }
    }

    public class AppSettingsFile
    {
        public static void SaveSettings(List<string> data)
        {
            string settsFile = GetSettingsPath();

            try
            {
                if (File.Exists(settsFile))
                    File.Delete(settsFile);

                using (StreamWriter sWriter = new StreamWriter(settsFile))
                {
                    foreach (string item in data)
                        sWriter.WriteLine(item);
                    sWriter.Close();
                }
            }
            catch
            { }
        }

        public static List<string> LoadSettings()
        {
            List<string> data = new List<string>();

            string settsFile = GetSettingsPath();
            if (File.Exists(GetSettingsPath()))
            {
                using (StreamReader sReader = new StreamReader(settsFile))
                {
                    while (!sReader.EndOfStream)
                        data.Add(sReader.ReadLine());
                }
            }

            return data;
        }

        private static string GetSettingsPath()
        {
            string generatorFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\SDL\\OpenExchange\\SplitSDLXLIFF";
            if (!Directory.Exists(generatorFolder))
            {
                Directory.CreateDirectory(generatorFolder);
            }

            return generatorFolder + "\\settings.txt";
        }
    }
}
