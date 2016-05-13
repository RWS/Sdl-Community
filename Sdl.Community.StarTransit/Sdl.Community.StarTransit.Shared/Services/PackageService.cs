using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.XPath;
using Sdl.Community.StarTransit.Shared.Models;
using Sdl.Community.StarTransit.Shared.Utils;
using Sdl.Core.Globalization;

namespace Sdl.Community.StarTransit.Shared.Services
{
    public class PackageService
    {
        private readonly List<KeyValuePair<string,string>> _dictionaryPropetries = new List<KeyValuePair<string, string>>(); 
        private  Dictionary<string,List<KeyValuePair<string,string>>> _pluginDictionary = new Dictionary<string, List<KeyValuePair<string,string>>>();
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

                    
                        if (line.StartsWith("[") && line.EndsWith("]"))
                        {
                            var valuesDictionaries = new List<KeyValuePair<string, string>>();
                            if (keyProperty != string.Empty && _dictionaryPropetries.Count != 0)
                            {
                                valuesDictionaries.AddRange(_dictionaryPropetries.Select(property => new KeyValuePair<string, string>(property.Key, property.Value)));
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
                            _dictionaryPropetries.Add(new KeyValuePair<string, string>(properties[0], properties[1]));


                        }

                 
                }
            }

            var packageModel = await CreateModel(pathToTempFolder);


            _package = packageModel;
            return packageModel;
        }

        private async Task<PackageModel> CreateModel(string pathToTempFolder)
        {
            var model = new PackageModel();
            var languagePair = new LanguagePair();
            var sourceLanguageCode = 0;
            var targetLanguageCode = 0;
            
            var languagePairList = new List<LanguagePair>();
                if (_pluginDictionary.ContainsKey("Admin"))
                {
                    var propertiesDictionary = _pluginDictionary["Admin"];
                    foreach (var item in propertiesDictionary)
                    {
                        if (item.Key == "ProjectName")
                        {
                            model.Name = item.Value;
                        }
                    }
                }

                if (_pluginDictionary.ContainsKey("Languages"))
                {
                    var propertiesDictionary = _pluginDictionary["Languages"];
                    foreach (var item in propertiesDictionary)
                    {
                        if (item.Key == "SourceLanguage")
                        {
                            sourceLanguageCode = int.Parse(item.Value);
                            languagePair.SourceLanguage = Language(sourceLanguageCode);

                        }
                        if (item.Key == "TargetLanguages")
                        {
                            //we assume languages code are separated by "|"
                            var languages = item.Value.Split(LanguageTargetSeparator);
                            
                            foreach (var language in languages)
                            {
                            targetLanguageCode = int.Parse(language);
                                var cultureInfo = Language(targetLanguageCode);
                                var pair = new LanguagePair
                                {
                                    SourceLanguage = languagePair.SourceLanguage,
                                    TargetLanguage = cultureInfo
                                };
                            languagePairList.Add(pair);
                            }
                        }
                    }
                }
            model.LanguagePairs = languagePairList;
            
            var filesName = await Task.FromResult( GetFilesName());

            var names=await Task.FromResult(ExtractFilesFromArchive(filesName, pathToTempFolder));

            var modelWithSourceTm = await Task.FromResult(AddSourceFilesAndTm(model, names, pathToTempFolder, sourceLanguageCode));

            var finalPackage = await Task.FromResult(AddTargetFilesAndTm(modelWithSourceTm, names,pathToTempFolder,targetLanguageCode));
      
            return finalPackage;

        }

  

        private Guid IsTmFile(string file)
        {
            var tmFile = XElement.Load(file);
            if (tmFile.Attribute("ExtFileType") != null)
            {
              
                var ffdNode =
                    (from ffd in tmFile.Descendants("FFD") select new Guid(ffd.Attribute("GUID").Value)).FirstOrDefault();
                return ffdNode;
            }

          return Guid.Empty;
        }

        private PackageModel AddSourceFilesAndTm(PackageModel model, List<string> names,string pathToTempFolder,int languageCode)
        {
            var tempFiles = Directory.GetFiles(pathToTempFolder);
          
            var sourcePathList = new List<string>();
            var languagePairList = new List<LanguagePair>();
            var tmMetadataList =new List<StarTranslationMemoryMetadata>();

            var sourceLanguage = Language(languageCode);
            var extension = sourceLanguage.ThreeLetterWindowsLanguageName;
            //selects from temp folder files which ends with source language code
            var filesFromTemp = (from file in tempFiles where file.Contains(extension) select file).ToList();

            //selects from files name only the names which contains the source language code
            var sourceName = (from name in names where name.Contains(extension) select name).ToList();

            foreach (var name in sourceName)
            {
                var path = (from file in filesFromTemp where file.Contains(name) select file).ToList();
                sourcePathList.AddRange(path);
            }

            foreach (var file in filesFromTemp)
            {
                var guid = IsTmFile(file);
                if (guid != Guid.Empty)
                {
                    var tmMetadata = new StarTranslationMemoryMetadata
                    {
                        Id = guid,
                        SourceFile = file
                    };
                    tmMetadataList.Add(tmMetadata);
                 
                }

            }
            bool hasTm;
            if (tmMetadataList.Count != 0)
            {
                hasTm = true;
            }
            else
            {
                hasTm = false;
            }
            var languagePair = new LanguagePair
            {
                HasTm = hasTm,
                SourceFile = sourcePathList,
                StarTranslationMemoryMetadatas = tmMetadataList,
                SourceLanguage = sourceLanguage
            };
            languagePairList.Add(languagePair);
            model.LanguagePairs = languagePairList;
            return model;
        }


        private PackageModel AddTargetFilesAndTm(PackageModel model, List<string> filesName, string pathToTempFolder,
            int targetLanguageCode)
        {

            var pathList = new List<string>();
            var tempFiles = Directory.GetFiles(pathToTempFolder);
            var pathTotargetFiles = new List<string>();
            var targetFilesName = new List<string>();

            var targetLanguage = Language(targetLanguageCode);
            var extension = targetLanguage.ThreeLetterWindowsLanguageName;

            //selects from temp folder files which ends with target language code language
            var targetFiles = (from file in tempFiles
                where file.EndsWith(extension)
                select file).ToList();

            //selects from files name only the names which contains the target language code
            var names = (from name in filesName where name.Contains(extension) select name).ToList();
            pathList.AddRange(targetFiles);
            targetFilesName.AddRange(names);



            foreach (var fileName in targetFilesName)
            {

                var targetPath = (from path in pathList where path.Contains(fileName) select path).ToList();
                pathTotargetFiles.AddRange(targetPath);
            }

            foreach (var file in targetFiles)
            {
                var guid = IsTmFile(file);
                foreach (var language in model.LanguagePairs)
                {
                    if (guid != Guid.Empty)
                    {
                        //selects the source tm which has the same id with the target tm id
                        var metaData =
                            (from pair in language.StarTranslationMemoryMetadatas where guid == pair.Id select pair)
                                .FirstOrDefault();
                        if (metaData != null)
                        {
                            metaData.TargetFile = file;
                        }

                    }
                    language.TargetFile = pathTotargetFiles;
                    language.TargetLanguage = targetLanguage;
                }

            }

            return model;
            
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

            //loop through the keys in order to take the name  
            return filesDictionary.Select(item => FileName(item.Value)).ToList();
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
