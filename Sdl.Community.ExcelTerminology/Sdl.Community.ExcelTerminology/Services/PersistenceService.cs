using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Sdl.Community.ExcelTerminology.Model;

namespace Sdl.Community.ExcelTerminology.Services
{
    public class PersistenceService
    {
        private readonly string _persistancePath;
        private  List<ProviderSettings> _providerSettingList = new List<ProviderSettings>();
      
     
        public PersistenceService()
        {
            _persistancePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                @"SDL Community\ExcelTerminology\excelTerminology.json");
        }

        public void Save()
        {

            if (!File.Exists(_persistancePath))
            {
                var directory = Path.GetDirectoryName(_persistancePath);
                if (directory != null && !Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }
            }

            var json = JsonConvert.SerializeObject(_providerSettingList);
            File.WriteAllText(_persistancePath, json);  
        }

        public void AddSettings(ProviderSettings providerSettings)
        {
            GetProviderSettingsList();
            _providerSettinngsUri = providerSettings.Uri;

            var result = _providerSettingList.FirstOrDefault(s => s.Uri == providerSettings.Uri);

            if (result != null)
            {
                var index = _providerSettingList.FindIndex(s=>s.Uri == providerSettings.Uri);

                result.WorksheetName = providerSettings.WorksheetName;
                result.Uri = providerSettings.Uri;
                result.ApprovedColumn = providerSettings.ApprovedColumn;
                result.HasHeader = providerSettings.HasHeader;
                result.Separator = providerSettings.Separator;
                result.SourceColumn = providerSettings.SourceColumn;
                result.SourceLanguage = providerSettings.SourceLanguage;
                result.TargetColumn = providerSettings.TargetColumn;
                result.TermFilePath = providerSettings.TermFilePath;
                result.TargetLanguage = providerSettings.TargetLanguage;

                _providerSettingList[index] = result;
            }
            else
            {
                if (providerSettings.Uri != null)
                {
                    _providerSettingList.Add(providerSettings);
                }
            }
        }

        public void GetProviderSettingsList()
        {
            var json = File.ReadAllText(_persistancePath);

            _providerSettingList = JsonConvert.DeserializeObject<List<ProviderSettings>>(json);
        }
        
        public ProviderSettings Load(Uri providerUri)
        {
            if (!File.Exists(_persistancePath)) return null;
            var json = File.ReadAllText(_persistancePath);

            var result = JsonConvert.DeserializeObject<List<ProviderSettings>>(json);

            var settings = result.FirstOrDefault(r => r.Uri == providerUri);

            return settings;
        }
    }
}
