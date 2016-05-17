using Sdl.Community.StarTransit.UI;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.TranslationStudioAutomation.IntegrationApi;
using Sdl.TranslationStudioAutomation.IntegrationApi.Presentation.DefaultLocations;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using Sdl.Community.StarTransit.Shared.Models;
using Sdl.Community.StarTransit.Shared.Services;
using Sdl.Community.StarTransit.Shared.Utils;
using Application = System.Windows.Application;


namespace Sdl.Community.StarTransit
{
    [RibbonGroup("Sdl.Community.StarTransit", Name = "StarTransit", ContextByType = typeof(ProjectsController))]
    [RibbonGroupLayout(LocationByType = typeof(TranslationStudioDefaultRibbonTabs.HomeRibbonTabLocation))]
    public class StarTransitRibbon : AbstractRibbonGroup
    {
    }

    [Action("Sdl.Community.StarTransit", Name ="Open StarTransit Package", Icon ="icon",Description ="Open a StarTransit package")]
    [ActionLayout(typeof(StarTransitRibbon),20,DisplayType.Large)]
    public class StarTransitOpenPackageAction: AbstractAction
    {
        protected override async void Execute()
        {
            EnsureApplicationResources();
            TelemetryService.Instance.Init();

            // check for new version
            await TelemetryService.Instance.CheckForUpdates(true);
            TelemetryService.Instance.SendCrashes(false);
            var pathToTempFolder = CreateTempPackageFolder();
            try
            {
                OpenFileDialog fileDialog = new OpenFileDialog();
                fileDialog.Filter = @"Transit Project Package Files (*.ppf)|*.ppf";
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
                       LanguagePairs = package.LanguagePairs
                    };
                    StarTransitMainWindow window = new StarTransitMainWindow(packageModel);
                    window.ShowDialog();
                }
            }
            catch (Exception e)
            {
                TelemetryService.Instance.HandleException(e);
            }
            finally
            {
                if (Directory.Exists(pathToTempFolder))
                {
                    Directory.Delete(pathToTempFolder, true);
                }
            }

        }

        private string CreateTempPackageFolder()
        {
            var pathToTempFolder = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());

            if (Directory.Exists(pathToTempFolder))
            {
                Directory.Delete(pathToTempFolder);
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
            System.Diagnostics.Process.Start("https://github.com/sdl/Sdl-Community/issues?q=is%3Aopen+is%3Aissue+label%3AStarTransit");
        }
    }


}
