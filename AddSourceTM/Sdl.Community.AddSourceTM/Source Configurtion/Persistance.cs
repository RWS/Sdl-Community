using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Sdl.Community.AddSourceTM.Source_Configurtion
{
    public class Persistance
    {
        private readonly string _persistancePath;

        public Persistance()
        {
            _persistancePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), @"SDL Community\AddSourceTM\addSourceTM.json");
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
            //using (var stream = new FileStream(_persistancePath, FileMode.OpenOrCreate))
            //{
            //    using (var writer = new StreamWriter(stream))
            //    {
                    
            //        writer.Write(json);
            //    }
            //}
        }

        public AddSourceTmConfigurations Load()
        {
            if (!File.Exists(_persistancePath)) return null;
            var json = File.ReadAllText(_persistancePath);

            var result = JsonConvert.DeserializeObject<AddSourceTmConfigurations>(json);

            return result;
        }
    }
}
