using System.Diagnostics.Contracts;
using System.Globalization;
using Sdl.Community.CleanUpTasks.Contracts;
using Sdl.Core.Settings;

namespace Sdl.Community.CleanUpTasks
{
	[ContractClass(typeof(ISettingsContract))]
    public interface ISettings
    {
        ISettingsGroup Settings { get; set; }
        CultureInfo SourceCulture { get; set; }
    }
}