using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;
using Sdl.Community.StarTransit.Shared.Models;


namespace Sdl.Community.StarTransit.Services
{
    public class PackageService
    {
        private readonly Dictionary<string, string> _dictionaryPropetries = new Dictionary<string, string>(); 
        private  Dictionary<string,Dictionary<string,string>> _pluginDictionary = new Dictionary<string, Dictionary<string, string>>();

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
            //  var pluginDictionary =  new Dictionary<string, Dictionary<string, string>>();
            var keyProperty = string.Empty;


            using (var reader = new StreamReader(filePath))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {

                    try
                    {
                        if (line.Contains("["))
                        {
                            var valuesDictionaries = new Dictionary<string, string>();
                            if (keyProperty != string.Empty && _dictionaryPropetries.Count != 0)
                            {
                                foreach (var property in _dictionaryPropetries)
                                {
                                    valuesDictionaries.Add(property.Key, property.Value);
                                }
                                _pluginDictionary.Add(keyProperty, valuesDictionaries);
                                _dictionaryPropetries.Clear();
                            }

                            var firstPosition = line.IndexOf("[", StringComparison.Ordinal) + 1;
                            var lastPosition = line.IndexOf("]", StringComparison.Ordinal) - 1;
                            keyProperty = line.Substring(firstPosition, lastPosition);

                        }
                        else
                        {
                            var properties = line.Split('=');
                            _dictionaryPropetries.Add(properties[0], properties[1]);


                        }

                    }
                    catch (Exception ex)
                    {
                    }
                }
            }

            var packageModel = CreateModel();


            try
            {
                File.Delete(filePath);
            }
            catch (Exception exception)
            {
            }

            return packageModel;
        }

        private PackageModel CreateModel()
        {
            var model = new PackageModel();
            if (_pluginDictionary.ContainsKey("Admin"))
            {
                var propertiesDictionary = _pluginDictionary["Admin"];
                foreach (var key in propertiesDictionary.Keys)
                {
                    if (key == "ProjectName")
                    {
                        model.Name = propertiesDictionary["ProjectName"];
                    }
                }
            }

            if (_pluginDictionary.ContainsKey("Languages"))
            {
                var propertiesDictionary = _pluginDictionary["Languages"];
                foreach (var key in propertiesDictionary.Keys)
                {
                    if (key == "SourceLanguage")
                    {
                        var languageCode =int.Parse(propertiesDictionary["SourceLanguage"]);
                        model.SourceLanguage = Language(languageCode);
                    }
                    if (key == "TargetLanguage")
                    {
                        var languageCode = int.Parse(propertiesDictionary["TargetLanguage"]);
                        model.TargetLanguage = Language(languageCode);
                    }
                }
            }
            return model;

        }

        private CultureInfo Language(int languageCode)
        {
            return new CultureInfo(languageCode);
        }


    }
}
