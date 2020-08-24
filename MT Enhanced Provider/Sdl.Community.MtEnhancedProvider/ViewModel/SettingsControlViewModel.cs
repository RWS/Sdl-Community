using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sdl.Community.MtEnhancedProvider.Model;
using Sdl.Community.MtEnhancedProvider.Model.Interface;
using Sdl.Community.MtEnhancedProvider.ViewModel.Interface;

namespace Sdl.Community.MtEnhancedProvider.ViewModel
{
	public class SettingsControlViewModel: ModelBase, ISettingsControlViewModel
	{
		public IModelBase ViewModel { get; set; }

		public SettingsControlViewModel()
		{
			ViewModel = this;
		}
	}
}
