using System;
using System.Windows.Forms;

namespace Multilingual.Excel.FileType.Verifier.SettingsUI
{
	public partial class VerifcationSettings : UserControl
	{
		public VerifcationSettings()
		{
			InitializeComponent();

			CheckMaxCharacterLengthEnabled();
			CheckMaxPixelLengthEnabled();
			CheckMaxLinesPerParagraphEnabled();

			var pixelValueToolTip = new ToolTip();
			pixelValueToolTip.SetToolTip(pictureBox_maxPixelLengthInfo, PluginResources.ToolTip_ValueInhertiedFromExcelWorksheet);
			var characterValueToolTip = new ToolTip();
			characterValueToolTip.SetToolTip(pictureBox_maxCharacterLengthInfo, PluginResources.ToolTip_ValueInhertiedFromExcelWorksheet);
		}

		public bool MaxCharacterLengthEnabled
		{
			get => checkBox_MaxCharacterLengthEnabled.Checked;
			set => checkBox_MaxCharacterLengthEnabled.Checked = value;
		}

		public int MaxCharacterLengthSeverity
		{
			get => comboBox_MaxCharacterLengthSeverity.SelectedIndex;
			set => comboBox_MaxCharacterLengthSeverity.SelectedIndex = value;
		}

		public bool MaxPixelLengthEnabled
		{
			get => checkBox_MaxPixelLengthEnabled.Checked;
			set => checkBox_MaxPixelLengthEnabled.Checked = value;
		}

		public int MaxPixelLengthSeverity
		{
			get => comboBox_MaxPixelLengthSeverity.SelectedIndex;
			set => comboBox_MaxPixelLengthSeverity.SelectedIndex = value;
		}

		public bool MaxLinesPerParagraphEnabled
		{
			get => checkBox_MaxLinesPerParagraphEnabled.Checked;
			set => checkBox_MaxLinesPerParagraphEnabled.Checked = value;
		}

		public int MaxLinesPerParagraph
		{
			get => Convert.ToInt32(numericUpDown_MaxLinesPerParagraph.Value);
			set => numericUpDown_MaxLinesPerParagraph.Value = value;
		}

		public int MaxLinesPerParagraphSeverity
		{
			get => comboBox_MaxLinesPerParagraphSeverity.SelectedIndex;
			set => comboBox_MaxLinesPerParagraphSeverity.SelectedIndex = value;
		}


		private void CheckMaxCharacterLengthEnabled()
		{
			if (checkBox_MaxCharacterLengthEnabled.Checked)
			{
				comboBox_MaxCharacterLengthSeverity.Enabled = true;
			}
			else
			{
				comboBox_MaxCharacterLengthSeverity.Enabled = false;
			}
		}

		private void CheckMaxPixelLengthEnabled()
		{
			if (checkBox_MaxPixelLengthEnabled.Checked)
			{
				comboBox_MaxPixelLengthSeverity.Enabled = true;
			}
			else
			{
				comboBox_MaxPixelLengthSeverity.Enabled = false;
			}
		}

		private void CheckMaxLinesPerParagraphEnabled()
		{
			if (checkBox_MaxLinesPerParagraphEnabled.Checked)
			{
				numericUpDown_MaxLinesPerParagraph.Enabled = true;
				comboBox_MaxLinesPerParagraphSeverity.Enabled = true;
			}
			else
			{
				numericUpDown_MaxLinesPerParagraph.Enabled = false;
				comboBox_MaxLinesPerParagraphSeverity.Enabled = false;
			}
		}

		public bool VerifySourceParagraphsEnabled
		{
			get => checkBox_VerifySourceParagraphsEnabled.Checked;
			set => checkBox_VerifySourceParagraphsEnabled.Checked = value;
		}

		public bool VerifyTargetParagraphsEnabled
		{
			get => checkBox_VerifyTargetParagraphsEnabled.Checked;
			set => checkBox_VerifyTargetParagraphsEnabled.Checked = value;
		}

		private void checkBox_MaxCharacterLengthEnabled_CheckedChanged(object sender, EventArgs e)
		{
			CheckMaxCharacterLengthEnabled();
		}

		private void checkBox_MaxPixelLengthEnabled_CheckedChanged(object sender, EventArgs e)
		{
			CheckMaxPixelLengthEnabled();
		}

		private void checkBox_MaxLinesPerParagraphEnabled_CheckedChanged(object sender, EventArgs e)
		{
			CheckMaxLinesPerParagraphEnabled();
		}
	}
}
