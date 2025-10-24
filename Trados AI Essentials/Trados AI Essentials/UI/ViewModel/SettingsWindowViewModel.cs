using System.Collections.Generic;
using Trados_AI_Essentials.Model;

namespace Trados_AI_Essentials.UI.ViewModel
{
    public class SettingsWindowViewModel : BaseModel
    {
        private List<TranslationEngineItem> _llmTranslationEngines = new List<TranslationEngineItem>();
        private TranslationEngineItem _selectedLLMEngine;

        public List<TranslationEngineItem> LLMTranslationEngines
        {
            get => _llmTranslationEngines;
            set => SetField(ref _llmTranslationEngines, value);
        }

        public TranslationEngineItem SelectedLLMEngine
        {
            get => _selectedLLMEngine;
            set => SetField(ref _selectedLLMEngine, value);
        }
    }
}