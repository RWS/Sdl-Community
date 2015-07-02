using System;

namespace Sdl.Community.StudioMigrationUtility.Model
{
    public class Project
    {

        public Guid Guid { get; set; }
        public string ProjectFilePath { get; set; }
        public DateTime StartedAt { get; set; }
        public bool IsInPlace { get; set; }
        public bool IsImported { get; set; }
        public string Description { get; set; }
        public string Name { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }

        public string Status { get; set; }

        public override string ToString()
        {
            return Name;
        }

        public override bool Equals(object obj)
        {
            var tmpProj = (Project) obj;
            return Guid.Equals(tmpProj.Guid);
        }
    }
}
