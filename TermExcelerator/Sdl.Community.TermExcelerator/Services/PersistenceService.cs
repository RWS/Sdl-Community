﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Sdl.Community.TermExcelerator.Model;

namespace Sdl.Community.TermExcelerator.Services
{
	public class PersistenceService
    {
        private readonly string _persistancePath;
        private List<ProviderSettings> _providerSettingList = new List<ProviderSettings>();

        public PersistenceService()
        {
            _persistancePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
				@"Trados AppStore\ExcelTerminology\excelTerminology.json");
        }

        internal void WriteToFile()
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
            if (providerSettings == null)
            {
                throw new NullReferenceException("Provider settings cannot be null");
            }

            if (providerSettings.Uri == null)
            {
                throw new NullReferenceException("Uri cannot be null");
            }

            GetProviderSettingsList();

            if (providerSettings.Uri != null)
            {
                var result = _providerSettingList.FirstOrDefault(s => s.Uri == providerSettings.Uri);

                if (result != null)
                {
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
                    result.IsReadOnly = providerSettings.IsReadOnly;
                }
                else
                {
                    _providerSettingList.Add(providerSettings);
                }
            }
            WriteToFile();
        }

        public void GetProviderSettingsList()
        {
            if (!File.Exists(_persistancePath)) return;
            var json = File.ReadAllText(_persistancePath);
            if (!string.IsNullOrEmpty(json))
            {
                _providerSettingList = JsonConvert.DeserializeObject<List<ProviderSettings>>(json);
            }
        }

        public ProviderSettings Load(Uri providerUri)
        {
            if (providerUri == null)
            {
                throw new NullReferenceException("Uri cannot be null");
            }

            GetProviderSettingsList();
            var providerSettings = _providerSettingList.FirstOrDefault(p => p.Uri == providerUri);

            return providerSettings;
        }
    }
}