using System.Collections.Generic;

namespace Sdl.Community.StarTransit.Shared.Models
{
	public class MetadataFileInfo
	{
		public List<string> SourceTmFilesPath { get; set; }
		public List<string> TargetTmFilesPath { get; set; }
		public List<string> SourceMtFilesPath { get; set; }
		public List<string> TargetMtFilesPath { get; set; }
		public bool IsRefFolderMetadata { get; set; }
	}
}
