using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Xml.Schema;
using Sdl.Community.MtEnhancedProvider.Commands;
using Sdl.Community.MtEnhancedProvider.Model;
using Sdl.Community.MtEnhancedProvider.Model.Interface;
using Sdl.Community.MtEnhancedProvider.ViewModel.Interface;

namespace Sdl.Community.MtEnhancedProvider.ViewModel
{
	public class ProviderControlViewModel: ModelBase, IProviderControlViewModel
	{
		public IModelBase ViewModel { get; set; }
		public ICommand ShowSettingsCommand { get; set; }

		public ProviderControlViewModel()
		{
			ViewModel = this;
			//ShowSettingsCommand = new CommandHandler(ShowSettingsPage,true);
		}

		//private void ShowSettingsPage()
		//{
		//	//TODO: Set selected view from MainWindow to be settings page
		//}
	}
}
