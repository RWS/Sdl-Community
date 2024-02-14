using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace TMProvider
{
    public class Log
    {

        /// <summary>
        /// Writes the string into the log file. Always opens and closes the file. NOT thread safe.
        /// </summary>
        /// <param name="text"></param>
        public static void WriteToLog(string text)
        {

            if (!Directory.Exists(Path.GetDirectoryName(AppData.LogFilePath))) return;
            try
            {
                using (StreamWriter sw = File.AppendText(AppData.LogFilePath))
                {
                    sw.WriteLine(DateTime.Now.ToString("yyyy.MM.dd HH:mm:ss") + " " + text);
                    sw.WriteLine();
                }
            }
            catch { }
        }

        /// <summary>
        /// Deletes the log file if its size is above 5MB.
        /// </summary>
        /// <param name="text"></param>
        public static void ClearLogFile()
        {
            if (!File.Exists(AppData.LogFilePath)) return;
            try
            {
                FileInfo fi = new FileInfo(AppData.LogFilePath);
                if (fi.Length > 5000000) File.Delete(AppData.LogFilePath);
            }
            catch
            { 
                
            }
        }
    }
}
