using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Sdl.Community.AhkPlugin.Helpers
{
	public static class Resources
	{
		public static  void EnsureApplicationResources()
		{

			if (Application.Current == null)
			{
				new Application { ShutdownMode = ShutdownMode.OnExplicitShutdown };

				var controlsResources = new ResourceDictionary
				{
					Source = new Uri("pack://application:,,,/MahApps.Metro;component/Styles/Controls.xaml")
				};
				var fontsResources = new ResourceDictionary
				{
					Source = new Uri("pack://application:,,,/MahApps.Metro;component/Styles/Fonts.xaml")
				};
				var colorsResources = new ResourceDictionary
				{
					Source = new Uri("pack://application:,,,/MahApps.Metro;component/Styles/Colors.xaml")
				};
				var blueResources = new ResourceDictionary
				{
					Source = new Uri("pack://application:,,,/MahApps.Metro;component/Styles/Accents/Blue.xaml")
				};
				var baseLightResources = new ResourceDictionary
				{
					Source = new Uri("pack://application:,,,/MahApps.Metro;component/Styles/Accents/BaseLight.xaml")
				};
				var flatButtonsResources = new ResourceDictionary
				{
					Source = new Uri("pack://application:,,,/MahApps.Metro;component/Styles/FlatButton.xaml")
				};

				Application.Current.Resources.MergedDictionaries.Add(controlsResources);
				Application.Current.Resources.MergedDictionaries.Add(fontsResources);
				Application.Current.Resources.MergedDictionaries.Add(colorsResources);
				Application.Current.Resources.MergedDictionaries.Add(blueResources);
				Application.Current.Resources.MergedDictionaries.Add(baseLightResources);
				Application.Current.Resources.MergedDictionaries.Add(flatButtonsResources);


			}
		}
	}
}
