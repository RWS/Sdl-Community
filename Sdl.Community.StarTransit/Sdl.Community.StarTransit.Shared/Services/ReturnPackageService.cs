using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Sdl.Community.StarTransit.Shared.Models;
using Sdl.Core.Settings;
using Sdl.Desktop.IntegrationApi;
using Sdl.ProjectApi.Settings;
using Sdl.ProjectAutomation.Core;
using Sdl.ProjectAutomation.FileBased;
using Sdl.TranslationStudioAutomation.IntegrationApi;

namespace Sdl.Community.StarTransit.Shared.Services
{
    public class ReturnPackageService: AbstractViewControllerAction<ProjectsController> 

    {
        public ReturnPackageService()
        {

        }

        /// <summary>
        /// Returns a list of StarTransit return package and  true if the projects selected are a StarTransit projects 
        /// </summary>
        /// <returns></returns>
        public Tuple<ReturnPackage,string> GetReturnPackage()
        {
            var projects = Controller.SelectedProjects.ToList();
            var message = string.Empty;
            if (projects.Count > 1)
            {
                message = @"Please select only one project.";           
                return new Tuple<ReturnPackage, string>(null,message);
            }

            List<bool> isTransitProject = new List<bool>();
            var returnPackage = new ReturnPackage();
            foreach (var project in projects)
            {
              
                var targetFiles = project.GetTargetLanguageFiles().ToList();
                var isTransit=IsTransitProject(targetFiles);

                if (isTransit)
                {
                    returnPackage.FileBasedProject = project;
                    returnPackage.ProjectLocation = project.FilePath;
                    returnPackage.TargetFiles = targetFiles;
                    //we take only the first file location, because the other files are in the same location
                    returnPackage.LocalFilePath = targetFiles[0].LocalFilePath;
                    isTransitProject.Add(true);
                }
                else
                {
                    isTransitProject.Add(false);
                }

            }
            
            if (isTransitProject.Contains(false))
            {
                message = @"Please select a StarTransit project!";
                return new Tuple<ReturnPackage,string>(returnPackage, message);
            }
            return new Tuple<ReturnPackage,string>(returnPackage,message);
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

                if (file.FileTypeId!=null &&file.FileTypeId.Equals("Transit File Type 1.0.0.0"))
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

        /// <summary>
        /// TO DO :The files are exported in studio project, they should be imported in a custom location
        /// </summary>
        /// <param name="package"></param>
        public void ExportFiles(ReturnPackage package)
        {
            
            var taskSequence = package.FileBasedProject.RunAutomaticTasks(package.TargetFiles.GetIds(), new string[]
            {
               AutomaticTaskTemplateIds.GenerateTargetTranslations
             
            });

            var outputFiles = taskSequence.OutputFiles.ToList();
           

              CreateArchive(package);

        }


        /// <summary>
        /// Creates an archive in the Return Package folder and add project files to it
        /// For the moment we add the files without runing any task on them
        /// </summary>
        /// <param name="package"></param>
        private void CreateArchive(ReturnPackage package)
        {
            var prjFileName = Path.GetFileNameWithoutExtension(package.PathToPrjFile);
            var archivePath = Path.Combine(package.FolderLocation, prjFileName+".tpf");

            var pathToTargetFileFolder = package.LocalFilePath.Substring(0, package.LocalFilePath.LastIndexOf(@"\", StringComparison.Ordinal));

            if (!File.Exists(archivePath))
            {
                //create the archive, and add files to it
                using (var archive = ZipFile.Open(archivePath, ZipArchiveMode.Create))
                {
                    archive.CreateEntryFromFile(package.PathToPrjFile, prjFileName, CompressionLevel.Optimal);
                    foreach (var file in package.TargetFiles)
                    {

                        var fileName = Path.GetFileNameWithoutExtension(file.LocalFilePath);

                        archive.CreateEntryFromFile(Path.Combine(pathToTargetFileFolder, fileName), fileName,
                            CompressionLevel.Optimal);

                    }

                }
            }
            else
            {
                UpdateArchive(archivePath, prjFileName, package, pathToTargetFileFolder);
            }

        }

        private void UpdateArchive(string archivePath, string prjFileName,ReturnPackage returnPackagePackage,string pathToTargetFileFolder)
        {
            //open the archive and delete old files
            // archvie in update mode not overrides existing files 
            using (var archive = ZipFile.Open(archivePath, ZipArchiveMode.Update))
            {
                var entriesColection = new ObservableCollection<ZipArchiveEntry>(archive.Entries);
                foreach (var entry in entriesColection)
                {
                    
                    if (entry.Name.Equals(prjFileName))
                    {
                        entry.Delete();
                    }

                    foreach (var project in returnPackagePackage.TargetFiles)
                    {
                        var projectFromArchiveToBeDeleted =
                            archive.Entries.FirstOrDefault(n => n.Name.Equals(Path.GetFileNameWithoutExtension(project.Name)));
                        if (projectFromArchiveToBeDeleted != null)
                        {
                            projectFromArchiveToBeDeleted.Delete();
                        }
                    }
                }
            }

            //add files to archive
            using (var archive = ZipFile.Open(archivePath, ZipArchiveMode.Update))
            {
                archive.CreateEntryFromFile(returnPackagePackage.PathToPrjFile, prjFileName, CompressionLevel.Optimal);
                foreach (var file in returnPackagePackage.TargetFiles)
                {
                    var fileName = Path.GetFileNameWithoutExtension(file.LocalFilePath);

                    archive.CreateEntryFromFile(Path.Combine(pathToTargetFileFolder, fileName), fileName,
                        CompressionLevel.Optimal);

                }

            }
        }
     
    }
}
