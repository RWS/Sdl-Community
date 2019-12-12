using System.Globalization;
using Sdl.Core.Settings;

namespace SDLCommunityCleanUpTasks.Contracts
{
    internal abstract class ISettingsContract : ISettings
    {
        public ISettingsGroup Settings { get; set; }


		public CultureInfo SourceCulture { get; set; }

	}
}