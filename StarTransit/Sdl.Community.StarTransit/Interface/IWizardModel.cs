using System.Collections.Generic;
using Sdl.Community.StarTransit.Service;
using Sdl.Community.StarTransit.Shared.Models;
using Sdl.ProjectAutomation.Core;

namespace Sdl.Community.StarTransit.Interface
{
	public interface IWizardModel
	{
		string TransitFilePathLocation { get; set; }

		/// <summary>
		/// Where we'll unzip the prj file to read the data
		/// </summary>
		string PathToTempFolder { get; set; }

		/// <summary>
		/// Location where Studio project will be created. It needs to be an empty folder
		/// </summary>
		string StudioProjectLocation { get; set; }
		Customer SelectedCustomer { get; set; }
		AsyncTaskWatcherService<List<Customer>> Customers { get; set; }
		AsyncTaskWatcherService<PackageModel> PackageModel { get; set; }
		/// <summary>
		/// Studio project template
		/// </summary>
		List<ProjectTemplateInfo> ProjectTemplates { get; set; }
		ProjectTemplateInfo SelectedTemplate { get; set; }
	}
}
