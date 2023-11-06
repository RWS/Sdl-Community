using Sdl.Core.Globalization;
using Sdl.Core.Globalization.LanguageRegistry;
using Sdl.Desktop.IntegrationApi.Interfaces;
using Sdl.TranslationStudioAutomation.IntegrationApi;
using System.Collections.Generic;

namespace InterpretBank.Studio
{
    public static class StudioContext
    {
        private static EditorController _editorController;
        private static ProjectsController _projectsController;

        public static EditorController EditorController =>
            _editorController ??= SdlTradosStudio.Application?.GetController<EditorController>();

        public static IStudioEventAggregator EventAggregator =>
            SdlTradosStudio.Application.GetService<IStudioEventAggregator>();

        public static IList<Language> Languages => LanguageRegistryApi.Instance.GetAllLanguages();

        public static ProjectsController ProjectsController =>
                                    _projectsController ??= SdlTradosStudio.Application?.GetController<ProjectsController>();
    }
}