using System;
using System.ComponentModel;
using System.Windows;
using Sdl.Community.StarTransit.Shared.Models;

namespace Sdl.Community.StarTransit.UI
{
	public class Program
	{
		static PackageModel _packageModel = new PackageModel();

		[STAThread]
		public static void InitializeMain(PackageModel packageModel)
		{
			_packageModel = packageModel;

			var backgroundWorker = new BackgroundWorker();
			backgroundWorker.DoWork += BackgroundWorker_DoWork;
			backgroundWorker.RunWorkerAsync();
		}

		private static void BackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
		{
			Application.Current.Dispatcher.BeginInvoke(new Action(() =>
			{
				StarTransitMainWindow window = new StarTransitMainWindow(_packageModel);
				window.Show();
			}));
		}
	}
}