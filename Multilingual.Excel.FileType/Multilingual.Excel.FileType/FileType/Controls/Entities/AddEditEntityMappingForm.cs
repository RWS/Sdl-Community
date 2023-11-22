using System;
using System.Globalization;
using System.Windows.Forms;

namespace Multilingual.XML.FileType.FileType.Controls.Entities
{
    public partial class AddEditEntityMappingForm : Form
    {
        private bool _settingCharacterProgrammatically;

        public AddEditEntityMappingForm()
        {
            InitializeComponent();
        }

        public AddEditEntityMappingForm(string entityName, int unicodeValue)
            : this()
        {
            textBoxEntityName.Text = entityName;
            maskedTextBoxUnicodeValue.Text = unicodeValue.ToString();
            if (entityName.StartsWith("#"))
            {
                radioButtonNumericEntity.Checked = true;
            }
            else
            {
                radioButtonCharacterEntity.Checked = true;
            }
            SetEntityName(unicodeValue);
            SetCharacter(unicodeValue);
        }


        public string EntityName => textBoxEntityName.Text;

        public int UnicodeValue
        {
            get
            {
                int.TryParse(maskedTextBoxUnicodeValue.Text, out var returnValue);
                return returnValue;
            }
        }

        private void radioButtonCharacterEntity_CheckedChanged(object sender, EventArgs e)
        {
            if (textBoxEntityName.Text.StartsWith("#"))
                textBoxEntityName.Text = string.Empty;

            SetEnabledItems();
        }


        private void maskedTextBoxUnicodeValue_TextChanged(object sender, EventArgs e)
        {
            if (int.TryParse(maskedTextBoxUnicodeValue.Text, out var value))
            {
                SetEntityName(value);
                SetCharacter(value);
            }

            SetEnabledItems();
        }

        private void SetCharacter(int value)
        {
            _settingCharacterProgrammatically = true;

            textBoxCharacter.Text = $"{(char)value}";

            _settingCharacterProgrammatically = false;
        }


        private void SetEntityName(int unicodeValue)
        {
            if (radioButtonNumericEntity.Checked)
            {
                var numericEntityName = "#" + unicodeValue.ToString(CultureInfo.InvariantCulture);
                textBoxEntityName.Text = numericEntityName;

                labelEntitySample.Text = "&" + numericEntityName + ";";
            }
            else
            {
                labelEntitySample.Text = "&" + textBoxEntityName.Text + ";";
            }
        }


        private void SetEnabledItems()
        {
            textBoxEntityName.Enabled = radioButtonCharacterEntity.Checked;

            var isValidUnicodeValue = IsValidUnicodeValue();
            var isValidEntityName = IsValidEntityName(textBoxEntityName.Text);

            buttonOK.Enabled = isValidEntityName && isValidUnicodeValue;
        }


        private bool IsValidEntityName(string name)
        {
            // TODO: parse for invalid entity name characters...
            return !string.IsNullOrEmpty(name);
        }


        private bool IsValidUnicodeValue()
        {
            return int.TryParse(maskedTextBoxUnicodeValue.Text, out _);
        }


        private void textBoxCharacter_TextChanged(object sender, EventArgs e)
        {
            if (_settingCharacterProgrammatically) return;


            if (IsValidCharacter(textBoxCharacter.Text))
            {
                int unicodeValue = textBoxCharacter.Text[0];
                SetEntityName(unicodeValue);
                SetUnicodeValue(unicodeValue);
            }

            SetEnabledItems();
        }

        private void SetUnicodeValue(int unicodeValue)
        {
            _settingCharacterProgrammatically = true;

            maskedTextBoxUnicodeValue.Text = unicodeValue.ToString();

            _settingCharacterProgrammatically = false;
        }


        private bool IsValidCharacter(string character)
        {
            return !string.IsNullOrEmpty(character) && character.Length == 1;
        }


        private void textBoxEntityName_TextChanged(object sender, EventArgs e)
        {
            labelEntitySample.Text = "&" + textBoxEntityName.Text + ";";

            SetEnabledItems();
        }


        private void radioButtonNumericEntity_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButtonNumericEntity.Checked)
            {
                if (IsValidUnicodeValue())
                {
                    SetEntityName(UnicodeValue);
                }
                else
                {
                    textBoxEntityName.Text = String.Empty;
                }
            }

            SetEnabledItems();
        }

    }
}
