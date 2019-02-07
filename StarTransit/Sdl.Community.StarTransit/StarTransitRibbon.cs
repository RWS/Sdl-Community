using System;
using System.Globalization;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Forms;
using Sdl.Community.StarTransit.Shared.Models;
using Sdl.Community.StarTransit.Shared.Services;
using Sdl.Community.StarTransit.UI;
using Sdl.Community.StarTransit.UI.Controls;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.TranslationStudioAutomation.IntegrationApi;
using Sdl.TranslationStudioAutomation.IntegrationApi.Presentation.DefaultLocations;
using Application = System.Windows.Application;

namespace Sdl.Community.StarTransit
{
	[RibbonGroup("Sdl.Community.StarTransit", Name = "StarTransit", ContextByType = typeof(ProjectsController))]
	[RibbonGroupLayout(LocationByType = typeof(TranslationStudioDefaultRibbonTabs.HomeRibbonTabLocation))]
	public class StarTransitRibbon : AbstractRibbonGroup
	{
	}

	[Action("Sdl.Community.StarTransit", Name = "Open StarTransit Package", Icon = "open_package", Description = "Open a StarTransit package")]
	[ActionLayout(typeof(StarTransitRibbon), 20, DisplayType.Large)]
	public class StarTransitOpenPackageAction : AbstractAction
	{
		protected override async void Execute()
		{
			EnsureApplicationResources();

			var pathToTempFolder = CreateTempPackageFolder();
			try
			{
				var fileDialog = new OpenFileDialog
				{
					Filter = @"Transit Project Package Files (*.ppf)|*.ppf"
				};
				var dialogResult = fileDialog.ShowDialog();
				if (dialogResult == DialogResult.OK)
				{
					var path = fileDialog.FileName;	
					var packageService = new PackageService();
					var package = await packageService.OpenPackage(path, pathToTempFolder);

					var templateService = new TemplateService();
					var templateList = templateService.LoadProjectTemplates();

					var packageModel = new PackageModel
					{
						Name = package.Name,
						Description = package.Description,
						StudioTemplates = templateList,
						LanguagePairs = package.LanguagePairs,
						PathToPrjFile = package.PathToPrjFile
					};

					// Start BackgroundWorder in InitializeMain method to have app working separately than Trados Studio process
					Program.InitializeMain(packageModel);
				}
			}
			catch (PathTooLongException ptle)
			{
				System.Windows.Forms.MessageBox.Show(ptle.Message);
			}
		}  
		
		private string CreateTempPackageFolder()
		{
			var tempFolder = $@"C:\Users\{Environment.UserName}\StarTransit";
			var pathToTempFolder = Path.Combine(tempFolder, Guid.NewGuid().ToString());
			
			if (Directory.Exists(pathToTempFolder))
			{
				Directory.Delete(pathToTempFolder, true);
			}
			Directory.CreateDirectory(pathToTempFolder);
			return pathToTempFolder;
		}

		private void EnsureApplicationResources()
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
				var greenResources = new ResourceDictionary
				{
					Source = new Uri("pack://application:,,,/MahApps.Metro;component/Styles/Accents/Green.xaml")
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
				Application.Current.Resources.MergedDictionaries.Add(greenResources);
				Application.Current.Resources.MergedDictionaries.Add(baseLightResources);
				Application.Current.Resources.MergedDictionaries.Add(flatButtonsResources);
			}
		}
	}

	[Action("Sdl.Community.StarTransit.Return", Name = "StarTransit return package", Icon = "return_package", Description = "StarTransit return package")]
	[ActionLayout(typeof(StarTransitRibbon), 20, DisplayType.Large)]

	public class ReturnPackageAction : AbstractAction
	{
		protected override void Execute()
		{
			EnsureApplicationResources();
			var returnService = new ReturnPackageService();
			var returnPackage = returnService.GetReturnPackage();

			if (returnPackage.Item2 != string.Empty)
			{
				System.Windows.Forms.MessageBox.Show(returnPackage.Item2, @"Warning",
					MessageBoxButtons.OK, MessageBoxIcon.Warning);
			}
			else
			{
				ReturnPackageMainWindow window = new ReturnPackageMainWindow(returnPackage.Item1);
				window.ShowDialog();
			}
		}

		private void EnsureApplicationResources()
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

	[Action("Sdl.Community.StarTransit.Contribute", Name = "Contribute to project", Icon = "opensourceimage", Description = "Contribute to project")]
	[ActionLayout(typeof(StarTransitRibbon), 20, DisplayType.Large)]
	public class ContributeToProjectAction : AbstractAction
	{
		protected override void Execute()
		{
			System.Diagnostics.Process.Start("https://github.com/sdl/Sdl-Community/tree/master/StarTransit");
		}
	}

	[Action("Sdl.Community.StarTransit.Help", Name = "Help", Icon = "help_icon", Description = "Help")]
	[ActionLayout(typeof(StarTransitRibbon), 20, DisplayType.Large)]
	public class HelpLinkAction : AbstractAction
	{
		protected override void Execute()
		{
			System.Diagnostics.Process.Start("https://community.sdl.com/product-groups/translationproductivity/w/customer-experience/3270.star-transit");
		}
	}
}