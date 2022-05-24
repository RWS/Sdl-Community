using System.IO;

namespace Sdl.Community.RecordSourceTU
{
    public class CustomFieldValues
    {
        public string FileNameFullPath { get; set; }

        public string FileName
        {
            get { return Path.GetFileName(FileNameFullPath); }
        }

        public string ProjectName { get; set; }
    }
}
