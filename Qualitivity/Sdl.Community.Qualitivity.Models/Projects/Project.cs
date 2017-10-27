using System;
using System.Collections.Generic;
using Sdl.Community.Structures.Projects.Activities;

namespace Sdl.Community.Structures.Projects
{
    
    [Serializable]
    public class Project : ICloneable
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }
        public string Description { get; set; }
        public string SourceLanguage { get; set; }
        public string ProjectStatus { get; set; }
        
        public string StudioProjectId { get; set; }       
        public string StudioProjectName { get; set; }       
        public string StudioProjectPath { get; set; }

      
        
      
        public int CompanyProfileId { get; set; }
    
       
        
        public DateTime? Created { get; set; }            
        public DateTime? Started { get; set; }      
        public DateTime? Completed { get; set; }      
        public DateTime? Due { get; set; }
     

        public List<Activity> Activities { get; set; }


        public Project()
        {
            Id = -1;
            Name = string.Empty;
            Description = string.Empty;
            Path = string.Empty;
            SourceLanguage = string.Empty;
            ProjectStatus = string.Empty;

            StudioProjectId = string.Empty;
            StudioProjectName = string.Empty;
            StudioProjectPath = string.Empty;

            CompanyProfileId = -1;

            Created = null;
            Started = null;
            Completed = null;
            Due = null;

            Activities = new List<Activity>();
        }

        public object Clone()
        {

            var tp = new Project
            {
                StudioProjectId = StudioProjectId,
                StudioProjectName = StudioProjectName,
                StudioProjectPath = StudioProjectPath,
                SourceLanguage = SourceLanguage,
                Id = Id,
                Name = Name,
                Path = Path,
                Description = Description,
                CompanyProfileId = CompanyProfileId,
                ProjectStatus = ProjectStatus,
                Created = Created,
                Started = Started,
                Completed = Completed,
                Due = Due,
                Activities = new List<Activity>()
            };

			foreach (var tpa in Activities)
			{
				tp.Activities.Add((Activity)tpa.Clone());
			}
            return tp;
        }
    }
}