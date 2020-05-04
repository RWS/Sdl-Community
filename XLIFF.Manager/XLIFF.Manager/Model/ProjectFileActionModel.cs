using System;
using System.Collections.Generic;
using System.Globalization;
using Sdl.Community.XLIFF.Manager.Common;

namespace Sdl.Community.XLIFF.Manager.Model
{
	public class ProjectFileActionModel : BaseModel, IDisposable
	{
		public ProjectFileActionModel(ProjectModel projectModel)
		{
			ProjectModel = projectModel;
			ProjectFileActivityModels = new List<ProjectFileActivityModel>();
			Action = Enumerators.Action.None;
		}

		public ProjectModel ProjectModel { get; }

		public List<ProjectFileActivityModel> ProjectFileActivityModels { get; set; }			

		public Enumerators.Action Action { get; set; }

		public string Id { get; set; }

		public string Name { get; set; }

		public string Path { get; set; }

		public DateTime Date { get; set; }

		public CultureInfo TargetLanguage { get; set; }

		public string Message { get; set; }

		
		//TODO
		// Options
		// Segments etc...

		public void Dispose()
		{
		}
	}
}
