using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sdl.Community.StarTransit.Interface;
using Sdl.Community.StarTransit.Service;
using Sdl.Community.StarTransit.Shared.Models;

namespace Sdl.Community.StarTransit.Model
{
	public class WizardModel:BaseModel,IWizardModel
	{
		private AsyncTaskWatcherService<PackageModel> _packageModel;
		public string TransitFilePathLocation { get; set; }
		public string PathToTempFolder { get; set; }

		public AsyncTaskWatcherService<PackageModel> PackageModel
		{
			get => _packageModel;
			set
			{
				_packageModel = value;
				OnPropertyChanged(nameof(PackageModel));
			}
		}
	}
}
