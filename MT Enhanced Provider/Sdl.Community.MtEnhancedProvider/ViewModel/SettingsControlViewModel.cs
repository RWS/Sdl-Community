using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Sdl.Community.MtEnhancedProvider.Model;
using Sdl.Community.MtEnhancedProvider.Model.Interface;
using Sdl.Community.MtEnhancedProvider.ViewModel.Interface;

namespace Sdl.Community.MtEnhancedProvider.ViewModel
{
	public class SettingsControlViewModel: ModelBase, ISettingsControlViewModel
	{
		public IModelBase ViewModel { get; set; }
		public ICommand ShowMainWindowCommand { get; set; }
		private readonly IMtTranslationOptions _options;

		public SettingsControlViewModel(IMtTranslationOptions options)
		{
			ViewModel = this;
			_options = options;
		}
	}
}
