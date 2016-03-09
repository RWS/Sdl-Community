using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace StudioIntegrationApiSample
{
    public class Persistence
    {
        private readonly string _persistancePath;
        public Persistence()
        {
            _persistancePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                @"SDL Community\StudioAutomation\studioAutomation.json");

        }

        public void Save(string path)
        {
            if (!File.Exists(_persistancePath))
            {
                var directory = Path.GetDirectoryName(_persistancePath);
                if (directory != null && !Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }
            }

            var json = JsonConvert.SerializeObject(path);
            File.WriteAllText(_persistancePath,json);
            
        }

        public string Load()
        {
            if (!File.Exists(_persistancePath)) return null;
            var json = File.ReadAllText(_persistancePath);

            var result = JsonConvert.DeserializeObject<string>(json);

            return result;
        }
    }
}
