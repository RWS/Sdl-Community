using System;
using System.ComponentModel;
using System.Configuration;
using System.Drawing;
using System.Windows.Forms;
using Sdl.Community.EmbeddedContentProcessor.Settings;

namespace Sdl.Community.EmbeddedContentProcessor.UI
{
    public partial class RegexRuleForm : Form
    {
        readonly MatchRule _rule;
        private const string TagPair = "Tag Pair";
        private const string PlaceholderType = "Placeholder";
        private const string Translatable = "Translatable";
        private const string NotTranslatable = "Not translatable";
       
        
        public RegexRuleForm(MatchRule rule)
        {
            InitializeComponent();

            _rule = rule;

            _ruleTypeComboBox.Text = rule.TagType == MatchRule.TagTypeOption.TagPair ? TagPair : PlaceholderType;

            _startTagTextBox.Text = _rule.StartTagRegexValue;
            _endTagTextBox.Text = _rule.EndTagRegexValue;
            
            if (rule.IsContentTranslatable)
            {
                _translateComboBox.Text = Translatable;
            }
            else
            {
                _translateComboBox.Text = NotTranslatable;
            }

            _ignoreCaseCheckbox.Checked = _rule.IgnoreCase;

        }

     
        private void _ruleTypeComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_ruleTypeComboBox.Text == PlaceholderType)
            {
                l_Closing.Visible = false;
                _endTagTextBox.Visible = false;
                _translateComboBox.Enabled = false;
                _translateComboBox.Text = NotTranslatable;
                _endTagTextBox.Text = "";
            }
            else
            {
                l_Closing.Visible = true;
                _endTagTextBox.Visible = true;
                _translateComboBox.Enabled = true;
                if (_translateComboBox.Items.Count > 0)
                {
                    _translateComboBox.SelectedIndex = 0;
                }
            }
        }

        protected override void OnLoad(EventArgs e)
        {
           if (_ruleTypeComboBox.Items.Count == 0)
            {
                _ruleTypeComboBox.Items.Add(TagPair);
                _ruleTypeComboBox.Items.Add(PlaceholderType);
            }
            
            _ruleTypeComboBox.SelectedItem = _rule.TagType == MatchRule.TagTypeOption.TagPair ? TagPair : PlaceholderType;

            if (_translateComboBox.Items.Count == 0)
            {
                _translateComboBox.Items.Add(Translatable);
                _translateComboBox.Items.Add(NotTranslatable);
            }

            if (_rule.IsContentTranslatable)
            {
                _translateComboBox.SelectedItem = Translatable;
            }
            else
            {
                _translateComboBox.SelectedItem = NotTranslatable;
            }
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            if (DialogResult != DialogResult.OK) return;
            //check the dialog has valid settings
            if ((_ruleTypeComboBox.SelectedItem.ToString() == TagPair &&
                 (_endTagTextBox.Text.Length == 0 || _startTagTextBox.Text.Length == 0)) ||
                (_ruleTypeComboBox.SelectedItem.ToString() == PlaceholderType &&
                 _startTagTextBox.Text.Length == 0))
            {
                MessageBox.Show("The regular expression pattern must be specified.",
                    "Embedded Processing", MessageBoxButtons.OK, MessageBoxIcon.Error);
                DialogResult = DialogResult.None;
                e.Cancel = true;
            }

            _rule.StartTagRegexValue = _startTagTextBox.Text;
            _rule.EndTagRegexValue = _endTagTextBox.Text;

            _rule.TagType = _ruleTypeComboBox.SelectedItem.ToString() == TagPair ? MatchRule.TagTypeOption.TagPair : MatchRule.TagTypeOption.Placeholder;

            _rule.IsContentTranslatable = _translateComboBox.SelectedItem.ToString() == Translatable;
        }

    private void tb_Opening_TextChanged(object sender, EventArgs e)
        {
            ValidateRegexTextBox(_startTagTextBox);
        }

        private void tb_Closing_TextChanged(object sender, EventArgs e)
        {
            ValidateRegexTextBox(_endTagTextBox);
        }

        private void ValidateRegexTextBox(TextBox validationBox)
        {
            try
            {
                if(validationBox.Text.Length > 0)
                {
                    var validator = new RegexStringValidator(validationBox.Text);
                }

                _errorProvider.SetError(validationBox, "");
                validationBox.ForeColor = Color.Black;
            }
            catch (ArgumentException e)
            {
                _errorProvider.SetIconAlignment(validationBox, ErrorIconAlignment.MiddleRight);
                _errorProvider.SetIconPadding(validationBox, -20);
                _errorProvider.SetError(validationBox, "The regular expression is incorrect:" + "\n" + e.Message);
                validationBox.ForeColor = Color.Red;
            }
        }
    }
}

