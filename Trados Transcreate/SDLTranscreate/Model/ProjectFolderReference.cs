using Sdl.ProjectAutomation.FileBased;

namespace Trados.Transcreate.Model
{
	public class ProjectFolderReference
	{
		public string LocalProjectFolder { get; set; }

		public string LocalProjectFolderTemp { get; set; }
		
		public FileBasedProject Project { get; set; }
	}
}
