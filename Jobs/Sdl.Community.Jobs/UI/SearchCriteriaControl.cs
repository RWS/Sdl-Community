using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Newtonsoft.Json;
using Sdl.Community.Jobs.Model;
using Sdl.Community.Jobs.Services;

namespace Sdl.Community.Jobs.UI
{
    public partial class SearchCriteriaControl : UserControl
    {
        private readonly JobService _service;

        public SearchCriteriaControl()
        {
            InitializeComponent();
        }

        public SearchCriteriaControl(JobService service):this()
        {
            _service = service;
        }

        protected override void OnLoad(EventArgs e)
        {
           
            var disciplineList = JsonConvert.DeserializeObject<List<Discipline>>(PluginResources.discipline);
            disciplineList.Insert(0, new Discipline {DiscSpecId = 0, DiscSpecName = "All fields"});
            cbFields.DataSource = disciplineList;

            var sourceLanguages = JsonConvert.DeserializeObject<List<Language>>(PluginResources.languages);
            sourceLanguages.Insert(0, new Language() { LanguageCode = "all", LanguageName = "All languages" });
            var targetLanguages = new List<Language>(sourceLanguages);

            cbSourceLanguages.DataSource = sourceLanguages;
            cbTargetLanguages.DataSource = targetLanguages;

        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            var searchCriteria = new SearchCriteria
            {
                Discipline = cbFields.SelectedItem as Discipline,
                LanguagePair = new LanguagePair(cbSourceLanguages.SelectedItem as Language, cbTargetLanguages.SelectedItem as Language),
                SearchTerm = txtSearch.Text,
                Translation = chkTranslation.Checked,
                Interpreting = chkInterpreting.Checked,
                Potential = chkPotential.Checked
            };

            _service.Search(searchCriteria);

            EnableDisablePagingButtons();
        }

        private void lnkNext_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            var searchCriteria = new SearchCriteria
            {
                Discipline = cbFields.SelectedItem as Discipline,
                LanguagePair = new LanguagePair(cbSourceLanguages.SelectedItem as Language, cbTargetLanguages.SelectedItem as Language),
                SearchTerm = txtSearch.Text,
                Translation = chkTranslation.Checked,
                Interpreting = chkInterpreting.Checked,
                Potential = chkPotential.Checked
            };

            _service.CurrentPage++;
            _service.Search(searchCriteria,_service.CurrentPage);

            EnableDisablePagingButtons();
        }

        private void lnkPrevious_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            var searchCriteria = new SearchCriteria
            {
                Discipline = cbFields.SelectedItem as Discipline,
                LanguagePair = new LanguagePair(cbSourceLanguages.SelectedItem as Language, cbTargetLanguages.SelectedItem as Language),
                SearchTerm = txtSearch.Text,
                Translation = chkTranslation.Checked,
                Interpreting = chkInterpreting.Checked,
                Potential = chkPotential.Checked
            };

            _service.CurrentPage--;
            _service.Search(searchCriteria, _service.CurrentPage);

            EnableDisablePagingButtons();
        }


        private void EnableDisablePagingButtons()
        {
            lnkNext.Enabled = _service.HasNext;
            lnkPrevious.Enabled = _service.HasPrevious;
            lblCurrentPage.Text = _service.CurrentPage.ToString();
        }
    }
}
