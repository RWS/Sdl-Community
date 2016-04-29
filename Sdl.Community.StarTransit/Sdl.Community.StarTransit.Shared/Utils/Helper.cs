using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sdl.Community.StarTransit.Shared.Utils
{
    public static class Helper
    {
        public static void DeleteFilesFromTemp(IEnumerable<string> files)
        {
            
                foreach (var file in files)
                {
                    File.Delete(file);
                }

            
         
        }

        public static void DeleteAnyFiles(IEnumerable<string> filesList)
        {

            var tempPath = Path.GetTempPath();
            var tempFiles = Directory.GetFiles(tempPath).ToList();
            foreach (var file in filesList)
            {
                var fileName = file.Substring(0, file.LastIndexOf('.'));
                var matchFileFromTemp = tempFiles.FirstOrDefault(m => m.Contains(fileName));
                if (matchFileFromTemp != null)
                {
                    File.Delete(matchFileFromTemp);
                }


            }
        }
    }
}
