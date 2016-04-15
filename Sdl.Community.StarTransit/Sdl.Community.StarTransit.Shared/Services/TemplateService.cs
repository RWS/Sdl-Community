using System.Collections.Generic;
using System.Linq;
using Sdl.Desktop.IntegrationApi;
using Sdl.ProjectAutomation.Core;
using Sdl.TranslationStudioAutomation.IntegrationApi;

namespace Sdl.Community.StarTransit.Shared.Services
{
   public class TemplateService: AbstractViewControllerAction<ProjectsController>
   {
       protected override void Execute()
       {
     
       }

       public List<ProjectTemplateInfo> LoadProjectTemplates()
       {
           var controller = Controller;
           var templateList = controller.GetProjectTemplates().ToList();

           return templateList;
       }
   }
}
