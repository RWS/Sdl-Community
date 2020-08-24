using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Sdl.Community.MtEnhancedProvider.Model;

namespace Sdl.Community.MtEnhancedProvider.ViewModel.Interface
{
	public interface IMainWindow
	{
		ViewDetails SelectedView { get; set; }
		List<ViewDetails> AvailableViews { get; set; }
		ICommand ShowSettingsViewCommand { get; set; }

	}
}
