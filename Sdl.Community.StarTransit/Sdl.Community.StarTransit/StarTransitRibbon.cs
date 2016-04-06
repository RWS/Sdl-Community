using Sdl.Community.StarTransit.UI;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.TranslationStudioAutomation.IntegrationApi;
using Sdl.TranslationStudioAutomation.IntegrationApi.Presentation.DefaultLocations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Sdl.Community.StarTransit.Services;
using Sdl.Community.StarTransit.Shared.Models;

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
        protected override void Execute()
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Filter = "Transit Project Package Files (*.ppf)|*.ppf";
            var dialogResult = fileDialog.ShowDialog();
            if(dialogResult == DialogResult.OK)
            {
                var path = fileDialog.FileName;

                //deschide arhiva si citeste fisierul .prj
                var packageService = new PackageService();
                var package = packageService.OpenPackage(path);

                //lista de template din studio
                var templateService = new TemplateService();
                var templateList = templateService.LoadProjectTemplates();

                
                var packageModel = new PackageModel
                {
                    Name = package.Name,
                    Description =package.Description,
                    ProjectTemplate = templateList,
                    SourceLanguage = package.SourceLanguage,
                    TargetLanguage = package.TargetLanguage
                };

                //creaza un proiect studio
                //var projectService = new ProjectService();
                //projectService.CreateProject(packageModel);
                StarTransitMainWindow window = new StarTransitMainWindow(packageModel);
                window.ShowDialog();
            }
        }
    }
    
}
