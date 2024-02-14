using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TMProvider
{
    public static class AppData
    {
        public static string AppName = "memoQ Plugin";
        public static int MainVersion = 1;
        public static int MinorVersion = 0;


  

        public static string KilgraySupportURL = "mailto:support@memoq.com";

        public static string GetStringVersion()
        {
            return MainVersion + "." + MinorVersion;
        }

        
        public static byte[] RijndaelKey = new byte[] {1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16 };
        public static byte[] RijndaelIV = new byte[] { 16, 15, 14, 13, 12, 11, 10, 9, 8, 7, 6, 5, 4, 3, 2, 1 };

        public static string ServerURLCommon = "/memoqserverhttpapi/v1";
        public static string ServerURLLogin = "/auth/login";
        public static string ServerURLLogout = "/auth/logout";
        public static string ServerURLListTMs = "/tms";
        public static string ServerURLTMInfo = "/tms/{0}";
        public static string ServerURLGetTMEntry = "/tms/{0}/entries/{1}";
        public static string ServerURLUpdateEntry = "/tms/{0}/entries/{1}/update";
        public static string ServerURLAddEntry = "/tms/{0}/entries/create";
        public static string ServerURLDeleteEntry = "/tms/{0}/entries/{1}/delete";
        public static string ServerURLConcordance = "/tms/{0}/concordance";
        public static string ServerURLLookup = "/tms/{0}/lookupsegments";


        public static string KeyFieldName = "Entry ID";
        public static string TMGuidFieldName = "TM Guid";
        public static string TMNameFieldName = "TM Name";
            
        public static string LogFilePath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Kilgray\\SDLPlugin\\memoQTMPluginLog.txt";
        public static string HelpFilePath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\memoQ_SDLTradosTMPlugin_GettingStarted_1_0.pdf";
        public static string ConfigPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Kilgray\\SDLPlugin\\memoQTMPluginSettings.xml";

    }
}
