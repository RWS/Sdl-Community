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
                @"Trados AppStore\StudioAutomation\studioAutomation.json");
        }

	    public void SaveProjectRequestList(List<ProjectRequest> persistenceListFolders)
	    {
		    var jsonText = File.ReadAllText(_persistancePath);
		    var request = JsonConvert.DeserializeObject<Request>(jsonText);

		    request.ProjectRequest = persistenceListFolders;
		    var json = JsonConvert.SerializeObject(request);

		    File.WriteAllText(_persistancePath, json);
	    }

	    public void SaveTimerSettings(TimerModel settings)
        {
            var jsonText = File.ReadAllText(_persistancePath);
            var request = JsonConvert.DeserializeObject<Request>(jsonText);

            request.Timer = settings;

            var json = JsonConvert.SerializeObject(request);

            File.WriteAllText(_persistancePath, json);
        }

        private void CreateNewJsonFile(Request recoveredJson)
        {
            if (!File.Exists(_persistancePath))
            {
                var directory = Path.GetDirectoryName(_persistancePath);
                if (directory != null && !Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }
            }

            recoveredJson.DeleteFolders = false;

            var json = JsonConvert.SerializeObject(recoveredJson);

            File.WriteAllText(_persistancePath, json);
        }
     
        public void Update(ProjectRequest projectRequest)
        {
            var projectRequestList = Load();
            var projectToUpdate = projectRequestList.FirstOrDefault(p => p.Name == projectRequest.Name);
            if (projectToUpdate != null) projectToUpdate.Files = new string[] {};

            SaveProjectRequestList(projectRequestList);
        }

        public void UpdateDelete(bool delete)
        {
            var savedRequest = LoadRequest();

            savedRequest.DeleteFolders = delete;
            var json = JsonConvert.SerializeObject(savedRequest);

            File.WriteAllText(_persistancePath, json);
        }

        public List<ProjectRequest> Load()
        {
            if (!File.Exists(_persistancePath)) return null;
            var json = File.ReadAllText(_persistancePath);
            
            var result = JsonConvert.DeserializeObject<Request>(json);
            if (result != null) { return result.ProjectRequest; }
            return new List<ProjectRequest>();
        }

        public Request LoadRequest()
        {
            if (!File.Exists(_persistancePath))
            {
                CreateNewJsonFile(new Request
                {
                    ProjectRequest = new List<ProjectRequest>(),
                    Timer = new TimerModel
                    {
                        HasTimer = false
                    },
                    DeleteFolders = false
                });
            }

            var json = File.ReadAllText(_persistancePath);

            var savedData = JsonConvert.DeserializeObject<Request>(json);
            return savedData;
        }
    }
}
