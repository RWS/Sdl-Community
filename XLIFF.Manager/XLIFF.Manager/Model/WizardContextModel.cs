using System;
using System.Collections.Generic;
using Sdl.Community.XLIFF.Manager.Common;

namespace Sdl.Community.XLIFF.Manager.Model
{
	public class WizardContextModel: BaseModel, ICloneable
	{
		public WizardContextModel()
		{
			XLIFFSupport = Enumerators.XLIFFSupport.xliff12polyglot;
			ProjectFileModels = new List<ProjectFileModel>();
			IncludeTranslations = false;
			CopySourceToTarget = false;
		}

		public List<ProjectFileModel> ProjectFileModels { get; set; }

		public bool IncludeTranslations { get; set; }

		public bool CopySourceToTarget { get; set; }

		public string OutputFolder { get; set; }

		public Enumerators.XLIFFSupport XLIFFSupport { get; set; }

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
