namespace Sdl.Utilities.SplitSDLXLIFF.Lib
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.IO;
    using System.Security.Cryptography;

    public static class FileHelper
    {
        public static string SHA1HashFile(string fPath)
        {
            if (!File.Exists(fPath))
                return "";

            string sHash = "";
            using (StreamReader sr = new StreamReader(fPath))
            {
                //SHA1Managed sha1h = new SHA1Managed();
                using (SHA1CryptoServiceProvider sha1h = new SHA1CryptoServiceProvider())
                {
                    sHash = BitConverter.ToString(sha1h.ComputeHash(sr.BaseStream));
                }
            }
            return sHash;
        }

        public static void CheckPath(string fileName, bool isOverride)
        {
            try
            {
                if (!Directory.Exists(Path.GetDirectoryName(fileName)))
                    Directory.CreateDirectory(Path.GetDirectoryName(fileName));
                if (isOverride && File.Exists(fileName))
                    File.Delete(fileName);
            }
            catch (Exception ex)
            {
                throw (ex);
            }
        }
    }
}
