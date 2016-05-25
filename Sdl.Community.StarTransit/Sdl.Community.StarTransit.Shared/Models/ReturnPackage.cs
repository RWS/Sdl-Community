using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sdl.ProjectAutomation.Core;
using Sdl.ProjectAutomation.FileBased;

namespace Sdl.Community.StarTransit.Shared.Models
{
    public class ReturnPackage
    {
        /// <summary>
        /// The location of the return package folder where the archive will be created
        /// </summary>
        public string FolderLocation { get; set; }
        public List<ProjectFile> TargetFiles { get; set; }
        public string ProjectLocation { get; set; }
        public FileBasedProject FileBasedProject { get; set; }
        public string LocalFilePath { get; set; }
        public string FileName { get; set; }
    }
}
