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

        public PersistenceService()
        {
            _persistancePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), @"SDL Community\ExcelTerminology\excelTerminology.json");
        }

        public void Save(ProviderSettings toSave)
        {

            if (!File.Exists(_persistancePath))
            {
                var directory = Path.GetDirectoryName(_persistancePath);
                if (directory != null && !Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }
            }

            var json = JsonConvert.SerializeObject(toSave);
            File.WriteAllText(_persistancePath, json);  
        }

        public ProviderSettings Load()
        {
            if (!File.Exists(_persistancePath)) return null;
            var json = File.ReadAllText(_persistancePath);

            var result = JsonConvert.DeserializeObject<ProviderSettings>(json);

            return result;
        }
    }
}
