using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Sdl.Community.StarTransit.Shared.Models;
using Sdl.Desktop.IntegrationApi;
using Sdl.ProjectAutomation.Core;
using Sdl.TranslationStudioAutomation.IntegrationApi;

namespace Sdl.Community.StarTransit.Shared.Services
{
    public class ReturnPackageService: AbstractViewControllerAction<ProjectsController> 

    {
        public ReturnPackageService()
        {

        }

        /// <summary>
        /// Returns StarTransit return package and  true if the project selected is a StarTransit project 
        /// </summary>
        /// <returns></returns>
        public Tuple<ReturnPackage, bool> GetReturnPackage()
        {
            var projects = Controller.SelectedProjects;
            var returnPackage = new ReturnPackage();
            var targetFiles = new List<ProjectFile>();
            bool isTransit;

            foreach (var project in projects)
            {
              
                var target = project.GetTargetLanguageFiles().ToList();
                isTransit=IsTransitProject(target);
                if (isTransit)
                {
                    targetFiles.AddRange(target);
                }

            }
            returnPackage.TargetFiles = targetFiles;

            if (targetFiles.Count != 0)
            {
                isTransit = true;
            }
            else
            {
                isTransit = false;
            }
            var package = new Tuple<ReturnPackage, bool>(returnPackage,isTransit);

            return package;
        }

        /// <summary>
        /// Check to see if the file type is the same with the Transit File Type
        /// </summary>
        /// <param name="filesPath"></param>
        /// <returns></returns>
        public bool IsTransitProject(List<ProjectFile> filesPath)
        {
            var areTranstFiles = new List<bool>();
           foreach (var file in filesPath)
            {
                if (file.FileTypeId.Equals("Transit File Type 1.0.0.0"))
                {
                    areTranstFiles.Add(true);
                }
                else
                {
                    areTranstFiles.Add(false);
                    return  false;
                }
            }

            return true;
        }
        protected override void Execute()
        {
            
        }
    }
}
