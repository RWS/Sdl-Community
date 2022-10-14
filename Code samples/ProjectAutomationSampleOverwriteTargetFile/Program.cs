
namespace ProjectAutomationSample
{
    internal class Program
    {
        // https://jira.sdl.com/browse/SDLCOM-3667
        // AutomaticTaskTemplateIds.TranslationCount does not change the ConfirmationStatistics.CombinedConfirmationLevel
        static void Main(string[] args)
        {
            // this just gets us the path to a project - in your specific case, it's the actual path to your project
            var projectPath = OverwriteTargetFileSample.CreateTestProject();
            var targetTranslatedFile = "C:\\john\\OneDrive - SDL\\test3667\\constitution en short.txt.sdlxliff";
            OverwriteTargetFileSample.CopyTargetLanguageFileToProject(projectPath, targetTranslatedFile);
        }
    }
}
