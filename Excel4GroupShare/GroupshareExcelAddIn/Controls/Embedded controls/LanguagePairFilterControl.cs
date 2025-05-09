using GroupshareExcelAddIn.Interfaces;
using GroupshareExcelAddIn.Models;
using GroupshareExcelAddIn.Services;
using Sdl.Community.GroupShareKit.Models.Response;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GroupshareExcelAddIn.Controls.Embedded_controls
{
    public partial class LanguagePairFilterControl : UserControl, IFilterControl
    {
        private readonly LanguageService _languageService;

        public LanguagePairFilterControl(LanguageService languageService)
        {
            InitializeComponent();
            _languageService = languageService;
        }

        public FilterParameter FilterParameter => new FilterParameter { LanguagePair = GetFilteringLanguagePair() };

        private LanguagePair GetFilteringLanguagePair()
        {
            var source = _sourceLangComboBox?.SelectedValue as Language;
            var target = _targetLangComboBox?.SelectedValue as Language;

            source = source?.Name == "Any" ? null : source;
            target = target?.Name == "Any" ? null : target;
            var filterLanguagePair = source == null && target == null ? null : new LanguagePair(source, target);
            return filterLanguagePair;
        }

        private async Task<List<Language>> GetLanguages()
        {
            return await _languageService.GetLanguages();
        }

        private async void SetLanguageList()
        {
            var sourceLanguages = await GetLanguages();
            sourceLanguages.Insert(0, new Language { Name = "Any" });
            //we have to create a deep copy of the sourceLanguages object
            //otherwise, selecting any one of them on the UI automatically changes the other
            var targetLanguages = new List<Language>(sourceLanguages);
            _sourceLangComboBox.DataSource = sourceLanguages;
            _sourceLangComboBox.DisplayMember = "Name";
            _targetLangComboBox.DataSource = targetLanguages;
            _targetLangComboBox.DisplayMember = "Name";
        }

        private void LanguagePairFilterControl_Load(object sender, System.EventArgs e)
        {
            SetLanguageList();
        }
    }
}