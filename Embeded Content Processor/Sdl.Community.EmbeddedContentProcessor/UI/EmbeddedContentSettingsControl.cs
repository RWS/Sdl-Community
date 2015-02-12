using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Sdl.Community.EmbeddedContentProcessor.Settings;
using Sdl.FileTypeSupport.Framework.Core.Settings;
using Sdl.FileTypeSupport.Framework.Core.Utilities.NativeApi;

namespace Sdl.Community.EmbeddedContentProcessor.UI
{
    public partial class EmbeddedContentSettingsControl : UserControl, IFileTypeSettingsAware<EmbeddedContentRegexSettings>
    {
        EmbeddedContentRegexSettings _settings;

        public EmbeddedContentSettingsControl()
        {
            InitializeComponent();
        }

        public void UpdateSettings()
        {
            Settings.Enabled = _enableProcessingCheckbox.Checked;

            Settings.StructureInfos.Clear();
            Settings.StructureInfos.Add(StandardContextTypes.CData);

            Settings.MatchRules.Clear();
            Settings.MatchRules.AddRange(GetMatchRules());
        }

        public void UpdateUi()
        {
            _enableProcessingCheckbox.Checked = Settings.Enabled;

           _rulesListView.Items.Clear();

            foreach (MatchRule rule in Settings.MatchRules)
            {
                _rulesListView.Items.Add(new RegexRuleListItem(rule));
            }

            UpdateEnabledState();
        }

        private IEnumerable<MatchRule> GetMatchRules()
        {
            return _rulesListView.Items.OfType<RegexRuleListItem>().Select(regexItem => regexItem.GetRule()).ToList();
        }

        private void _addRuleButton_Click(object sender, EventArgs e)
        {
            var newRule = new MatchRule();

            using (var regexForm = new RegexRuleForm(newRule))
            {
                if (regexForm.ShowDialog(this) == DialogResult.OK)
                {
                    _rulesListView.Items.Add(new RegexRuleListItem(newRule));

                    UpdateEnabledState();
                }
            }
        }

        private void _editRuleButton_Click(object sender, EventArgs e)
        {
            if (_rulesListView.SelectedItems.Count > 0)
            {
                var regexItem = _rulesListView.SelectedItems[0] as RegexRuleListItem;

                if (regexItem == null) return;
                var rule = regexItem.GetRule();

                using (var regexForm = new RegexRuleForm(rule))
                {
                    if (regexForm.ShowDialog(this) == DialogResult.OK)
                    {
                        regexItem.SetRule(rule);
                    }
                }
            }
        }

        private void _removeRuleButton_Click(object sender, EventArgs e)
        {
            if (_rulesListView.SelectedItems.Count <= 0) return;
            var item = _rulesListView.SelectedItems[0];

            if (item == null) return;
            _rulesListView.Items.Remove(item);

            UpdateEnabledState();
        }

        private void _removeAllButton_Click(object sender, EventArgs e)
        {
            _rulesListView.Items.Clear();

            UpdateEnabledState();
        }

        private void _enableProcessingCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            if (_enableProcessingCheckbox.Checked == false)
            {
                _rulesListView.SelectedItems.Clear();
            }
            UpdateEnabledState();
        }

        private void _rulesListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateEnabledState();
        }

        public void UpdateEnabledState()
        {
            var enabled = _enableProcessingCheckbox.Checked;

            _rulesListView.Enabled = enabled;
            _addRuleButton.Enabled = enabled;
            _editRuleButton.Enabled = _rulesListView.SelectedItems.Count > 0 && enabled;
            _removeRuleButton.Enabled = _rulesListView.SelectedItems.Count > 0 && enabled;
            _removeAllButton.Enabled = _rulesListView.SelectedItems.Count > 0 && enabled;
        }

        public EmbeddedContentRegexSettings Settings
        {
            get
            {
                return _settings;
            }
            set
            {
                _settings = value;
                UpdateUi();
            }
        }

    }
}
