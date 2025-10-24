using Trados_AI_Essentials.Interface;
using Trados_AI_Essentials.LC;
using Trados_AI_Essentials.Service;
using Trados_AI_Essentials.UI.ViewModel;
using Trados_AI_Essentials.View;

namespace Trados_AI_Essentials
{
    public static class Dependencies
    {
        public static ILCClient LCClient { get; set; } = new LCClient(new HttpClient());
        public static SettingsService SettingsService => new(LCClient, SettingsWindow);
        public static SettingsWindow SettingsWindow => new() { DataContext = new SettingsWindowViewModel() };

        public static TranslationService TranslationService => new();
    }
}