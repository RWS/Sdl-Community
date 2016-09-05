using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Sdl.Community.InvoiceAndQuotes.Templates
{
    public class OfficeTemplates : IOfficeTemplates
    {
        private String _extention;
        public OfficeTemplates(String extention)
        {
            _extention = extention;
        }
        public List<KeyValuePair<String, String>> GetAllTemplates()
        {

            var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var invoiceFolderPath = @"SDL Community\Invoice\Office Templates";
            var path = Path.Combine(appDataPath, invoiceFolderPath);
           
            var files = Directory.GetFiles(path, String.Format("*.{0}*", _extention)).Where(file => !Path.GetFileName(file).StartsWith("~")).ToArray();

            List<KeyValuePair<String, String>> fileList = files.Select(
                file =>
                new KeyValuePair<String, String>(file, Path.GetFileName(file).Replace(Path.GetExtension(file), "")))
                 .ToList();
//            fileList.Insert(0, new KeyValuePair<string, string>(String.Empty, "<No template>"));
            return fileList;
        }
    }
}