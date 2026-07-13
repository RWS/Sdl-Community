using Sdl.TranslationStudioAutomation.IntegrationApi;

namespace Sdl.Community.AntidoteVerifier
{
    /// <summary>Cached access to the Studio controllers the plugin uses.</summary>
    public static class ApplicationContext
    {
        private static EditorController _editorController;

        public static EditorController EditorController =>
            _editorController ??= SdlTradosStudio.Application.GetController<EditorController>();
    }
}
