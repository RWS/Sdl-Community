using System;
using System.Collections.Generic;

namespace Sdl.Community.Studio.Time.Tracker.Structures
{
    [Serializable]
    public class TrackerProject: ICloneable
    {

        public string IdStudio { get; set; }
        public string NameStudio { get; set; }
        public string PathStudio { get; set; }

        public string Id { get; set; }        
        public string Name { get; set; }
        public string Path { get; set; }

        public string Description { get; set; }
        public string ClientId { get; set; }
       
        public string ProjectStatus { get; set; } //In progress, Completed

        public DateTime DateCreated { get; set; }
        
        public DateTime DateStart { get; set; }
        
        public DateTime DateCompleted { get; set; }
        
        public DateTime DateDue { get; set; }

        public List<TrackerProjectActivity> ProjectActivities { get; set; }
        

        public TrackerProject()
        {
            IdStudio = string.Empty;
            NameStudio = string.Empty;
            PathStudio = string.Empty;
            
            Id = string.Empty;
            Name = string.Empty;
            Path = string.Empty;

            Description = string.Empty;
            ClientId = string.Empty;
            
            ProjectStatus = string.Empty;

            DateCreated = Common.DateNull;

            DateStart = Common.DateNull;

            DateCompleted = Common.DateNull;

            DateDue = Common.DateNull;


            ProjectActivities = new List<TrackerProjectActivity>();
           
        }

        public object Clone()
        {

            var trackerProject = new TrackerProject();

            trackerProject.IdStudio = IdStudio;
            trackerProject.NameStudio = NameStudio;
            trackerProject.PathStudio = PathStudio;

            trackerProject.Id = Id;       
            trackerProject.Name = Name;
            trackerProject.Path = Path;

            trackerProject.Description = Description;
            trackerProject.ClientId = ClientId;
          
            trackerProject.ProjectStatus = ProjectStatus;

            trackerProject.DateCreated = DateCreated;
            trackerProject.DateStart = DateStart;
            trackerProject.DateCompleted = DateCompleted;
            trackerProject.DateDue = DateDue;

            trackerProject.ProjectActivities = new List<TrackerProjectActivity>();
            foreach (var tpa in ProjectActivities)
                trackerProject.ProjectActivities.Add((TrackerProjectActivity)tpa.Clone());

            return trackerProject;

            
        }
    }
}
