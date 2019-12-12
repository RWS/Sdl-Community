using Sdl.FileTypeSupport.Framework.Core.Utilities.BilingualApi;
using Sdl.FileTypeSupport.Framework.IntegrationApi;
using Sdl.ProjectAutomation.Core;
using SDLCommunityCleanUpTasks.Utilities;

namespace SDLCommunityCleanUpTasks
{
	public class SegmentProcessor
    {
        private readonly ICleanUpSourceSettings _settings ;

        public SegmentProcessor(ICleanUpSourceSettings settings)
        {
            _settings = settings;
        }

        public void Run(IMultiFileConverter multiFileConverter, IProject project, ProjectFile projectFile, IXmlReportGenerator reportGenerator)
        {
            reportGenerator.AddFile(projectFile.LocalFilePath);
            multiFileConverter.AddBilingualProcessor(new BilingualContentHandlerAdapter(new SegmentContentHandler(_settings, project, reportGenerator)));
        }
    }
}