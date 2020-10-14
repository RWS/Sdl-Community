using System.Collections.Generic;
using Sdl.Community.Transcreate.Model;

namespace Sdl.Community.Transcreate.CustomEventArgs
{
	public class ProjectFileSelectionChangedEventArgs: System.EventArgs
	{
		public List<ProjectFile> SelectedFiles { get; set; }
	}
}
