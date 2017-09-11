using Sdl.Community.ProjectTerms.Controls.Interfaces;
using System.Collections.Generic;
using Sdl.Community.ProjectTerms.Controls.Utils;
using Sdl.ProjectAutomation.Core;
using Sdl.Community.ProjectTerms.Plugin.ExportTermsToXML;
using System.IO;
using Sdl.Community.ProjectTerms.Plugin.Utils;
using System;
using System.ComponentModel;
using Sdl.TranslationStudioAutomation.IntegrationApi;
using System.Linq;
using Sdl.Community.ProjectTerms.Plugin.Exceptions;
using Sdl.ProjectAutomation.FileBased;

namespace Sdl.Community.ProjectTerms.Plugin
{
    public class ProjectTermsViewModel
    {
        public static ProjectTermsViewModel Instance = new ProjectTermsViewModel();
        private ProjectTermsExtractor extractor;
        private ProjectTermsCache cache;
        private List<ProjectFile> sourceProjectFilesToProcessed;

        public Filters Filters { get; set; }
        public IEnumerable<ITerm> Terms { get; set; }

        public FileBasedProject Project { get; set; }
        private ProjectsController ProjectsController { get; set; }
        public bool ProjectSelected { get; set; }

        public event EventHandler SelectedProjectChanged;
        public event EventHandler<Utils.ProgressEventArgs> Progress;

        public ProjectTermsViewModel()
        {
            Initialize();
        }

        public void Initialize()
        {
            extractor = new ProjectTermsExtractor();
            cache = new ProjectTermsCache();

            ProjectsController = SdlTradosStudio.Application.GetController<ProjectsController>();
            ProjectsController.SelectedProjectsChanged += ProjectsController_SelectedProjectsChanged;
            OnSelectedProjectsChanged();
        }

        void ProjectsController_SelectedProjectsChanged(object sender, EventArgs e)
        {
            OnSelectedProjectsChanged();
        }

        private void OnSelectedProjectsChanged()
        {
            List<FileBasedProject> projects = ProjectsController.SelectedProjects.ToList();

            Project = projects.Count == 1 ? projects[0] : null;

            if (Project != null)
            {
                if (File.Exists(Utils.Utils.GetXMLFilePath(Utils.Utils.GetProjecPath(), true)))
                {
                    Terms = cache.ReadXmlFile(Utils.Utils.GetXMLFilePath(Utils.Utils.GetProjecPath(), true));
                }
                else
                {
                    Terms = null;
                }
            }
            else
            {
                Terms = null;
            }

            if (SelectedProjectChanged != null)
            {
                SelectedProjectChanged(this, EventArgs.Empty);
            }
        }

        #region Utils
        private List<ProjectFile> GetFiles()
        {
            List<ProjectFile> sourceFilesToProcessed = new List<ProjectFile>();

            var projectSourceFiles = SdlTradosStudio.Application.GetController<ProjectsController>().CurrentProject.GetSourceLanguageFiles();
            if (ProjectSelected)
            {
                foreach (var file in projectSourceFiles)
                {
                    if (!file.Name.Contains(SdlTradosStudio.Application.GetController<ProjectsController>().CurrentProject.GetProjectInfo().Name))
                    {
                        sourceFilesToProcessed.Add(file);
                    }
                }
            }
            else
            {
                List<ProjectFile> selectedFiles = (SdlTradosStudio.Application.GetController<FilesController>().SelectedFiles).ToList();
                foreach (var file in selectedFiles)
                {
                    sourceFilesToProcessed.Add(projectSourceFiles.FirstOrDefault(x => x.Name == file.Name));
                }
            }

            return sourceFilesToProcessed;
        }

        private bool CheckXmlProjectTermsFileExists()
        {
            var sourceFiles = Utils.Utils.GetCurrentProject().GetSourceLanguageFiles();
            foreach (var file in sourceFiles)
            {
                if (file.Name.Contains(Utils.Utils.GetCurrentProject().GetProjectInfo().Name))
                {
                    return true;
                }
            }

            return false;
        }
        #endregion

        #region Extract Project Terms
        public void ExtractProjectTermsAsync(Action<ProjectTermsCloudResult> resultCallback, Action<int> progressCallback)
        {
            var worker = new BackgroundWorker();
            worker.WorkerReportsProgress = true;

            worker.DoWork += (sender, e) =>
            {
                extractor.Progress += (s, p) => {
                    if (worker.IsBusy)
                    {
                        worker.ReportProgress(p.Percent);
                    }
                };

                ExtractProjectTerms();
            };
            worker.RunWorkerCompleted += (sender, e) =>
            {
                var result = new ProjectTermsCloudResult();
                if (e.Error != null)
                {
                    result.Exception = e.Error;
                }

                resultCallback(result);
            };
            worker.ProgressChanged += (sender, e) => { progressCallback(e.ProgressPercentage); };
            worker.RunWorkerAsync();
        }

        public void ExtractProjectTerms()
        {
            sourceProjectFilesToProcessed = GetFiles();
            var projectPath = Utils.Utils.GetProjecPath();
            var xmlWordCloudFilePath = Utils.Utils.GetXMLFilePath(projectPath, true);

            if (File.Exists(xmlWordCloudFilePath))
            {
                File.Delete(xmlWordCloudFilePath);
            }

            ExtractTermsCache(projectPath, sourceProjectFilesToProcessed, xmlWordCloudFilePath);
        }

        public void ReadProjectTermsFromFile()
        {
            Terms = cache.ReadXmlFile(Utils.Utils.GetXMLFilePath(Utils.Utils.GetProjecPath(), true));
        }

        private void ExtractTermsCache(string projectPath, List<ProjectFile> files, string xmlWordCloudFilePath)
        {
            extractor.ExtractProjectFilesTerms(files);

            var terms = extractor.GetSourceTerms();
            Terms = terms.CountOccurences().SortByOccurences();

            cache.Save(projectPath, Terms, true);
        }
        #endregion

        #region Generate Cloud
        private void FilterProjectTerms()
        {
            ReadProjectTermsFromFile();
            Terms = Terms
                .FilterByBlackList(Filters.Blacklist)
                .FilterByOccurrences(Filters.Occurrences)
                .FilterByLength(Filters.Length);
        }

        public void GenerateWordCloudAsync(Action<ProjectTermsCloudResult> resultCallback)
        {
            BackgroundWorker worker = new BackgroundWorker();

            worker.DoWork += (sender, e) =>
            {
                FilterProjectTerms();
                e.Result = Terms;
            };
            worker.RunWorkerCompleted += (sender, e) =>
            {
                ProjectTermsCloudResult result = new ProjectTermsCloudResult();
                if (e.Error != null)
                {
                    result.Exception = e.Error;
                }
                else
                {
                    result.WeightedTerms = (IEnumerable<ITerm>)e.Result;
                }

                resultCallback(result);
            };
            worker.RunWorkerAsync();
        }

        #endregion

        #region Include the xml file to the Studio project
        private void GenerateXmlTermsFile()
        {
            FilterProjectTerms();
            cache.Save(Utils.Utils.GetProjecPath(), Terms);
        }

        public void AddXMlFileToProject()
        {
            GenerateXmlTermsFile();
            OnProgress(15);

            var xmlFolder = Path.GetDirectoryName(Utils.Utils.GetXMLFilePath(Utils.Utils.GetProjecPath()));
            if (CheckXmlProjectTermsFileExists())
            {
                // Todo: to remove the existed file with reflexion
            }
            OnProgress(30);

            IncludeFileToProject(Utils.Utils.GetCurrentProject(), xmlFolder, false);

            Utils.Utils.RemoveDirectory(xmlFolder);
        }

        private void IncludeFileToProject(IProject project, string xmlFolder, bool recursion)
        {
            try
            {
                project.AddFolderWithFiles(xmlFolder, recursion);
                var projectFiles = project.GetSourceLanguageFiles();
                var scan = project.RunAutomaticTask(projectFiles.GetIds(), AutomaticTaskTemplateIds.Scan);
                OnProgress(70);
                var convertTask = project.RunAutomaticTask(projectFiles.GetIds(), AutomaticTaskTemplateIds.ConvertToTranslatableFormat);
                OnProgress(90);
                var copyTask = project.RunAutomaticTask(projectFiles.GetIds(), AutomaticTaskTemplateIds.CopyToTargetLanguages);
                var preTran = project.RunAutomaticTask(projectFiles.GetIds(), AutomaticTaskTemplateIds.PreTranslateFiles);
                OnProgress(100);
            }
            catch (Exception e)
            {
                throw new ProjectTermsException(PluginResources.Error_AddXMlToProject + e.Message);
            }
        }

        private void OnProgress(int percent)
        {
            if (Progress != null)
            {
                Progress(this, new Utils.ProgressEventArgs { Percent = percent });
            }
        }

        public void AddXMlFileToProjectAsync(Action<ProjectTermsCloudResult> resultCallback, Action<int> progressCallback)
        {
            var worker = new BackgroundWorker();
            worker.WorkerReportsProgress = true;

            worker.DoWork += (sender, e) =>
            {
                this.Progress += (s, p) =>
                {
                    if (worker.IsBusy)
                    {
                        worker.ReportProgress(p.Percent);
                    }
                };

                AddXMlFileToProject();
            };
            worker.RunWorkerCompleted += (sender, e) =>
            {
                var result = new ProjectTermsCloudResult();
                if (e.Error != null)
                {
                    result.Exception = e.Error;
                }

                resultCallback(result);
            };
            worker.ProgressChanged += (sender, e) => { progressCallback(e.ProgressPercentage); };
            worker.RunWorkerAsync();
        }
        #endregion
    }
}
