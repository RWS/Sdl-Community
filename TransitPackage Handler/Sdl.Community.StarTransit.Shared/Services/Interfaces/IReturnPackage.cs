using System.Collections.Generic;
using Sdl.Community.StarTransit.Shared.Models;
using Sdl.ProjectAutomation.Core;
using Sdl.ProjectAutomation.FileBased;

namespace Sdl.Community.StarTransit.Shared.Services.Interfaces
{
	public interface IReturnPackage
	{
		string FolderLocation { get; set; }
		List<ProjectFile> TargetFiles { get; set; }
		List<ProjectFile> SelectedTargetFilesForImport { get; set; }
		string ProjectLocation { get; set; }
		FileBasedProject FileBasedProject { get; set; }
		string LocalFilePath { get; set; }
		string FileName { get; set; }
		string PathToPrjFile { get; set; }
		List<ReturnFileDetails> ReturnFilesDetails { get; set; }
	}
}
