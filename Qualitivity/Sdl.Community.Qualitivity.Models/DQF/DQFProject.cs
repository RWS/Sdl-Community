using System;
using System.Collections.Generic;

namespace Sdl.Community.Structures.DQF
{
    [Serializable]
    public class DqfProject: ICloneable
    {
        public int Id { get; set; }

        public int ProjectId { get; set; }
        public string ProjectIdStudio { get; set; }

        public string DqfPmanagerKey { get; set; }  
        public string DqfProjectKey { get; set; }
        public int DqfProjectId { get; set; }
        
        public string Name { get; set; }
        public DateTime? Created {get;set;}
        public string SourceLanguage { get; set; }
     
        public int Process { get; set; }
        public int ContentType { get; set; }
        public int Industry { get; set; }
        public int QualityLevel { get; set; }

        public bool Imported { get; set; }

        public List<DqfProjectTask> DqfTasks { get; set; }
   
        public DqfProject()
        {
            Id = -1;

            ProjectId = -1;
            ProjectIdStudio = string.Empty;

            DqfPmanagerKey = string.Empty;          
            DqfProjectKey = string.Empty;
            DqfProjectId = -1;     

            Name = string.Empty;
            Created = null;
            SourceLanguage = string.Empty; 
        
            Process = 3;
            ContentType = 7;
            Industry = 1;
            QualityLevel = 1;

            Imported = false;

            DqfTasks = new List<DqfProjectTask>();
       
        }

        public object Clone()
        {
            var dqfp = new DqfProject
            {
                Id = Id,
                ProjectId = ProjectId,
                ProjectIdStudio = ProjectIdStudio,
                DqfPmanagerKey = DqfPmanagerKey,
                DqfProjectKey = DqfProjectKey,
                DqfProjectId = DqfProjectId,
                Name = Name,
                Created = Created,
                SourceLanguage = SourceLanguage,
                Process = Process,
                ContentType = ContentType,
                Industry = Industry,
                QualityLevel = QualityLevel,
                Imported = Imported,
                DqfTasks = new List<DqfProjectTask>()
            };


            foreach (var dqfpt in DqfTasks)
                dqfp.DqfTasks.Add((DqfProjectTask)dqfpt.Clone());

            return dqfp;
        }
    }
}
