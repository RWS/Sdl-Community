using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Sdl.Community.XLIFF.Manager.Service;

namespace Sdl.Community.XLIFF.Manager.Wizard.ViewModel
{
	public class WizardViewModel
	{
		private readonly Window _window;
		public WizardViewModel(Window window)
		{
			_window = window;
			_window.Loaded += Window_Loaded;
			
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			_window.Loaded -= Window_Loaded;

			var pages = WizardService.CreatePages();
			WizardService.AddDataTemplates(_window, pages);
		}
	}
}
