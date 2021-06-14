using System.Collections.Generic;
using Sdl.Community.StarTransit.Shared.Services.Interfaces;
using Sdl.ProjectAutomation.Core;
using Sdl.ProjectAutomation.FileBased;

namespace Sdl.Community.StarTransit.Shared.Models
{
    public class ReturnPackage: IReturnPackage
	{
        /// <summary>
        /// The location of the return package folder where the archive will be created
        /// </summary>
        public string FolderLocation { get; set; }
        public List<ProjectFile> TargetFiles { get; set; }
        public List<ProjectFile> SelectedTargetFilesForImport { get; set; }
        public string ProjectLocation { get; set; }
        public FileBasedProject FileBasedProject { get; set; }
        public string LocalFilePath { get; set; }
        public string FileName { get; set; }
        public string PathToPrjFile { get; set; }
        public List<ReturnFileDetails> ReturnFilesDetails { get; set; }
	}
}
