using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Sdl.Community.XliffCompare.Core;

namespace Sdl.Community.XliffCompare
{
    public partial class FormSettings : Form
    {
        public bool SaveOptions { get; set; }

        public Settings.DifferencesFormatting StyleNewText = new Settings.DifferencesFormatting();
        public Settings.DifferencesFormatting StyleRemovedText = new Settings.DifferencesFormatting();
        public Settings.DifferencesFormatting StyleNewTag = new Settings.DifferencesFormatting();
        public Settings.DifferencesFormatting StyleRemovedTag = new Settings.DifferencesFormatting();

        private ErrorProvider _filePathCustomXslt;

        

        public FormSettings()
        {
            
            InitializeComponent();
            SaveOptions = false;
        }

        private void FormOptions_Load(object sender, EventArgs e)
        {
            Text = "Settings";

            treeView_main.ExpandAll();
            treeView_main.Select();
            treeView_main.SelectedNode = treeView_main.Nodes[0];

            checkBox_useCustomStyleSheet_CheckedChanged(sender, e);


            _filePathCustomXslt = new ErrorProvider();
            _filePathCustomXslt.SetIconAlignment(textBox_Custom_xsltFilePath, ErrorIconAlignment.TopRight);
            _filePathCustomXslt.SetIconPadding(textBox_Custom_xsltFilePath, 2);
            _filePathCustomXslt.BlinkRate = 1000;
            _filePathCustomXslt.BlinkStyle = ErrorBlinkStyle.AlwaysBlink;
       
            UpdateVisualStyle();
           
        }

        private void UpdateVisualStyle()
        {
            UpdateVisualTextFormatting(StyleNewText, richTextBox_formatting_Text_New, "This is an example of inserted text formatting.", 31, 4);
            UpdateVisualTextFormatting(StyleRemovedText, richTextBox_formatting_Text_Removed, "This is an example of deleted text formatting.", 30, 4);
            UpdateVisualTextFormatting(StyleNewTag, richTextBox_formatting_Tag_New, "This is an example of inserted <tag/> formatting.", 31, 6);
            UpdateVisualTextFormatting(StyleRemovedTag, richTextBox_formatting_Tag_Removed, "This is an example of deleted <tag/> formatting.", 30, 6);
        }

        private void UpdateVisualTextFormatting(Settings.DifferencesFormatting differencesFormatting, RichTextBox richTextBox, string text, int selectionStart, int selectionLength)
        {
            richTextBox.Text = text;

            var isBold = string.Compare(differencesFormatting.StyleBold, "Activate", StringComparison.OrdinalIgnoreCase) == 0;
            var isItalic = string.Compare(differencesFormatting.StyleItalic, "Activate", StringComparison.OrdinalIgnoreCase) == 0;
            var isStrikethrough = string.Compare(differencesFormatting.StyleStrikethrough, "Activate", StringComparison.OrdinalIgnoreCase) == 0;
            var isUnderline = string.Compare(differencesFormatting.StyleUnderline, "Activate", StringComparison.OrdinalIgnoreCase) == 0;
            var strPosition = differencesFormatting.TextPosition;

            var fontStyleNew = GetFontStyle(isBold, isItalic, isStrikethrough, isUnderline);

            richTextBox.Select(selectionStart, selectionLength);
            richTextBox.SelectionFont = new Font(richTextBox.Font.Name, richTextBox.Font.Size, fontStyleNew);

            if (differencesFormatting.FontSpecifyColor)
                richTextBox.SelectionColor = ColorTranslator.FromHtml(differencesFormatting.FontColor);
            else
                richTextBox.SelectionColor = richTextBox.ForeColor;

            if (differencesFormatting.FontSpecifyBackroundColor)
                richTextBox.SelectionBackColor = ColorTranslator.FromHtml(differencesFormatting.FontBackroundColor);
            else
                richTextBox.SelectionBackColor = richTextBox.BackColor;

            switch (strPosition)
            {
                case "Normal": richTextBox.SelectionCharOffset = 0; break;
                case "Superscript": richTextBox.SelectionCharOffset = 5; break;
                case "Subscript": richTextBox.SelectionCharOffset = -5; break;
            }


        }

        private FontStyle GetFontStyle(bool bold, bool italic, bool strikethrough, bool underline)
        {
            var fontStyle = FontStyle.Regular;
            if (bold)
            {
                fontStyle = FontStyle.Bold;
            }
            if (italic)
            {
                if (fontStyle != FontStyle.Regular)
                    fontStyle = fontStyle | FontStyle.Italic;
                else
                    fontStyle = FontStyle.Italic;
            }
            if (strikethrough)
            {
                if (fontStyle != FontStyle.Regular)
                    fontStyle = fontStyle | FontStyle.Strikeout;
                else
                    fontStyle = FontStyle.Strikeout;
            }
            if (!underline) 
                return fontStyle;

            if (fontStyle != FontStyle.Regular)
                fontStyle = fontStyle | FontStyle.Underline;
            else
                fontStyle = FontStyle.Underline;

            return fontStyle;
        }


        private void button_Save_Click(object sender, EventArgs e)
        {
            SaveOptions = true;
            Close();

        }

        private void button_Close_Click(object sender, EventArgs e)
        {
            SaveOptions = false;
            Close();
        }

        private void treeView_main_AfterSelect(object sender, TreeViewEventArgs e)
        {
            switch (e.Node.Text)
            {
                case "General Settings":
                    {
                        panel_General.BringToFront();
                    }break;
                case "Report Settings":
                    {
                        panel_Report.BringToFront();
                    }break;
                case "Custom style sheet":
                    {                    
                        panel_styleSheet.BringToFront();
                    } break;  
            }
        }


        private void button_changeFormattingTextNew_Click(object sender, EventArgs e)
        {
            var formSettingsFormatting = new FormSettingsFormatting
            {
                checkBox_fontColor = {Checked = StyleNewText.FontSpecifyColor},
                checkBox_backroundColor = {Checked = StyleNewText.FontSpecifyBackroundColor}
            };


            if (formSettingsFormatting.checkBox_fontColor.Checked)
                formSettingsFormatting.label_fontColorDisplay.BackColor = ColorTranslator.FromHtml(StyleNewText.FontColor);
            if (formSettingsFormatting.checkBox_backroundColor.Checked)
                formSettingsFormatting.label_backroundColorDisplay.BackColor = ColorTranslator.FromHtml(StyleNewText.FontBackroundColor);


            formSettingsFormatting.comboBox_styleBold.SelectedItem = StyleNewText.StyleBold;
            formSettingsFormatting.comboBox_styleItalic.SelectedItem = StyleNewText.StyleItalic;
            formSettingsFormatting.comboBox_styleStrikethrough.SelectedItem = StyleNewText.StyleStrikethrough;
            formSettingsFormatting.comboBox_styleUnderline.SelectedItem = StyleNewText.StyleUnderline;
            formSettingsFormatting.comboBox_stylePosition.SelectedItem = StyleNewText.TextPosition;

            
            formSettingsFormatting.ShowDialog();

            if (formSettingsFormatting.SaveSettings)
            {
                StyleNewText.FontSpecifyColor = formSettingsFormatting.checkBox_fontColor.Checked;
                StyleNewText.FontSpecifyBackroundColor = formSettingsFormatting.checkBox_backroundColor.Checked;

                if (StyleNewText.FontSpecifyColor)
                    StyleNewText.FontColor = ColorTranslator.ToHtml(formSettingsFormatting.label_fontColorDisplay.BackColor);
                if (StyleNewText.FontSpecifyBackroundColor)
                    StyleNewText.FontBackroundColor = ColorTranslator.ToHtml(formSettingsFormatting.label_backroundColorDisplay.BackColor);

                StyleNewText.StyleBold = formSettingsFormatting.comboBox_styleBold.SelectedItem.ToString();
                StyleNewText.StyleItalic = formSettingsFormatting.comboBox_styleItalic.SelectedItem.ToString();
                StyleNewText.StyleStrikethrough = formSettingsFormatting.comboBox_styleStrikethrough.SelectedItem.ToString();
                StyleNewText.StyleUnderline = formSettingsFormatting.comboBox_styleUnderline.SelectedItem.ToString();
                StyleNewText.TextPosition = formSettingsFormatting.comboBox_stylePosition.SelectedItem.ToString();
                UpdateVisualStyle();
            }
        }

        private void button_changeFormattingTextRemoved_Click(object sender, EventArgs e)
        {
            var formSettingsFormatting = new FormSettingsFormatting
            {
                checkBox_fontColor = {Checked = StyleRemovedText.FontSpecifyColor},
                checkBox_backroundColor = {Checked = StyleRemovedText.FontSpecifyBackroundColor}
            };


            if (formSettingsFormatting.checkBox_fontColor.Checked)
                formSettingsFormatting.label_fontColorDisplay.BackColor = ColorTranslator.FromHtml(StyleRemovedText.FontColor);
            if (formSettingsFormatting.checkBox_backroundColor.Checked)
                formSettingsFormatting.label_backroundColorDisplay.BackColor = ColorTranslator.FromHtml(StyleRemovedText.FontBackroundColor);


            formSettingsFormatting.comboBox_styleBold.SelectedItem = StyleRemovedText.StyleBold;
            formSettingsFormatting.comboBox_styleItalic.SelectedItem = StyleRemovedText.StyleItalic;
            formSettingsFormatting.comboBox_styleStrikethrough.SelectedItem = StyleRemovedText.StyleStrikethrough;
            formSettingsFormatting.comboBox_styleUnderline.SelectedItem = StyleRemovedText.StyleUnderline;
            formSettingsFormatting.comboBox_stylePosition.SelectedItem = StyleRemovedText.TextPosition;
            formSettingsFormatting.ShowDialog();

            if (!formSettingsFormatting.SaveSettings) 
                return;

            StyleRemovedText.FontSpecifyColor = formSettingsFormatting.checkBox_fontColor.Checked;
            StyleRemovedText.FontSpecifyBackroundColor = formSettingsFormatting.checkBox_backroundColor.Checked;

            if (StyleRemovedText.FontSpecifyColor)
                StyleRemovedText.FontColor = ColorTranslator.ToHtml(formSettingsFormatting.label_fontColorDisplay.BackColor);
            if (StyleRemovedText.FontSpecifyBackroundColor)
                StyleRemovedText.FontBackroundColor = ColorTranslator.ToHtml(formSettingsFormatting.label_backroundColorDisplay.BackColor);

            StyleRemovedText.StyleBold = formSettingsFormatting.comboBox_styleBold.SelectedItem.ToString();
            StyleRemovedText.StyleItalic = formSettingsFormatting.comboBox_styleItalic.SelectedItem.ToString();
            StyleRemovedText.StyleStrikethrough = formSettingsFormatting.comboBox_styleStrikethrough.SelectedItem.ToString();
            StyleRemovedText.StyleUnderline = formSettingsFormatting.comboBox_styleUnderline.SelectedItem.ToString();
            StyleRemovedText.TextPosition = formSettingsFormatting.comboBox_stylePosition.SelectedItem.ToString();

            UpdateVisualStyle();
        }

        private void button_changeFormattingTagNew_Click(object sender, EventArgs e)
        {
            var formSettingsFormatting = new FormSettingsFormatting
            {
                checkBox_fontColor = {Checked = StyleNewTag.FontSpecifyColor},
                checkBox_backroundColor = {Checked = StyleNewTag.FontSpecifyBackroundColor}
            };


            if (formSettingsFormatting.checkBox_fontColor.Checked)
                formSettingsFormatting.label_fontColorDisplay.BackColor = ColorTranslator.FromHtml(StyleNewTag.FontColor);
            if (formSettingsFormatting.checkBox_backroundColor.Checked)
                formSettingsFormatting.label_backroundColorDisplay.BackColor = ColorTranslator.FromHtml(StyleNewTag.FontBackroundColor);


            formSettingsFormatting.comboBox_styleBold.SelectedItem = StyleNewTag.StyleBold;
            formSettingsFormatting.comboBox_styleItalic.SelectedItem = StyleNewTag.StyleItalic;
            formSettingsFormatting.comboBox_styleStrikethrough.SelectedItem = StyleNewTag.StyleStrikethrough;
            formSettingsFormatting.comboBox_styleUnderline.SelectedItem = StyleNewTag.StyleUnderline;
            formSettingsFormatting.comboBox_stylePosition.SelectedItem = StyleNewTag.TextPosition;

            formSettingsFormatting.ShowDialog();

            if (!formSettingsFormatting.SaveSettings)
                return;

            StyleNewTag.FontSpecifyColor = formSettingsFormatting.checkBox_fontColor.Checked;
            StyleNewTag.FontSpecifyBackroundColor = formSettingsFormatting.checkBox_backroundColor.Checked;

            if (StyleNewTag.FontSpecifyColor)
                StyleNewTag.FontColor = ColorTranslator.ToHtml(formSettingsFormatting.label_fontColorDisplay.BackColor);
            if (StyleNewTag.FontSpecifyBackroundColor)
                StyleNewTag.FontBackroundColor = ColorTranslator.ToHtml(formSettingsFormatting.label_backroundColorDisplay.BackColor);

            StyleNewTag.StyleBold = formSettingsFormatting.comboBox_styleBold.SelectedItem.ToString();
            StyleNewTag.StyleItalic = formSettingsFormatting.comboBox_styleItalic.SelectedItem.ToString();
            StyleNewTag.StyleStrikethrough = formSettingsFormatting.comboBox_styleStrikethrough.SelectedItem.ToString();
            StyleNewTag.StyleUnderline = formSettingsFormatting.comboBox_styleUnderline.SelectedItem.ToString();
            StyleNewTag.TextPosition = formSettingsFormatting.comboBox_stylePosition.SelectedItem.ToString();
            UpdateVisualStyle();
        }

        private void button_changeFormattingTagRemoved_Click(object sender, EventArgs e)
        {
            var formSettingsFormatting = new FormSettingsFormatting
            {
                checkBox_fontColor = {Checked = StyleRemovedTag.FontSpecifyColor},
                checkBox_backroundColor = {Checked = StyleRemovedTag.FontSpecifyBackroundColor}
            };


            if (formSettingsFormatting.checkBox_fontColor.Checked)
                formSettingsFormatting.label_fontColorDisplay.BackColor = ColorTranslator.FromHtml(StyleRemovedTag.FontColor);
            if (formSettingsFormatting.checkBox_backroundColor.Checked)
                formSettingsFormatting.label_backroundColorDisplay.BackColor = ColorTranslator.FromHtml(StyleRemovedTag.FontBackroundColor);


            formSettingsFormatting.comboBox_styleBold.SelectedItem = StyleRemovedTag.StyleBold;
            formSettingsFormatting.comboBox_styleItalic.SelectedItem = StyleRemovedTag.StyleItalic;
            formSettingsFormatting.comboBox_styleStrikethrough.SelectedItem = StyleRemovedTag.StyleStrikethrough;
            formSettingsFormatting.comboBox_styleUnderline.SelectedItem = StyleRemovedTag.StyleUnderline;
            formSettingsFormatting.comboBox_stylePosition.SelectedItem = StyleRemovedTag.TextPosition;
            formSettingsFormatting.ShowDialog();

            if (!formSettingsFormatting.SaveSettings)
                return;

            StyleRemovedTag.FontSpecifyColor = formSettingsFormatting.checkBox_fontColor.Checked;
            StyleRemovedTag.FontSpecifyBackroundColor = formSettingsFormatting.checkBox_backroundColor.Checked;

            if (StyleRemovedTag.FontSpecifyColor)
                StyleRemovedTag.FontColor = ColorTranslator.ToHtml(formSettingsFormatting.label_fontColorDisplay.BackColor);
            if (StyleRemovedTag.FontSpecifyBackroundColor)
                StyleRemovedTag.FontBackroundColor = ColorTranslator.ToHtml(formSettingsFormatting.label_backroundColorDisplay.BackColor);

            StyleRemovedTag.StyleBold = formSettingsFormatting.comboBox_styleBold.SelectedItem.ToString();
            StyleRemovedTag.StyleItalic = formSettingsFormatting.comboBox_styleItalic.SelectedItem.ToString();
            StyleRemovedTag.StyleStrikethrough = formSettingsFormatting.comboBox_styleStrikethrough.SelectedItem.ToString();
            StyleRemovedTag.StyleUnderline = formSettingsFormatting.comboBox_styleUnderline.SelectedItem.ToString();
            StyleRemovedTag.TextPosition = formSettingsFormatting.comboBox_stylePosition.SelectedItem.ToString();
            UpdateVisualStyle();
        }

        public void SetDefaultToDefaults()
        {

            #region  |  general settings  |

            comboBox_comparisonType.SelectedItem = "words";
            checkBox_includeTagsInComparison.Checked = true;

            #endregion

            #region  |  report filter settings  |

            checkBox_reportFilterChangedTargetContent.Checked = true;
            checkBox_reportFilterSegmentStatusChanged.Checked = false;
            checkBox_reportFilterSegmentsContainingComments.Checked = true;
            #endregion

            #region  |  new text style  |

            StyleNewText = new Settings.DifferencesFormatting
            {
                StyleBold = "Deactivate",
                StyleItalic = "Deactivate",
                StyleStrikethrough = "Deactivate",
                StyleUnderline = "Activate",
                TextPosition = "Normal",
                FontSpecifyColor = true,
                FontColor = "#0000FF",
                FontSpecifyBackroundColor = true,
                FontBackroundColor = "#FFFF66"
            };

            #endregion

            #region  |  removed text style  |

            StyleRemovedText = new Settings.DifferencesFormatting
            {
                StyleBold = "Deactivate",
                StyleItalic = "Deactivate",
                StyleStrikethrough = "Activate",
                StyleUnderline = "Deactivate",
                TextPosition = "Normal",
                FontSpecifyColor = true,
                FontColor = "#FF0000",
                FontSpecifyBackroundColor = false,
                FontBackroundColor = "#FFFFFF"
            };

            #endregion

            #region  |  new tag style  |

            StyleNewTag = new Settings.DifferencesFormatting
            {
                StyleBold = "Deactivate",
                StyleItalic = "Deactivate",
                StyleStrikethrough = "Deactivate",
                StyleUnderline = "Deactivate",
                TextPosition = "Normal",
                FontSpecifyColor = false,
                FontColor = "#000000",
                FontSpecifyBackroundColor = true,
                FontBackroundColor = "#DDEEFF"
            };

            #endregion

            #region  |  removed tag style  |

            StyleRemovedTag = new Settings.DifferencesFormatting
            {
                StyleBold = "Deactivate",
                StyleItalic = "Deactivate",
                StyleStrikethrough = "Deactivate",
                StyleUnderline = "Deactivate",
                TextPosition = "Normal",
                FontSpecifyColor = false,
                FontColor = "#000000",
                FontSpecifyBackroundColor = true,
                FontBackroundColor = "#FFE8E8"
            };

            #endregion

            #region  |  custom style sheet  |

            checkBox_useCustomStyleSheet.Checked = false;
            
            #endregion

            UpdateVisualStyle();
        }

        private void button_resetToDefaults_Click(object sender, EventArgs e)
        {
            SetDefaultToDefaults();
        }

        private void button_browseCustomXsltFilePath_Click(object sender, EventArgs e)
        {
            var sPath = textBox_Custom_xsltFilePath.Text;

            if (!Directory.Exists(sPath))
            {
                while (sPath.Contains("\\"))
                {
                    sPath = sPath.Substring(0, sPath.LastIndexOf("\\", StringComparison.Ordinal));
                    if (Directory.Exists(sPath))
                    {
                        break;
                    }
                }
            }



            var openFileDialog = new OpenFileDialog
            {
                InitialDirectory = sPath,
                Filter = "Extensible style sheet (*.xslt)|*.xslt;",
                Title = "Select the xslt file",
                FilterIndex = 0,
                Multiselect = false,
                RestoreDirectory = true
            };


            if (openFileDialog.ShowDialog() != DialogResult.OK) 
                return;

            if (openFileDialog.FileName != null && openFileDialog.FileName.Trim() != "")
            {
                textBox_Custom_xsltFilePath.Text = openFileDialog.FileName;
            }
        }

        private void checkBox_useCustomStyleSheet_CheckedChanged(object sender, EventArgs e)
        {
            textBox_Custom_xsltFilePath.Enabled = checkBox_useCustomStyleSheet.Checked;

            if (checkBox_useCustomStyleSheet.Checked)
                textBox_Custom_xsltFilePath_TextChanged(sender, e);
            else if (_filePathCustomXslt != null)
                _filePathCustomXslt.SetError(textBox_Custom_xsltFilePath, string.Empty);
        }

        private void textBox_Custom_xsltFilePath_TextChanged(object sender, EventArgs e)
        {
            if (!checkBox_useCustomStyleSheet.Checked || _filePathCustomXslt == null) return;
            if (textBox_Custom_xsltFilePath.Text.Trim() != string.Empty && !File.Exists(textBox_Custom_xsltFilePath.Text))
                _filePathCustomXslt.SetError(textBox_Custom_xsltFilePath, "File not found!");
            else
                _filePathCustomXslt.SetError(textBox_Custom_xsltFilePath, string.Empty);
        }






        

    
    }
}
