using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Sdl.Utilities.SplitSDLXLIFF.Wizard;
using Sdl.Utilities.SplitSDLXLIFF.Lib;

namespace Sdl.Utilities.SplitSDLXLIFF
{
    public partial class OptionsPage : InternalWizardPage
    {
        private const int _wordsDef = 1000;
        private const int _partsDef = 2;
        private const int _percDef = 90;

        public OptionsPage()
        {
            InitializeComponent();
        }

        private void bindSegments()
        {
            clbSegments.Items.Clear();

            clbSegments.Items.Add(TagSegStatus.getTagSegStatus(SegStatus.Draft));
            clbSegments.Items.Add(TagSegStatus.getTagSegStatus(SegStatus.Translated));
            clbSegments.Items.Add(TagSegStatus.getTagSegStatus(SegStatus.RejectedTranslation));
            clbSegments.Items.Add(TagSegStatus.getTagSegStatus(SegStatus.RejectedSignOff));
            clbSegments.Items.Add(TagSegStatus.getTagSegStatus(SegStatus.ApprovedTranslation));
            clbSegments.Items.Add(TagSegStatus.getTagSegStatus(SegStatus.ApprovedSignOff));
        }
        private void bindOptions()
        {
            chSplitCheckPercent.Checked = AppOptions.splitOpts.IsPercent;
            rbWordCount.Checked = true;
            if (AppOptions.splitOpts.Criterion == SplitOptions.SplitType.EqualParts)
                rbEqualParts.Checked = true;
            if (AppOptions.splitOpts.Criterion == SplitOptions.SplitType.SegmentNumbers)
                rbSegNumbers.Checked = true;
            nudWords.Value = (AppOptions.splitOpts.WordsCount == 0 ? _wordsDef : AppOptions.splitOpts.WordsCount);
            nudParts.Value = (AppOptions.splitOpts.PartsCount == 0 ? _partsDef : AppOptions.splitOpts.PartsCount);
            nudPercent.Value = (AppOptions.splitOpts.PercMax == 0 ? _percDef : AppOptions.splitOpts.PercMax);

            tbSegments.Text = AppOptions.segmentIDs;

            // restore non-countable segments
            for (int i = 0; i < clbSegments.Items.Count; i++)
                if (AppOptions.splitOpts.SplitNonCountStatus.Contains(TagSegStatus.getTagSegStatus((string)clbSegments.Items[i])))
                    clbSegments.SetItemChecked(i, true);
                else clbSegments.SetItemChecked(i, false);

            // rebind views
            bindCriterionView(AppOptions.splitOpts.Criterion);
            bindPercentView();
        }
        private void bindCriterionView(SplitOptions.SplitType criterion)
        {
            switch (criterion)
            {
                case SplitOptions.SplitType.WordsCount:
                    nudWords.Enabled = true;
                    nudParts.Enabled = false;
                    tbSegments.Enabled = false;
                    pWordCountOptions.Visible = true;
                    break;
                case SplitOptions.SplitType.EqualParts:
                    nudWords.Enabled = false;
                    nudParts.Enabled = true;
                    tbSegments.Enabled = false;
                    pWordCountOptions.Visible = true;
                  break;
                case SplitOptions.SplitType.SegmentNumbers:
                    nudWords.Enabled = false;
                    nudParts.Enabled = false;
                    tbSegments.Enabled = true;
                    pWordCountOptions.Visible = false;
               break;
            }
        }
        private void bindPercentView()
        {
            if (chSplitCheckPercent.Checked)
                nudPercent.Enabled = true;
            else nudPercent.Enabled = false;
        }

        private string validateInput()
        {
            if (rbSegNumbers.Checked)
            {
                AppOptions.splitOpts.setSegmentIDs(tbSegments.Text.Trim());
                if (AppOptions.splitOpts.SegmentIDs.Count < 1)
                    return Properties.Resources.errSplitSegNumbers;
            }
            return "";
        }

        private void saveOptions()
        {
            AppOptions.splitOpts.IsPercent = chSplitCheckPercent.Checked;
            AppOptions.splitOpts.PercMax = int.Parse(nudPercent.Value.ToString());
            if (rbWordCount.Checked)
            {
                AppOptions.splitOpts.Criterion = SplitOptions.SplitType.WordsCount;
                AppOptions.splitOpts.WordsCount = int.Parse(nudWords.Value.ToString());
            }
            else if (rbEqualParts.Checked)
            {
                AppOptions.splitOpts.Criterion = SplitOptions.SplitType.EqualParts;
                AppOptions.splitOpts.PartsCount = int.Parse(nudParts.Value.ToString());
            }
            else if (rbSegNumbers.Checked)
            {
                AppOptions.splitOpts.Criterion = SplitOptions.SplitType.SegmentNumbers;
                AppOptions.segmentIDs = tbSegments.Text.Trim();
                AppOptions.splitOpts.setSegmentIDs(AppOptions.segmentIDs);
            }

            AppOptions.splitOpts.SplitNonCountStatus.Clear();
            for (int i = 0; i < clbSegments.Items.Count; i++ )
                if (clbSegments.GetItemCheckState(i) == CheckState.Checked)
                    AppOptions.splitOpts.SplitNonCountStatus.Add(TagSegStatus.getTagSegStatus((string)clbSegments.Items[i]));
        }

        private void OptionsPage_SetActive(object sender, CancelEventArgs e)
        {
            this.Banner.Title = Properties.Resources.msgSplitOptsTitle;
            this.Banner.Subtitle = Properties.Resources.msgSplitOptsSubtitle;
            bindSegments();

            bindOptions();

            SetWizardButtons(WizardButtons.Back | WizardButtons.Finish);
        }

        private void OptionsPage_WizardNext(object sender, WizardPageEventArgs e)
        {
            saveOptions();
        }
        private void OptionsPage_WizardPreNext(object sender, WizardPageEventArgs e)
        {
            // validation
            string validMsg = validateInput();
            if (validMsg.Length == 0)
                this.AllowNext = true;
            else
            {
                this.AllowNext = false;
                MessageBox.Show(validMsg, Properties.Resources.SplitSettingsTitle);
            }
        }

        private void OptionsPage_WizardBack(object sender, WizardPageEventArgs e)
        {
            saveOptions();
        }

        // form functionality
        private void chSplitCheckPercent_CheckedChanged(object sender, EventArgs e)
        {
            bindPercentView();
        }

        private void rbWordCount_CheckedChanged(object sender, EventArgs e)
        {

            bindCriterionView(SplitOptions.SplitType.WordsCount);
        }

        private void rbEqualParts_CheckedChanged(object sender, EventArgs e)
        {
            bindCriterionView(SplitOptions.SplitType.EqualParts);
        }

        private void rbSegNumbers_CheckedChanged(object sender, EventArgs e)
        {
            bindCriterionView(SplitOptions.SplitType.SegmentNumbers);
        }
    }
}
