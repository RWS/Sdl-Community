using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using Sdl.Community.StarTransit.Shared.Models;
using Sdl.Community.StarTransit.Shared.Utils;

namespace Sdl.Community.StarTransit.Shared.Services
{
    public class PackageService
    {
        private readonly Dictionary<string, string> _dictionaryPropetries = new Dictionary<string, string>(); 
        private  Dictionary<string,Dictionary<string,string>> _pluginDictionary = new Dictionary<string, Dictionary<string, string>>();
        private PackageModel _package = new PackageModel();
        private List<string> _fileNameList = new List<string>();
        private const char LanguageTargetSeparator = '|';

        public async Task<PackageModel> OpenPackage(string packagePath,string pathToTempFolder)
        {

            var entryName = string.Empty;
          

            using (var archive = ZipFile.OpenRead(packagePath))
            {
                foreach (ZipArchiveEntry entry in archive.Entries)
                {
                    var subdirectoryPath = Path.GetDirectoryName(entry.FullName);
                    if (!Directory.Exists(Path.Combine(pathToTempFolder, subdirectoryPath)))
                    {
                        Directory.CreateDirectory(Path.Combine(pathToTempFolder, subdirectoryPath));
                    }
                    entry.ExtractToFile(Path.Combine(pathToTempFolder, entry.FullName));

                    if (entry.FullName.EndsWith(".PRJ", StringComparison.OrdinalIgnoreCase))
                    {
                        entryName = entry.FullName;
                    }

                }
            }

            return await ReadProjectMetadata(pathToTempFolder, entryName, packagePath);
        }
        
        private async Task<PackageModel> ReadProjectMetadata(string pathToTempFolder, string fileName,string packagePath)
        {
            var filePath = Path.Combine(pathToTempFolder, fileName);
            var keyProperty = string.Empty;


            using (var reader = new StreamReader(filePath))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
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
            }

            var packageModel = await CreateModel(pathToTempFolder);



               // File.Delete(filePath);
          

            _package = packageModel;
            return packageModel;
        }

        private async Task<PackageModel> CreateModel(string pathToTempFolder)
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
                            var languageCode = int.Parse(propertiesDictionary["SourceLanguage"]);
                            model.SourceLanguage = Language(languageCode);
                        }
                        if (key == "TargetLanguages")
                        {
                            //we assume languages code are separated by "|"
                            var languages = propertiesDictionary["TargetLanguages"].Split(LanguageTargetSeparator);
                            var targetLanguagesList = new List<CultureInfo>();
                            foreach (var language in languages)
                            {
                                var languageCode = int.Parse(language);
                                var cultureInfo = Language(languageCode);
                                targetLanguagesList.Add(cultureInfo);
                            }
                            model.TargetLanguage = targetLanguagesList;
                        }
                    }
                }
            
          
            var filesName = await Task.FromResult( GetFilesName());

            var names=await Task.FromResult(ExtractFilesFromArchive(filesName, pathToTempFolder));

            var targetFiles = await Task.FromResult(AddTargetFiles(model, names,pathToTempFolder));
            model.TargetFiles = targetFiles;

            var sourceFiles = await Task.FromResult(AddSourceFiles(model, names,pathToTempFolder));
            model.SourceFiles = sourceFiles;

            return model;

        }

        private string[] AddSourceFiles(PackageModel model, List<string> names,string pathToTempFolder)
        {
            var tempFiles = Directory.GetFiles(pathToTempFolder);
            var extension = model.SourceLanguage.ThreeLetterWindowsLanguageName;
            var sourcePathList = new List<string>();
            //selects from temp folder files which ends with source language code
            var filesFromTemp = (from file in tempFiles where file.Contains(extension) select file).ToList();

            //selects from files name only the names which contains the source language code
            var sourceName = (from name in names where name.Contains(extension) select name).ToList();

            foreach (var name in sourceName)
            {
                var path = (from file in filesFromTemp where file.Contains(name) select file).ToList();
                sourcePathList.AddRange(path);
            }

            var files = new string[sourcePathList.Count];
            for (var i = 0; i < sourcePathList.Count; i++)
            {
                files[i] = sourcePathList[i];
            }

            return files;
        }


        private string[] AddTargetFiles(PackageModel model, List<string> filesName,string pathToTempFolder)
        {

            var pathList = new List<string>();
            var tempFiles = Directory.GetFiles(pathToTempFolder);
            var pathTotargetFiles = new List<string>();
            var targetFilesName = new List<string>();



            foreach (var language in model.TargetLanguage)
            {
                var extension = language.ThreeLetterWindowsLanguageName;
                //selects from temp folder files which ends with target language code language
                var targetFiles = (from file in tempFiles
                                   where file.Contains(extension)
                                   select file).ToList();

                //selects from files name only the names which contains the target language code
                var names = (from name in filesName where name.Contains(extension) select name).ToList();
                pathList.AddRange(targetFiles);
                targetFilesName.AddRange(names);

            }

            foreach (var fileName in targetFilesName)
            {

                var targetPath = (from path in pathList where path.Contains(fileName) select path).ToList();
                pathTotargetFiles.AddRange(targetPath);
            }


            var files = new string[pathTotargetFiles.Count];
            for (var i = 0; i < pathTotargetFiles.Count; i++)
            {
                files[i] = pathTotargetFiles[i];
            }

            return files;
        }



        private List<string> ExtractFilesFromArchive(List<string> filesName, string pathToTempFolder)
        {
            var filesNameList = new List<string>();
            var tempFiles = Directory.GetFiles(pathToTempFolder,"*.*",SearchOption.AllDirectories);
           
                    foreach (var entry in tempFiles)
                    {
                        filesNameList.AddRange(from name in filesName where entry.Contains(name) select entry);
                    }
                
         
            return filesNameList;
        }

        private CultureInfo Language(int languageCode)
        {
            return new CultureInfo(languageCode);
        }

        /// <summary>
        /// Return a list of file names
        /// </summary>
        /// <returns></returns>
        private  List<string> GetFilesName()
        {
            //takes values from dictionary
            var filesDictionary = _pluginDictionary["Files"];
            var fileNameList = new List<string>();

            //loop through the keys in order to take the name  
            foreach (var key in filesDictionary.Keys)
            {
                var file = filesDictionary[key];
                var fileName=FileName(file);
                fileNameList.Add(fileName);
            }
            return fileNameList;
        }

        /// <summary>
        /// Splits the text after "|" and take the file name 
        /// </summary>
        /// <param name="file"></param>
        /// <returns>file name</returns>
        private string FileName(string file)
        {
            var words = file.Split('|');
            var fileName = words[6];
            return fileName;
        }
    }
}
