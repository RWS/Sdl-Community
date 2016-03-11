using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace Sdl.Community.ContentConnector
{
    public class Persistence
    {
        private readonly string _persistancePath;
       // private List<Folder> _persistenceListFolders = new List<Folder>(); 
        public Persistence()
        {
            _persistancePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                @"SDL Community\StudioAutomation\studioAutomation.json");

        }

        public void Save(List<Folder> persistenceListFolders)
        {
            if (!File.Exists(_persistancePath))
            {
                var directory = Path.GetDirectoryName(_persistancePath);
                if (directory != null && !Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }
            }

            var json = JsonConvert.SerializeObject(persistenceListFolders);
            File.WriteAllText(_persistancePath,json);
            
        }

        public List<Folder> Load()
        {
            if (!File.Exists(_persistancePath)) return null;
            var json = File.ReadAllText(_persistancePath);

            var result = JsonConvert.DeserializeObject<List<Folder>>(json);

            return result;
        }
    }
}
