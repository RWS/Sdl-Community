using System;
using Sdl.Utilities.SplitSDLXLIFF;

namespace Sdl.Community.SplitMergeUIWix
{
	static class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main()
		{
			var wizardPage = new WizardPage();
			wizardPage.ShowDialog();
		}
	}
}
