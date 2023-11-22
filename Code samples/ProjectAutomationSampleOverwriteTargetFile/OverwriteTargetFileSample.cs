using ProjectAutomationDataProtectionSuiteSample;
using Sdl.ProjectAutomation.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sdl.Core.Globalization;
using Sdl.ProjectAutomation.FileBased;
using Sdl.ProjectAutomation.Settings;

namespace ProjectAutomationSample
{
    internal class OverwriteTargetFileSample
    {
        const string SOURCE_FILE = "C:\\john\\OneDrive - SDL\\test3667\\constitution en short.txt";

        // create a test project we can reference later - I just need the path, to simulate the user's issue
        public static string CreateTestProject()
        {
            var settings = ProjectUtil.ProjectSettings(SOURCE_FILE, "en-US", "de-de");

            Console.WriteLine($"creating project {settings.ProjectName} from {settings.InputFolder} to {settings.OutputFolder}");
            var project = ProjectUtil.CreateProject(settings);
            // run automatic tasks
            bool ok = true;
            ok = ProjectUtil.TryRunSourceTaskAndAnalyze(project, AutomaticTaskTemplateIds.Scan) && ok;

            project.SetFileRole(project.GetSourceLanguageFiles().Select(f => f.Id).ToArray(), FileRole.Translatable);

            project.RunDefaultTaskSequence(project.GetSourceLanguageFiles().Select(p => p.Id).ToArray());
            project.Save();
            return project.FilePath;
        }

        // the reason I duplicate the project is to easily find the target's file target language
        // 1. first, since the target file used to exist in the project, I will look at the existing project,
        //    and create a new project with all those possible target languages. 
        // 2. import the target file -- since I used all the source project's target languages, the import will now work
        //
        // note: the duplicated project is created in Temp folder, and you can later delete it yourself
        private static FileBasedProject DuplicateProjectTargetLanguages(FileBasedProject project, string targetFile)
        {
            var targetLanguages = project.GetTargetLanguageFiles().Select(f => f.Language.IsoAbbreviation).ToList();
            var sourceLanguage = project.GetSourceLanguageFiles()[0].Language.IsoAbbreviation;
            var settings = ProjectUtil.TempProjectSettings(targetFile, sourceLanguage, targetLanguages);
            var duplicate = ProjectUtil.CreateProject(settings);

            var sourceIds = duplicate.GetSourceLanguageFiles().Select(f => f.Id).ToArray();
            ProjectUtil.VerifyAutomaticTask(duplicate.RunAutomaticTask(sourceIds, AutomaticTaskTemplateIds.Scan));
            ProjectUtil.VerifyAutomaticTask(duplicate.RunAutomaticTask(sourceIds, AutomaticTaskTemplateIds.ConvertToTranslatableFormat));
            ProjectUtil.VerifyAutomaticTask(duplicate.RunAutomaticTask(sourceIds, AutomaticTaskTemplateIds.CopyToTargetLanguages));

            var targetIds = duplicate.GetTargetLanguageFiles().Select(f => f.Id).ToArray();
            ProjectUtil.VerifyAutomaticTask(duplicate.RunAutomaticTask(targetIds, AutomaticTaskTemplateIds.AnalyzeFiles));
            duplicate.Save();

            return duplicate;
        }

        // We assume you kept the original file name of the target file (the path doesn't matter)
        // we need this in order to find out the location of the target file in your project, so that 
        // we overwrite the correct file, in the correct language
        public static void CopyTargetLanguageFileToProject(string projectPath, string targetFile)
        {
            var project = new FileBasedProject(projectPath);

            var duplicate = DuplicateProjectTargetLanguages(project, targetFile);
            var targetLanguage = duplicate.GetTargetLanguageFiles()[0].Language.IsoAbbreviation;

            // IMPORTANT: here, we assume the file is already in the project, thus, if we overwrite a file, its ID will remain the same
            // otherwise, you'd need to reload the project
            if (project.GetTargetLanguageFiles().Length == 1) 
                File.Copy(targetFile, project.GetTargetLanguageFiles()[0].LocalFilePath, true);
            else 
            {
                // here, we care about the target language
                var fileName = Path.GetFileName(targetFile);
                var foundTargetFile = project.GetTargetLanguageFiles(new Language(targetLanguage)).FirstOrDefault(f => f.Name == fileName);
                if (foundTargetFile != null)
                    File.Copy(targetFile, foundTargetFile.LocalFilePath, true);
                else
                    throw new Exception($"Can't find matching target file in project for {targetFile}");
            }

            var targetIds = project.GetTargetLanguageFiles().Select(f => f.Id).ToArray();
            ProjectUtil.VerifyAutomaticTask(project.RunAutomaticTask(targetIds, AutomaticTaskTemplateIds.AnalyzeFiles));
            ProjectUtil.VerifyAutomaticTask(project.RunAutomaticTask(targetIds, AutomaticTaskTemplateIds.TranslationCount));
            project.Save();

            Console.WriteLine($"test complete - {projectPath}");
            Console.ReadKey();
        }




    }
}
