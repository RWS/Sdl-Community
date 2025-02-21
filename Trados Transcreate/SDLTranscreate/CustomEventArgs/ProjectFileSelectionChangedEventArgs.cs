using System.Collections.Generic;
using Trados.Transcreate.Model;

namespace Trados.Transcreate.CustomEventArgs
{
	public class ProjectFileSelectionChangedEventArgs: System.EventArgs
	{
		public List<ProjectFile> SelectedFiles { get; set; }
	}
}
