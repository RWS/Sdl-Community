using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace Sdl.Community.ContentConnector
{
    public class Persistence
    {
        private readonly string _persistancePath;
       
        public Persistence()
        {
            _persistancePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                @"SDL Community\StudioAutomation\studioAutomation.json");

        }

        public void Save(List<ProjectRequest> persistenceListFolders)
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

        public void Update(ProjectRequest projectRequest)
        {
            var projectRequestList = Load();
            var projectToUpdate = projectRequestList.FirstOrDefault(p => p.Name == projectRequest.Name);
            if (projectToUpdate != null) projectToUpdate.Files = new string[] {};

            Save(projectRequestList);

        }

        public void Watch(ProjectRequest projectRequest)
        {
            var projectRequestList = Load();
            var projectToUpdate = projectRequestList.FirstOrDefault(p => p.Name == projectRequest.Name);
            if (projectToUpdate != null) projectToUpdate.Files = projectRequest.Files;

            Save(projectRequestList);
        }

        public List<ProjectRequest> Load()
        {
            if (!File.Exists(_persistancePath)) return null;
            var json = File.ReadAllText(_persistancePath);

            var result = JsonConvert.DeserializeObject<List<ProjectRequest>>(json);

            return result;
        }

        public void AddProjectRequests(List<ProjectRequest> projectRequests)
        {
            var projectRequestList = Load();
            foreach (var project in projectRequests)
            {
                projectRequestList.Add(project);
            }

            Save(projectRequestList);
        }
    }
}
