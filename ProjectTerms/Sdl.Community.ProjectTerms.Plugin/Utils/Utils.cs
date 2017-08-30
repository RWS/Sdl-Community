using Sdl.Community.ProjectTerms.Plugin.Exceptions;
using Sdl.ProjectAutomation.FileBased;
using Sdl.TranslationStudioAutomation.IntegrationApi;
using System;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Sdl.Community.ProjectTerms.Plugin.Utils
{
    public static class Utils
    {
        private static void RemoveDirectoryFiles(string directoryPath)
        {
            foreach (var file in Directory.GetFiles(directoryPath))
            {
                File.Delete(file);
            }
        }

        public static void RemoveDirectory(string directoryPath)
        {
            try
            {
                RemoveDirectoryFiles(directoryPath);
                Directory.Delete(directoryPath);
            }
            catch (Exception e)
            {
                throw new ProjectTermsException(PluginResources.Error_RemoveDirectory + e.Message);
            }
        }

        public static void CreateDirectory(string directoryPath)
        {
            try
            {
                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }
                else
                {
                    RemoveDirectoryFiles(directoryPath);
                }
            }
            catch (Exception e)
            {

                throw new ProjectTermsException(PluginResources.Error_CreateDirectory + e.Message);
            }
        }

        public static bool IsRegexPatternValid(String pattern)
        {
            try
            {
                new Regex(pattern);
                return true;
            }
            catch { }
            return false;
        }

        public static bool CheckSingleFileProject()
        {
            var project = SdlTradosStudio.Application.GetController<ProjectsController>().CurrentProject;
            FieldInfo projectVal = typeof(FileBasedProject).GetField(PluginResources.Constant_ProjectType, BindingFlags.NonPublic | BindingFlags.Instance);

            dynamic projectValDynamic = projectVal.GetValue(project);
            dynamic projectType = projectValDynamic.ProjectType != null ? projectValDynamic.ProjectType : string.Empty;

            string projectTypeContent = Convert.ToString(projectType);

            return projectTypeContent.Equals(PluginResources.Constant_ProjectTypeContent);
        }
    }
}
