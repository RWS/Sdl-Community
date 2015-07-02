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
            String path = String.Format(@"{0}\Office Templates",
                                        Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));
            String[] files = Directory.GetFiles(path, String.Format("*.{0}*", _extention)).Where(file => !Path.GetFileName(file).StartsWith("~")).ToArray();

            List<KeyValuePair<String, String>> fileList = files.Select(
                file =>
                new KeyValuePair<String, String>(file, Path.GetFileName(file).Replace(Path.GetExtension(file), "")))
                 .ToList();
//            fileList.Insert(0, new KeyValuePair<string, string>(String.Empty, "<No template>"));
            return fileList;
        }
    }
}