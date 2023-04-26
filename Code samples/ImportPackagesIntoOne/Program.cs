using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sdl.ProjectAutomation.Core;
using Sdl.ProjectAutomation.FileBased;

namespace ImportPackagesIntoOne
{
    internal class Program
    {
        static void Main(string[] args)
        {
	        var projectPackageFilePath = @"C:\Things\Work\Dev\Community\CRQ-33423\Project 1.sdlppx";
	        var projectPackageFilePath2 = @"C:\Things\Work\Dev\Community\CRQ-33423\Project 2.sdlppx";

	        var projectFolder = @"C:\Things\Work\Dev\Community\CRQ-33423\MergedFilesProject";
	        var projectFolder2 = @"C:\Things\Work\Dev\Community\CRQ-33423\TemporaryProject";

	        if (Directory.Exists(projectFolder))
	        {
		        Directory.Delete(projectFolder, true);
		        Directory.Delete(projectFolder2, true);
	        }

	        var project = FileBasedProject.CreateFromProjectPackage(projectPackageFilePath, projectFolder, out var packageImport);
	        var project2 = FileBasedProject.CreateFromProjectPackage(projectPackageFilePath2, projectFolder2, out var packageImport2);

	        var projectSourceFiles2 = project2.GetSourceLanguageFiles();


	        var newlyAddedProjectFiles = project.AddFiles(projectSourceFiles2.Select(pf => pf.LocalFilePath).ToArray());
	        project.RunAutomaticTasks(newlyAddedProjectFiles.GetIds(), new[]
	        {
		        AutomaticTaskTemplateIds.Scan,
		        AutomaticTaskTemplateIds.ConvertToTranslatableFormat,
		        AutomaticTaskTemplateIds.CopyToTargetLanguages,
	        });
	        project.Save();

	        var projectFiles = project.GetSourceLanguageFiles();

	        foreach (var projectFile in projectFiles)
	        {
		        var targetFiles = projectFile.TargetFiles;
		        var correspondingFile = projectSourceFiles2.FirstOrDefault(pf => pf.Name == projectFile.Name);

		        if (correspondingFile == null)
			        continue;

		        foreach (var targetFile in targetFiles)
		        {
			        var localFilePath = correspondingFile.TargetFiles.FirstOrDefault(pf => pf.Language.Equals(targetFile.Language)).LocalFilePath;
			        project.AddNewFileVersion(targetFile.Id, localFilePath);
		        }
	        }
		}
    }
}
