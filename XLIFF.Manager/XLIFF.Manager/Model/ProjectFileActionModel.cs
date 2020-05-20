using System;
using System.Collections.Generic;
using Sdl.Community.XLIFF.Manager.Common;

namespace Sdl.Community.XLIFF.Manager.Model
{
	public class ProjectFileActionModel : BaseModel, IDisposable
	{
		private bool _selected;

		public ProjectFileActionModel(ProjectModel projectModel)
		{
			ProjectModel = projectModel;
			ProjectFileActivityModels = new List<ProjectFileActivityModel>();			
		}

		public ProjectModel ProjectModel { get; }

		public List<ProjectFileActivityModel> ProjectFileActivityModels { get; set; }

		public Enumerators.Action Action { get; set; }

		public bool Selected
		{
			get { return _selected; }
			set
			{
				if (_selected == value)
				{
					return;
				}

				_selected = value;
				OnPropertyChanged(nameof(Selected));
			}
		}

		public string Status
		{
			get
			{
				switch (Action)
				{
					case Enumerators.Action.Export:
						return "Exported";
					case Enumerators.Action.Import:
						return "Imported";
					default:

						return "None";
				}
			}
		}

		public string Id { get; set; }

		public string Name { get; set; }

		public string Path { get; set; }

		public string XliffFilePath { get; set; }

		public DateTime Date { get; set; }

		public LanguageInfo TargetLanguage { get; set; }

		public string Details { get; set; }

		//TODO
		// Options
		// Segments etc...

		public void Dispose()
		{
		}
	}
}
