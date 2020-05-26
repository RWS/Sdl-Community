using System;
using Sdl.Community.XLIFF.Manager.Common;

namespace Sdl.Community.XLIFF.Manager.Model
{
	public class ProjectFileActivityModel: BaseModel, IDisposable, ICloneable
	{	
		public ProjectFileActivityModel(ProjectFileModel projectFileModel)
		{
			ProjectFileModel = projectFileModel;
			Status = Enumerators.Status.None;
			Action = Enumerators.Action.None;
		}

		public ProjectFileModel ProjectFileModel { get; }

		public Enumerators.Status Status { get; set; }

		public Enumerators.Action Action { get; set; }	
		
		public bool Selected { get; set; }

		public string Id { get; set; }

		public string Name { get; set; }

		public string Path { get; set; }

		public DateTime Date { get; set; }		

		public string Details { get; set; }		

		public void Dispose()
		{
		}

		public object Clone()
		{
			var model = new ProjectFileActivityModel(ProjectFileModel.Clone() as ProjectFileModel)
			{
				Action = Action,
				Status = Status,
				Id = Id.Clone() as string,
				Name = Name.Clone() as string,
				Date = Date,
				Path = Path,
				Details = Details,
				Selected = Selected
			};

			return model;
		}
	}
}
