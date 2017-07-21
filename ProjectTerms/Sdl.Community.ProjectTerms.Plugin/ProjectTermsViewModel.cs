using Sdl.Community.ProjectTerms.Controls.Interfaces;
using System.Collections.Generic;
using Sdl.Community.ProjectTerms.Controls.Utils;
using Sdl.ProjectAutomation.Core;
using Sdl.FileTypeSupport.Framework.IntegrationApi;

namespace Sdl.Community.ProjectTerms.Plugin
{
    public class ProjectTermsViewModel
    {
        private ProjectTermsExtractor extractor;
        public IEnumerable<IWord> Terms { get; set; }

        public ProjectTermsViewModel()
        {
            extractor = new ProjectTermsExtractor();
        }

        public void ExtractProjectFileTerms(ProjectFile projectFile, IMultiFileConverter multiFileConverter)
        {
            extractor.ExtractProjectFileTerms(projectFile, multiFileConverter);
        }

        public void ExtractProjectTerms(int occurrences, int length, List<string> blackList)
        {
            IEnumerable<string> terms = extractor.GetProjectTerms();

            Terms = terms
                .FilterByBlackList(blackList)
                .CountOccurences()
                .FilterByOccurrences(occurrences)
                .FilterByLength(length);

        }
    }
}
