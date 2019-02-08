using Sdl.Community.CleanUpTasks.Utilities;
using Sdl.FileTypeSupport.Framework.Core.Utilities.BilingualApi;
using Sdl.FileTypeSupport.Framework.IntegrationApi;
using Sdl.ProjectAutomation.Core;

namespace Sdl.Community.CleanUpTasks
{
	public class SegmentProcessor
    {
        private readonly ICleanUpSourceSettings settings = null;

        public SegmentProcessor(ICleanUpSourceSettings settings)
        {
            this.settings = settings;
        }

        public void Run(IMultiFileConverter multiFileConverter, IProject project, ProjectFile projectFile, IXmlReportGenerator reportGenerator)
        {
            reportGenerator.AddFile(projectFile.LocalFilePath);
            multiFileConverter.AddBilingualProcessor(new BilingualContentHandlerAdapter(new SegmentContentHandler(settings, project, reportGenerator)));
        }
    }
}