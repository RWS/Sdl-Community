using System.Collections.Generic;
using System.Windows.Forms;
using Sdl.Community.NumberVerifier.Model;
using Sdl.Core.Settings;

namespace Sdl.Community.NumberVerifier
{
    public partial class NumberVerifierUI : UserControl
    {

        public bool ReportAddedNumbers
        {
            get {return cb_ReportAddedNumbers.Checked; }
            set { cb_ReportAddedNumbers.Checked = value; }
        }

        public string AddedNumbersErrorType
        {
            get { return combo_AddedNumbersErrorType.Text; }
            set { combo_AddedNumbersErrorType.Text = value; }
        }

        public bool ReportRemovedNumbers
        {
            get { return cb_ReportRemovedNumbers.Checked; }
            set { cb_ReportRemovedNumbers.Checked = value; }
        }

        public string RemovedNumbersErrorType
        {
            get { return combo_RemovedNumbersErrorType.Text; }
            set { combo_RemovedNumbersErrorType.Text = value; }
        }

        public bool ReportModifiedNumbers
        {
            get { return cb_ReportModifiedNumbers.Checked; }
            set { cb_ReportModifiedNumbers.Checked = value; }
        }

        public string ModifiedNumbersErrorType
        {
            get { return combo_ModifiedNumbersErrorType.Text; }
            set { combo_ModifiedNumbersErrorType.Text = value; }
        }

        public bool ReportModifiedAlphanumerics
        {
            get { return cb_ReportModifiedAlphanumerics.Checked; }
            set { cb_ReportModifiedAlphanumerics.Checked = value; }
        }

		public bool HindiNumberVerification
		{
			get { return cb_Hindi.Checked; }
			set { cb_Hindi.Checked = value; }
		}

		public string ModifiedAlphanumericsErrorType
		{
			get { return combo_ModifiedAlphanumericsErrorType.Text; }
			set { combo_ModifiedAlphanumericsErrorType.Text = value; }
		}

		public bool CustomsSeparatorsAlphanumerics
		{
			get { return cb_customSeparators.Checked; }
			set { cb_customSeparators.Checked = value; }
		}
		
		public bool ReportBriefMessages
        {
            get { return rb_ReportBriefMessages.Checked; }
            set { rb_ReportBriefMessages.Checked = value; }
        }

        public bool ReportExtendedMessages
        {
            get { return rb_ReportExtendedMessages.Checked; }
            set { rb_ReportExtendedMessages.Checked = value; }
        }

        public bool ExcludeTagText
        {
            get { return cb_ExcludeTagText.Checked; }
            set { cb_ExcludeTagText.Checked = value; }
        }

        public bool AllowLocalizations
        {
            get { return rb_AllowLocalizations.Checked; }
            set { rb_AllowLocalizations.Checked = value; }
        }

        public bool PreventLocalizations
        {
            get { return rb_PreventLocalizations.Checked; }
            set { rb_PreventLocalizations.Checked = value; }
        }

        public bool RequireLocalizations
        {
            get { return rb_RequireLocalizations.Checked; }
            set { rb_RequireLocalizations.Checked = value; }
        }

        public bool SourceThousandsSpace
        {
            get { return cb_SourceThousandsSpace.Checked; }
            set { cb_SourceThousandsSpace.Checked = value; }
        }

        public bool SourceThousandsNobreakSpace
        {
            get { return cb_SourceThousandsNobreakSpace.Checked; }
            set { cb_SourceThousandsNobreakSpace.Checked = value; }
        }

        public bool SourceThousandsThinSpace
        {
            get { return cb_SourceThousandsThinSpace.Checked; }
            set { cb_SourceThousandsThinSpace.Checked = value; }
        }

        public bool SourceThousandsNobreakThinSpace
        {
            get { return cb_SourceThousandsNobreakThinSpace.Checked; }
            set { cb_SourceThousandsNobreakThinSpace.Checked = value; }
        }

        public bool SourceThousandsComma
        {
            get { return cb_SourceThousandsComma.Checked; }
            set { cb_SourceThousandsComma.Checked = value; }
        }

        public bool SourceThousandsPeriod
        {
            get { return cb_SourceThousandsPeriod.Checked; }
            set { cb_SourceThousandsPeriod.Checked = value; }
        }

        public bool SourceNoSeparator
        {
            get { return cb_SourceNoSeparator.Checked; }
            set { cb_SourceNoSeparator.Checked = value; }
        }

        public bool TargetThousandsSpace
        {
            get { return cb_TargetThousandsSpace.Checked; }
            set { cb_TargetThousandsSpace.Checked = value; }
        }

        public bool TargetThousandsNobreakSpace
        {
            get { return cb_TargetThousandsNobreakSpace.Checked; }
            set { cb_TargetThousandsNobreakSpace.Checked = value; }
        }

        public bool TargetThousandsThinSpace
        {
            get { return cb_TargetThousandsThinSpace.Checked; }
            set { cb_TargetThousandsThinSpace.Checked = value; }
        }

        public bool TargetThousandsNobreakThinSpace
        {
            get { return cb_TargetThousandsNobreakThinSpace.Checked; }
            set { cb_TargetThousandsNobreakThinSpace.Checked = value; }
        }

        public bool TargetThousandsComma
        {
            get { return cb_TargetThousandsComma.Checked; }
            set { cb_TargetThousandsComma.Checked = value; }
        }

        public bool TargetThousandsPeriod
        {
            get { return cb_TargetThousandsPeriod.Checked; }
            set { cb_TargetThousandsPeriod.Checked = value; }
        }

        public bool TargetNoSeparator
        {
            get { return cb_TargetNoSeparator.Checked; }
            set { cb_TargetNoSeparator.Checked = value; }
        }

        public bool SourceDecimalComma
        {
            get { return cb_SourceDecimalComma.Checked; }
            set { cb_SourceDecimalComma.Checked = value; }
        }

        public bool SourceDecimalPeriod
        {
            get { return cb_SourceDecimalPeriod.Checked; }
            set { cb_SourceDecimalPeriod.Checked = value; }
        }

        public bool TargetDecimalComma
        {
            get { return cb_TargetDecimalComma.Checked; }
            set { cb_TargetDecimalComma.Checked = value; }
        }

        public bool TargetDecimalPeriod
        {
            get { return cb_TargetDecimalPeriod.Checked; }
            set { cb_TargetDecimalPeriod.Checked = value; }
        }

        public bool ExcludeLockedSegments
        {
            get { return cb_ExcludeLockedSegments.Checked; }
            set { cb_ExcludeLockedSegments.Checked = value; }
        }
        public bool Exclude100Percents
        {
            get { return cb_Exclude100Percents.Checked; }
            set { cb_Exclude100Percents.Checked = value; }
        }

        public bool ExcludeUntranslatedSegments
        {
            get { return untranslatedCheck.Checked; }
            set { untranslatedCheck.Checked = value; }
        }

        public bool ExcludeDraftSegments
        {
            get { return draftCheck.Checked; }
            set { draftCheck.Checked = value; }
        }

        public bool SourceOmitLeadingZero
        {
            get { return sourceOmitZero.Checked; }
            set { sourceOmitZero.Checked = value; }
        }

        public bool TargetOmitLeadingZero
        {
            get { return targetOmitZero.Checked; }
            set { targetOmitZero.Checked = value; }
        }

        public bool SourceThousandsCustomSeparator
        {
            get { return customTSep.Checked; }
            set { customTSep.Checked = value; }
        }

        public bool TargetThousandsCustomSeparator
        {
            get { return targetTbox.Checked; }
            set { targetTbox.Checked = value; }
        }

        public bool SourceDecimalCustomSeparator
        {
            get { return customDSep.Checked; }
            set { customDSep.Checked = value; }
        }

        public bool TargetDecimalCustomSeparator
        {
            get { return customTargetSep.Checked; }
            set { customTargetSep.Checked = value; }
        }

        public string GetSourceThousandsCustomSeparator
        {
            get { return sourceTBox.Text; }
            set { sourceTBox.Text = value; }
        }

        public string GetTargetThousandsCustomSeparator
        {
            get { return customTBox.Text; }
            set { customTBox.Text = value; }
        }

		public string GetAlphanumericsCustomSeparator
		{
			get { return tb_customsSeparators.Text; }
			set { tb_customsSeparators.Text = value; }
		}

		public string GetSourceDecimalCustomSeparator
        {
            get { return sourceDBox.Text; }
            set { sourceDBox.Text = value; }
        }

        public string GetTargetDecimalCustomSeparator
        {
            get { return targetDBox.Text; }
            set { targetDBox.Text = value; }
        }
        public NumberVerifierUI()
        {
            InitializeComponent();
            this.VScroll = true;
        }
		public Setting<List<TargetFileSetting>> TargetFileSettings { get; set; }

		private void rb_PreventLocalizations_CheckedChanged(object sender, System.EventArgs e)
        {
           

            #region soutce separators

            if (TargetThousandsSpace)
            {
                cb_TargetThousandsSpace.Checked = false;
            }
            cb_TargetThousandsSpace.Enabled = false;

            if (TargetThousandsNobreakSpace)
            {
                cb_TargetThousandsNobreakSpace.Checked = false;
            }
            cb_TargetThousandsNobreakSpace.Enabled = false;

            if (TargetThousandsThinSpace)
            {
                cb_TargetThousandsThinSpace.Checked = false;
            }
            cb_TargetThousandsThinSpace.Enabled = false;

            if (TargetThousandsNobreakThinSpace)
            {
                cb_TargetThousandsNobreakThinSpace.Checked = false;
            }
            cb_TargetThousandsNobreakThinSpace.Enabled = false;

            if (TargetThousandsComma)
            {
                cb_TargetThousandsComma.Checked = false;
            }
            cb_TargetThousandsComma.Enabled = false;

            if (TargetThousandsPeriod)
            {
                cb_TargetThousandsPeriod.Checked = false;
            }
            cb_TargetThousandsPeriod.Enabled = false;

            if (TargetNoSeparator)
            {
                cb_TargetNoSeparator.Checked = false;
            }
            cb_TargetNoSeparator.Enabled = false;

            if (TargetDecimalComma)
            {
                cb_TargetDecimalComma.Checked = false;
            }
            cb_TargetDecimalComma.Enabled = false;

            if (TargetDecimalPeriod)
            {
                cb_TargetDecimalPeriod.Checked = false;
            }
            cb_TargetDecimalPeriod.Enabled = false;

            if (TargetThousandsCustomSeparator)
            {
                targetTbox.Checked = false;
            }
            targetTbox.Enabled = false;
            customTBox.Clear();
            customTBox.Enabled = false;

            if (TargetDecimalCustomSeparator)
            {
                customTargetSep.Checked = false;
            }
            customTargetSep.Enabled = false;
            targetDBox.Clear();
            targetDBox.Enabled = false;

			if(CustomsSeparatorsAlphanumerics)
			{
				cb_customSeparators.Checked = false;
			}
			cb_customSeparators.Enabled = false;
			tb_customsSeparators.Clear();
			tb_customsSeparators.Enabled = false;

			#endregion
		}

        private void EnableCheckBoxes()
        {

            #region target separators

            cb_TargetThousandsSpace.Enabled = true;
            cb_TargetThousandsNobreakSpace.Enabled = true;
            cb_TargetThousandsThinSpace.Enabled = true;
            cb_TargetThousandsNobreakThinSpace.Enabled = true;
            cb_TargetThousandsComma.Enabled = true;
            cb_TargetThousandsPeriod.Enabled = true;
            cb_TargetNoSeparator.Enabled = true;
            cb_TargetDecimalComma.Enabled = true;
            cb_TargetDecimalPeriod.Enabled = true;
            targetTbox.Enabled = true;
            customTBox.Enabled = true;
            customTargetSep.Enabled = true;
			cb_customSeparators.Enabled = true;
			tb_customsSeparators.Enabled = true;

			targetDBox.Enabled = true;

            #endregion
        }

        private void rb_RequireLocalizations_CheckedChanged(object sender, System.EventArgs e)
        {
            EnableCheckBoxes();
        }

        private void rb_AllowLocalizations_CheckedChanged(object sender, System.EventArgs e)
        {
            EnableCheckBoxes();
        }

		private void cb_customSeparators_CheckedChanged(object sender, System.EventArgs e)
		{

		}
	}
}
