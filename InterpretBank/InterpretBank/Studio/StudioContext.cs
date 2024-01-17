using Sdl.Core.Globalization;
using Sdl.Core.Globalization.LanguageRegistry;
using Sdl.Desktop.IntegrationApi.Interfaces;
using Sdl.TranslationStudioAutomation.IntegrationApi;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

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

        public static IList<Language> Languages { get; set; } =
            LanguageRegistryApi.Instance.GetAllLanguages().ToList();

        public static ProjectsController ProjectsController =>
                                    _projectsController ??= SdlTradosStudio.Application?.GetController<ProjectsController>();

        public static Image GetLanguageFlag(string languageName)
        {
            var neutralLangCode = Languages.FirstOrDefault(sl => sl.EnglishName == languageName && sl.IsNeutral)?.DefaultSpecificLanguageCode;

            if (neutralLangCode == null) return null;
            var lang = LanguageRegistryApi.Instance.GetLanguage(neutralLangCode);
            return lang.GetFlagImage();
        }
    }
}