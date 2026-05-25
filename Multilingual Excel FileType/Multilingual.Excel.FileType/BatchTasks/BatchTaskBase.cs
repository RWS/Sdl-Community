using Multilingual.Excel.FileType.Constants;
using Multilingual.Excel.FileType.FileType.Processors;
using Multilingual.Excel.FileType.Services;
using Sdl.ProjectAutomation.AutomaticTasks;
using Sdl.ProjectAutomation.Core;
using Sdl.ProjectAutomation.FileBased;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Multilingual.Excel.FileType.BatchTasks
{
    public abstract class BatchTaskBase : AbstractFileContentProcessingAutomaticTask
    {
        protected static void UpdateFile(ProjectFile projectFile, SdlxliffUpdater sdlxliffUpdater)
        {
            var updatedFilePath = Path.GetTempFileName();
            sdlxliffUpdater.UpdateFile(projectFile.LocalFilePath, updatedFilePath);
            File.Copy(updatedFilePath, projectFile.LocalFilePath, true);
            if (File.Exists(updatedFilePath))
            {
                File.Delete(updatedFilePath);
            }
        }

        public override bool ShouldProcessFile(ProjectFile projectFile)
        {
            var valid = projectFile.FileTypeId == FiletypeConstants.FileTypeDefinitionId;
            if (!valid)
            {
                var message = string.Format(PluginResources.ExceptionMessage_Incorrect_File_Type,
                    projectFile.FileTypeId, FiletypeConstants.FileTypeDefinitionId);
                throw new Exception(message);
            }

            return true;
        }

        protected List<Models.AnalysisBand> GetAnalysisBands(FileBasedProject project)
        {
            var regex = new Regex(@"(?<min>[\d]*)([^\d]*)(?<max>[\d]*)", RegexOptions.IgnoreCase);

            var analysisBands = new List<Models.AnalysisBand>();
            var type = project.GetType();
            var internalProjectField = type.GetField("_project", BindingFlags.NonPublic | BindingFlags.Instance);
            if (internalProjectField != null)
            {
                dynamic internalDynamicProject = internalProjectField.GetValue(project);
                foreach (var analysisBand in internalDynamicProject.AnalysisBands)
                {
                    Match match = regex.Match(analysisBand.ToString());
                    if (match.Success)
                    {
                        var min = match.Groups["min"].Value;
                        var max = match.Groups["max"].Value;
                        analysisBands.Add(new Models.AnalysisBand
                        {
                            MinimumMatchValue = Convert.ToInt32(min),
                            MaximumMatchValue = Convert.ToInt32(max)
                        });
                    }
                }
            }

            return analysisBands;
        }
    }
}
