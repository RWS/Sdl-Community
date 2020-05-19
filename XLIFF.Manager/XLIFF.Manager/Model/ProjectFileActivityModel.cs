using System;
using Sdl.Community.XLIFF.Manager.Common;

namespace Sdl.Community.XLIFF.Manager.Model
{
	public class ProjectFileActivityModel: BaseModel, IDisposable
	{	
		public ProjectFileActivityModel(ProjectFileActionModel projectFileActionModel)
		{
			ProjectFileActionModel = projectFileActionModel;
			Status = Enumerators.Status.None;
			Action = Enumerators.Action.None;
		}

		public ProjectFileActionModel ProjectFileActionModel { get; }

		public Enumerators.Status Status { get; set; }

		public Enumerators.Action Action { get; set; }	
		
		public bool Selected { get; set; }

		public string Id { get; set; }

		public string Name { get; set; }

		public string Path { get; set; }

		public DateTime Date { get; set; }		

		public string Details { get; set; }
	
		//TODO
		// Options		
		// Segments etc...

		public void Dispose()
		{
		}		
	}
}
