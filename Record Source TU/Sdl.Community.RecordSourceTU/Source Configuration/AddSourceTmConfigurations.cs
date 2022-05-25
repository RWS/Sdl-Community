using System.Collections.Generic;

namespace Sdl.Community.RecordSourceTU
{
    public class AddSourceTmConfigurations
    {

        public AddSourceTmConfiguration Default
        {
            get
            {
                return new AddSourceTmConfiguration()
                {
                    FileNameField = "Source filename",
                    FullPathField = "Source filename and path",
                    ProjectNameField = "Source project name",
                    StoreFilename =  true,
                    StoreProjectName = false,
                    StoreFullPath = false
                };
            }
        }

        public List<AddSourceTmConfiguration> Configurations { get; set; }

        public void SaveChanges()
        {
            Configurations.ForEach(x => x.SaveChanges());
        }
    }
}
