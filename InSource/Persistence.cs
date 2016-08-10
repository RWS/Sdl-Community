using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Sdl.Community.InSource.Models;

namespace Sdl.Community.InSource
{
    public class Persistence
    {
        private readonly string _persistancePath;
       
        public Persistence()
        {
            _persistancePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                @"SDL Community\StudioAutomation\studioAutomation.json");

        }

        public void SaveProjectRequestList(List<ProjectRequest> persistenceListFolders)
        {

            var jsonText = File.ReadAllText(_persistancePath);
            var request = JsonConvert.DeserializeObject<Request>(jsonText);

            request.ProjectRequest = persistenceListFolders;


            var json = JsonConvert.SerializeObject(request);
            
            File.WriteAllText(_persistancePath,json);
            
        }

        public void SaveTimerSettings(TimerModel settings)
        {
            var jsonText = File.ReadAllText(_persistancePath);
            var request = JsonConvert.DeserializeObject<Request>(jsonText);

            request.Timer = settings;

            var json = JsonConvert.SerializeObject(request);

            File.WriteAllText(_persistancePath, json);

        }
        private void RecoverDataFromOldJson()
        {
            var jsonText = File.ReadAllText(_persistancePath);
            var watchFoldersPath = JsonConvert.DeserializeObject<List<ProjectRequest>>(jsonText);

            DeleteOldJson();
            CreateNewJsonFile(watchFoldersPath);
        }

        private void CreateNewJsonFile(List<ProjectRequest> watchFolderList)
        {
            if (!File.Exists(_persistancePath))
            {
                var directory = Path.GetDirectoryName(_persistancePath);
                if (directory != null && !Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }
            }

            var request = new Request
            {
                ProjectRequest = watchFolderList,
                Timer = new TimerModel()

            };

            var json = JsonConvert.SerializeObject(request);

            File.WriteAllText(_persistancePath, json);
        }
        private void DeleteOldJson()
        {
            File.Delete(_persistancePath);
        }

        public void IsStructureChanged()
        {
            if (!File.Exists(_persistancePath))
            {
                
                var directory = Path.GetDirectoryName(_persistancePath);
                if (directory != null && !Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }
                File.Create(_persistancePath);
            }
            var jsonText = File.ReadAllText(_persistancePath);

            //this means the structure of json file has changed
            if (!jsonText.Contains("HasTimer")&&!string.IsNullOrEmpty(jsonText))
            {
                RecoverDataFromOldJson();
            }
        }

     
        public void Update(ProjectRequest projectRequest)
        {
            var projectRequestList = Load();
            var projectToUpdate = projectRequestList.FirstOrDefault(p => p.Name == projectRequest.Name);
            if (projectToUpdate != null) projectToUpdate.Files = new string[] {};

            SaveProjectRequestList(projectRequestList);

        }

        public void Watch(ProjectRequest projectRequest)
        {
            var projectRequestList = Load();
            var projectToUpdate = projectRequestList.FirstOrDefault(p => p.Name == projectRequest.Name);
            if (projectToUpdate != null) projectToUpdate.Files = projectRequest.Files;
            
            SaveProjectRequestList(projectRequestList);
        }


        public List<ProjectRequest> Load()
        {
            if (!File.Exists(_persistancePath)) return null;
            var json = File.ReadAllText(_persistancePath);
            
            var result = JsonConvert.DeserializeObject<Request>(json);
            if (result != null) { return result.ProjectRequest; }
            return new List<ProjectRequest>();
        }

        public TimerModel LoadTimerSettings()
        {
            if (!File.Exists(_persistancePath))
            {
                CreateNewJsonFile(new List<ProjectRequest>());
            }
            var json = File.ReadAllText(_persistancePath);

            var savedData = JsonConvert.DeserializeObject<Request>(json);
            if (savedData != null)
            {
                return savedData.Timer;
            }
            
                return  new TimerModel();
         
        }

        public void AddProjectRequests(List<ProjectRequest> projectRequests)
        {
            var projectRequestList = Load();
            foreach (var project in projectRequests)
            {
                projectRequestList.Add(project);
            }

            SaveProjectRequestList(projectRequestList);
        }
    }
}
