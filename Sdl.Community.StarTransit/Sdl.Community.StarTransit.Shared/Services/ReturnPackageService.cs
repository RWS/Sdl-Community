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
            ChangeMetadataFile(package.PathToPrjFile);

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

        /// <summary>
        /// Reads the prj file and check if an update has been made already
        /// </summary>
        /// <param name="pathToPrjFile"></param>
        private void ChangeMetadataFile(string pathToPrjFile)
        {
            var metadata = new Metadata
            {
                ExchangedDate = Utils.CustomDateTime.CustomExchangeDate(DateTime.Now),
                ExchangedTime = Utils.CustomDateTime.CustomExchangeTime(DateTime.Now),
                LastChangedDate = Utils.CustomDateTime.CreateCustomDate(DateTime.Now)
            };

          

            using (var reader = new StreamReader(pathToPrjFile))
            {
                string fileContent = reader.ReadToEnd();
                reader.Close();

                if (fileContent.Contains("PromptForNewWorkingDir"))
                {
                    //that means we need to add "PromptForNewWorkingDir" line
                    MedatataBuilder(pathToPrjFile, metadata,false);
                }
                else
                {
                    //the prj has been edited before so we don't need to add this line anymore, just update
                    MedatataBuilder(pathToPrjFile, metadata, true);
                }
                    
            }
        }

        /// <summary>
        /// Changes the metadata from prj file
        /// </summary>
        /// <param name="pathToPrjFile"></param>
        /// <param name="metadata"></param>
        /// <param name="createNewMetadata"></param>
        private void MedatataBuilder(string pathToPrjFile, Metadata metadata,bool createNewMetadata)
        {
            var builder = new StringBuilder();
            var lines = File.ReadAllLines(pathToPrjFile).ToList();
            foreach (var line in lines)
            {
                if (line.StartsWith("LastChangedDate"))
                {
                    var newLine = line.Replace(line, string.Concat("LastChangedDate=", metadata.LastChangedDate));
                    builder.Append(newLine + Environment.NewLine);
                    continue;

                }

                //check to see if the project was edited before
                //if yes, that means we don't need to add this line anymore
                if (createNewMetadata)
                {
                    if (line.Equals("[Exchange]"))
                    {
                        builder.AppendFormat("{0}{1}{2}{3}", "[Exchange]", Environment.NewLine, "PromptForNewWorkingDir=0", Environment.NewLine);

                        continue;
                    }
                }
          
                if (line.StartsWith("IsExchange"))
                {
                    var newLine = line.Replace(line, "IsExchange=1");
                    builder.Append(newLine + Environment.NewLine);
                    continue;
                }
                if (line.StartsWith("ExchangeDate"))
                {
                    var date = line.Replace(line, string.Concat("ExchangeDate=", metadata.ExchangedDate));
                    builder.Append(date + Environment.NewLine);
                    continue;
                }
                if (line.StartsWith("ExchangeTime"))
                {
                    var time = line.Replace(line, string.Concat("ExchangeTime=", metadata.ExchangedTime));
                    builder.Append(time + Environment.NewLine);
                    continue;
                }
                builder.Append(line + Environment.NewLine);
            }

            File.WriteAllText(pathToPrjFile, builder.ToString());
        }



    }
}
