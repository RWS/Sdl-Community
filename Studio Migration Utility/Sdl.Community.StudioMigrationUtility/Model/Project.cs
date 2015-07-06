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



        //Modification ID: PH_2015-07-05T21:12:00
        //Date: 2015-07-05
        //Added by: Patrick Hartnett
        //Begin Edit (PH_2015-07-05T21:00:00)      
        public Customer Customer { get; set; }
        //End Edit (PH_2015-07-05T21:00:00)      

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
