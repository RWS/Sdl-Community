using Sdl.Community.StarTransit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sdl.Community.StarTransit.Shared.Models;
using Sdl.Community.StarTransit.Shared.Services;

namespace TestApp
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            var packageService = new PackageService();
            var package = packageService.OpenPackage(@"C:\Users\aghisa\Desktop\Transit packages\PACK_00000228_TRA_HUN_00_HUN.PPF");
            var templateService = new TemplateService();
            var templateList = templateService.LoadProjectTemplates();
            var packageModel = new PackageModel
            {
                Name = package.Name,
                Description = package.Description,
                StudioTemplates = templateList,
                SourceLanguage = package.SourceLanguage,
                TargetLanguage = package.TargetLanguage,
                SourceFiles = package.SourceFiles,
                TargetFiles = package.TargetFiles
            };

            StarTransitMainWindow mainWindow = new StarTransitMainWindow(packageModel);
            mainWindow.ShowDialog();
        }
    }
}
