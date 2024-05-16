using Sdl.ProjectAutomation.FileBased;
using Sdl.TranslationStudioAutomation.IntegrationApi;
using System.Linq;

namespace Sdl.Community.AntidoteVerifier
{
    public static class ApplicationContext
    {
        private static EditorController _editorController;
        private static FilesController _filesController;
        private static ProjectsController _projectsController;

        public static EditorController EditorController =>
            _editorController ??= SdlTradosStudio.Application.GetController<EditorController>();

        public static FilesController FilesController =>
            _filesController ??= SdlTradosStudio.Application.GetController<FilesController>();

        public static ProjectsController ProjectsController =>
                            _projectsController ??= SdlTradosStudio.Application.GetController<ProjectsController>();

       
    }
}