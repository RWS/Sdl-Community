using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sdl.Community.XLIFF.Manager.Wizard.ViewModel;

namespace Sdl.Community.XLIFF.Manager.Model
{
	public class WizardContextModel: BaseModel, ICloneable
	{
		public WizardContextModel()
		{
			ProjectFileModels = new List<ProjectFileModel>();
		}

		public List<ProjectFileModel> ProjectFileModels { get; set; }

		public object Clone()
		{
			var model = new WizardContextModel();
			foreach (var projectFileModel in ProjectFileModels)
			{
				model.ProjectFileModels.Add(projectFileModel.Clone() as ProjectFileModel);
			}

			return model;
		}
	}
}
