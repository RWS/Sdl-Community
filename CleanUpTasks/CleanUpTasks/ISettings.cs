using System.Diagnostics.Contracts;
using System.Globalization;
using Sdl.Core.Settings;
using SDLCommunityCleanUpTasks.Contracts;

namespace SDLCommunityCleanUpTasks
{
	[ContractClass(typeof(ISettingsContract))]
    public interface ISettings
    {
        ISettingsGroup Settings { get; set; }
        CultureInfo SourceCulture { get; set; }
    }
}