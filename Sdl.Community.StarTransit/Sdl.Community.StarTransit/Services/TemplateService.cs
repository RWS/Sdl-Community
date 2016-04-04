using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sdl.Desktop.IntegrationApi;
using Sdl.ProjectAutomation.Core;
using Sdl.TranslationStudioAutomation.IntegrationApi;

namespace Sdl.Community.StarTransit.Services
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
