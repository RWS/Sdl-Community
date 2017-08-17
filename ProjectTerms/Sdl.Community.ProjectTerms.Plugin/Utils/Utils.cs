using Sdl.Community.ProjectTerms.Plugin.Exceptions;
using System;
using System.IO;

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
    }
}
