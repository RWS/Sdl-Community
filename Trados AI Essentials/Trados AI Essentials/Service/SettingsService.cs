using Trados_AI_Essentials.Interface;
using Trados_AI_Essentials.Model;
using Trados_AI_Essentials.UI.ViewModel;
using Trados_AI_Essentials.View;

namespace Trados_AI_Essentials.Service
{
    public class SettingsService(ILCClient lcClient, SettingsWindow settingsWindow)
    {
        private ILCClient LCClient { get; } = lcClient;

        private SettingsWindow SettingsWindow { get; } = settingsWindow;

        private SettingsWindowViewModel SettingsWindowViewModel => (SettingsWindowViewModel)SettingsWindow.DataContext;

        public Settings GetSettingsFromUser()
        {
            LCClient.GetLLMTranslationEngines()
                .ContinueWith(tr =>
                    SettingsWindow.Dispatcher.Invoke(() => SettingsWindowViewModel.LLMTranslationEngines = tr.Result));
            SettingsWindow.ShowDialog();

            return new Settings { SelectedLLMTE = SettingsWindowViewModel.SelectedLLMEngine?.Id };
        }
    }
}