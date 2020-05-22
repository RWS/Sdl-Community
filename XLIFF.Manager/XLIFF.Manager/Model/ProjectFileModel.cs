using System;
using System.Collections.Generic;
using Sdl.Community.XLIFF.Manager.Common;

namespace Sdl.Community.XLIFF.Manager.Model
{
	public class ProjectFileModel : BaseModel, IDisposable, ICloneable
	{
		private bool _selected;
		private string _path;
		private Enumerators.Status _status;

		public ProjectFileModel(ProjectModel projectModel)
		{
			ProjectModel = projectModel;
			ProjectFileActivityModels = new List<ProjectFileActivityModel>();			
		}

		public ProjectModel ProjectModel { get; }

		public List<ProjectFileActivityModel> ProjectFileActivityModels { get; set; }

		public Enumerators.Action Action { get; set; }

		public string FileType { get; set; }

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

		public Enumerators.Status Status
		{
			get => _status;
			set
			{
				if (_status == value)
				{
					return;
				}

				_status = value;
				OnPropertyChanged(nameof(Status));
			}
		}

		public string Id { get; set; }

		public string Name { get; set; }

		public string Path
		{
			get => string.IsNullOrEmpty(_path) ? "\\" : _path;
			set
			{
				if (_path == value)
				{
					return;
				}

				_path = value;
				OnPropertyChanged(nameof(Path));
			}
		}

		public string XliffFilePath { get; set; }

		public DateTime Date { get; set; }

		public LanguageInfo TargetLanguage { get; set; }

		public string ShortMessage { get; set; }

		public string Details { get; set; }

		public void Dispose()
		{
		}

		public object Clone()
		{
			var model = new ProjectFileModel(ProjectModel.Clone() as ProjectModel)
			{
				Id = Id.Clone() as string,
				Name = Name.Clone() as string,
				Action = Action,
				Date = Date,
				Status =  Status,
				Path = Path.Clone() as string,
				Selected = Selected,
				TargetLanguage = TargetLanguage.Clone() as LanguageInfo,
				XliffFilePath = XliffFilePath.Clone() as string,
				Details = Details?.Clone() as string,
				FileType = FileType.Clone() as string,
			};

			foreach (var fileActivityModel in model.ProjectFileActivityModels)
			{
				model.ProjectFileActivityModels.Add(fileActivityModel.Clone() as ProjectFileActivityModel);
			}

			return model;
		}
	}
}
