using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Sdl.Community.StarTransit.Shared.Models;


namespace Sdl.Community.StarTransit.Services
{
    public class PackageService
    {
        public PackageModel OpenPackage(string packagePath)
        {
            
            var entryName = string.Empty;
            var pathToExtract = packagePath.Substring(0,packagePath.LastIndexOf(@"\", StringComparison.Ordinal)+1);
            using (ZipArchive archive = ZipFile.OpenRead(packagePath))
            {
                foreach (ZipArchiveEntry entry in archive.Entries)
                {
                    if (entry.FullName.EndsWith(".PRJ", StringComparison.OrdinalIgnoreCase))
                    {

                        try
                        {
                            entry.ExtractToFile(Path.Combine(pathToExtract, entry.FullName));
                        }
                        catch (Exception e)
                        {
                        }
                        entryName = entry.FullName;
                    }

                 }
              

            }

            return ReadPackage(pathToExtract, entryName);
        }

        private PackageModel ReadPackage(string path, string fileName)
        {
            var filePath = path + @"\" + fileName;
           // var pluginDictionary = new Dictionary<string, string>();
           var pluginDictionary1 =  new Dictionary<string, Dictionary<string, string>>();
            var keyProperty = string.Empty;
            var dictionaryPropetries = new Dictionary<string, string>();
            using (var reader = new StreamReader(filePath))
            {

                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    
                    
                    try
                    {
                        if (line.Contains("["))
                        {
                            if (keyProperty != string.Empty && dictionaryPropetries.Count != 0)
                            {
                                pluginDictionary1.Add(keyProperty, dictionaryPropetries);
                                dictionaryPropetries.Clear();
                            }
                            
                            var firstPosition = line.IndexOf("[", StringComparison.Ordinal) + 1;
                            var lastPosition = line.IndexOf("]", StringComparison.Ordinal)-1;
                            keyProperty = line.Substring(firstPosition, lastPosition);
                           // pluginDictionary1.Add(keyProperty, null);
                        }
                        else
                        {
                            var properties = line.Split('=');
                            dictionaryPropetries.Add(properties[0],properties[1]);
                            

                        }
                       
                       // 
                        //var properties = line.Split('=');
                        //pluginDictionary.Add(properties[0], properties[1]);
                    }
                    catch (Exception ex)
                    {
                    }
                }
            }

            var packageModel = new PackageModel();
            //if (pluginDictionary.ContainsKey("ProjectName"))
            //{
            //    packageModel.Name = pluginDictionary["ProjectName"];
            //}

            if (pluginDictionary1.ContainsKey("Admin"))
            {
                var propertiesDictionary = pluginDictionary1["Admin"];
                foreach (var key in propertiesDictionary.Keys)
                {
                    if (key == "ProjectName")
                    {
                        packageModel.Name = propertiesDictionary["ProjectName"];
                    }
                }
            }

            try
            {
                File.Delete(filePath);
            }
            catch (Exception exception)
            {
            }

            return packageModel;
        }

    }
}
