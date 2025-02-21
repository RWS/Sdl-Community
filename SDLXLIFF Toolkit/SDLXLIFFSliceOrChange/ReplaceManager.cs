using NLog;
using Sdl.FileTypeSupport.Framework.Core.Utilities.BilingualApi;
using Sdl.FileTypeSupport.Framework.Core.Utilities.IntegrationApi;
using Sdl.FileTypeSupport.Framework.IntegrationApi;
using SdlXliff.Toolkit.Integration.File;

namespace SDLXLIFFSliceOrChange
{
    public static class ReplaceManager
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        private static IFileTypeManager FileTypeManager { get; } = DefaultFileTypeManager.CreateInstance(true);

        public static void DoReplaceInFile(string file, ReplaceSettings settings,
                    SdlxliffSliceOrChange sdlxliffSliceOrChange)
        {
            var converter = FileTypeManager.GetConverterToDefaultBilingual(file, file, null);

            var contentProcessor = new FileReplaceProcessor(settings);
            converter?.AddBilingualProcessor(new BilingualContentHandlerAdapter(contentProcessor));
            converter?.Parse();
        }
    }
}