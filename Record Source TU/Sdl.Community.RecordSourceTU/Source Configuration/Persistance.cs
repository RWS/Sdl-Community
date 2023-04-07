using System;
using System.IO;
using Newtonsoft.Json;

namespace Sdl.Community.RecordSourceTU
{
    public class Persistance
    {
        private readonly string _persistancePath;

        public Persistance()
        {
            _persistancePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), @"Trados AppStore\RecordSourceTU\recordSourceTU.json");
        }

        public void Save(AddSourceTmConfigurations toSave)
        {
            if (!File.Exists(_persistancePath))
            {
                var directory = Path.GetDirectoryName(_persistancePath);
                if (directory != null && !Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }
            }

            string json = JsonConvert.SerializeObject(toSave);
            File.WriteAllText(_persistancePath, json);
            toSave.SaveChanges();
        }

        public AddSourceTmConfigurations Load()
        {
            if (!File.Exists(_persistancePath)) return null;
            var json = File.ReadAllText(_persistancePath);

            var result = JsonConvert.DeserializeObject<AddSourceTmConfigurations>(json);
            result.SaveChanges();

            return result;
        }
    }
}
