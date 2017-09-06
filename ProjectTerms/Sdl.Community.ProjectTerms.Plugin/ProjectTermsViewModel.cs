using Sdl.Community.ProjectTerms.Controls.Interfaces;
using System.Collections.Generic;
using Sdl.Community.ProjectTerms.Controls.Utils;
using Sdl.ProjectAutomation.Core;
using Sdl.FileTypeSupport.Framework.IntegrationApi;
using Sdl.Community.ProjectTerms.Plugin.ExportTermsToXML;
using Sdl.ProjectAutomation.FileBased;
using System.IO;
using Sdl.FileTypeSupport.Framework.Core.Utilities.IntegrationApi;
using System.Linq;

namespace Sdl.Community.ProjectTerms.Plugin
{
    public class ProjectTermsViewModel
    {
        private ProjectTermsExtractor extractor;
        public IEnumerable<ITerm> Terms { get; set; }
        public IFileTypeManager FileTypeManager { get; set; }

        public ProjectTermsViewModel()
        {
            extractor = new ProjectTermsExtractor();
            FileTypeManager = DefaultFileTypeManager.CreateInstance(true);
        }

        public void ExtractProjectFileTerms(ProjectFile projectFile, IMultiFileConverter multiFileConverter)
        {
            extractor.ExtractProjectFileTerms(projectFile, multiFileConverter);
        }

        public void ExtractProjectTerms(int occurrences, int length, List<string> blackList, string projectPath)
        {
            IEnumerable<string> terms = extractor.GetSourceTerms();

            Terms = terms
                .FilterByBlackList(blackList)
                .CountOccurences()
                .FilterByOccurrences(occurrences)
                .FilterByLength(length);

            ProjectTermsCache cache = new ProjectTermsCache();

            cache.Save(projectPath, Terms);
        }

        public void ExtractProjectTerms(int occurrences, int length, List<string> blackList, string projectPath, List<ProjectFile> files, bool buttonWordCloudPressedOnce)
        {
            ProjectTermsCache cache = new ProjectTermsCache();
            string xmlWordCloudFilePath = Utils.Utils.GetXMLFilePath(projectPath, true);
            if (!File.Exists(xmlWordCloudFilePath))
            {
                ExtractTermsAndSave(projectPath, files, cache, xmlWordCloudFilePath);
            }
            else if (File.Exists(xmlWordCloudFilePath) && buttonWordCloudPressedOnce)
            {
                File.Delete(xmlWordCloudFilePath);
                ExtractTermsAndSave(projectPath, files, cache, xmlWordCloudFilePath);
            }
            else
            {
                Terms = cache.ReadXmlFile(xmlWordCloudFilePath);
            }

            Terms = Terms
                .FilterByBlackList(blackList)
                .FilterByOccurrences(occurrences)
                .FilterByLength(length)
                .SortByOccurences();
        }

        private void ExtractTermsAndSave(string projectPath, List<ProjectFile> files, ProjectTermsCache cache, string xmlWordCloudFilePath)
        {
            var extractor = new ProjectTermsExtractor();
            foreach (var file in files)
            {
                if (!Directory.Exists(Path.GetDirectoryName(xmlWordCloudFilePath)))
                    Directory.CreateDirectory(Path.GetDirectoryName(xmlWordCloudFilePath));

                IMultiFileConverter converter = FileTypeManager.GetConverter(file.LocalFilePath, (sender, e) => { });
                extractor.ExtractProjectFileTerms(file, converter);
            }

            IEnumerable<string> terms = extractor.GetSourceTerms();
            Terms = terms.CountOccurences().SortByOccurences();

            cache.Save(projectPath, Terms, true);
        }
    }
}
