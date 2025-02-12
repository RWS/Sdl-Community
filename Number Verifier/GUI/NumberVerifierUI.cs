using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Sdl.Community.NumberVerifier.GUI;
using Sdl.Community.NumberVerifier.Helpers;
using Sdl.Community.NumberVerifier.Model;

namespace Sdl.Community.NumberVerifier
{
    public partial class NumberVerifierUI : UserControl
    {
	    private string _exampleDNormalizedValue;
	    private string _exampleTNormalizedValue;
	    private string _exampleOmitLeadingZeroValue;

	    public bool CheckInOrder
		{
			get => cb_InOrder.Checked;
			set => cb_InOrder.Checked = value;
		}


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

		public bool ReportNumberFormatErrors
		{
			get { return cb_ReportNumberFormatErrors.Checked; }
			set { cb_ReportNumberFormatErrors.Checked = value; }
		}

		public string NumberFormatErrorType
		{
			get { return combo_NumberFormatErrorType.Text; }
			set { combo_NumberFormatErrorType.Text = value; }
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
            get { return targetTBox.Checked; }
            set { targetTBox.Checked = value; }
        }

        public bool SourceDecimalCustomSeparator
        {
            get { return customDSep.Checked; }
            set { customDSep.Checked = value; }
        }

        public bool TargetDecimalCustomSeparator
        {
            get { return customTargetDSep.Checked; }
            set { customTargetDSep.Checked = value; }
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

			InitializeExample();
		}

		private void InitializeExample()
		{
			_exampleTNormalizedValue = "1t300t000";
			var exampleTDefaultValue = "1300000";

			_exampleDNormalizedValue = "1d25";
			var exampleDDefaultValue = "125";

			_exampleOmitLeadingZeroValue = "0,25";


			tLabel1.Text = exampleTDefaultValue;
			tLabel2.Text = exampleTDefaultValue;
			tLabel3.Text = exampleTDefaultValue;
			tLabel4.Text = exampleTDefaultValue;
			tLabel5.Text = exampleTDefaultValue;
			tLabel6.Text = exampleTDefaultValue;
			tLabel7.Text = exampleTDefaultValue;
			tLabel8.Text = exampleTDefaultValue;
			
			ttlabel1.Text = exampleTDefaultValue;
			ttlabel2.Text = exampleTDefaultValue;
			ttlabel3.Text = exampleTDefaultValue;
			ttlabel4.Text = exampleTDefaultValue;
			ttlabel5.Text = exampleTDefaultValue;
			ttlabel6.Text = exampleTDefaultValue;
			ttlabel7.Text = exampleTDefaultValue;
			ttlabel8.Text = exampleTDefaultValue;

			dlabel1.Text = exampleDDefaultValue;
			dlabel2.Text = exampleDDefaultValue;
			dlabel3.Text = exampleDDefaultValue;
			tdlabel1.Text = exampleDDefaultValue;
			tdlabel2.Text = exampleDDefaultValue;
			tdlabel3.Text = exampleDDefaultValue;


			tLabel1.Text = _exampleTNormalizedValue.Replace("t", cb_SourceThousandsSpace.Checked ? " " : string.Empty);
			tLabel1.ForeColor = cb_SourceThousandsSpace.Checked ? Color.Green : Color.Blue;

			tLabel2.Text = _exampleTNormalizedValue.Replace("t", cb_SourceThousandsNobreakSpace.Checked ? " " : string.Empty);
			tLabel2.ForeColor = cb_SourceThousandsNobreakSpace.Checked ? Color.Green : Color.Blue;

			tLabel3.Text = _exampleTNormalizedValue.Replace("t", cb_SourceThousandsThinSpace.Checked ? " " : string.Empty);
			tLabel3.ForeColor = cb_SourceThousandsThinSpace.Checked ? Color.Green : Color.Blue;

			tLabel4.Text = _exampleTNormalizedValue.Replace("t", cb_SourceThousandsNobreakThinSpace.Checked ? " " : string.Empty);
			tLabel4.ForeColor = cb_SourceThousandsNobreakThinSpace.Checked ? Color.Green : Color.Blue;

			tLabel5.Text = _exampleTNormalizedValue.Replace("t", cb_SourceThousandsComma.Checked ? "," : string.Empty);
			tLabel5.ForeColor = cb_SourceThousandsComma.Checked ? Color.Green : Color.Blue;

			tLabel6.Text = _exampleTNormalizedValue.Replace("t", cb_SourceThousandsPeriod.Checked ? "." : string.Empty);
			tLabel6.ForeColor = cb_SourceThousandsPeriod.Checked ? Color.Green : Color.Blue;

			tLabel7.ForeColor = cb_SourceNoSeparator.Checked ? Color.Green : Color.Blue;

			tLabel8.Text = _exampleTNormalizedValue.Replace("t", customTSep.Checked && !string.IsNullOrEmpty(sourceTBox.Text) ? sourceTBox.Text : string.Empty);
			tLabel8.ForeColor = customTSep.Checked && !string.IsNullOrEmpty(sourceTBox.Text) ? Color.Green : Color.Blue;


			ttlabel1.Text = _exampleTNormalizedValue.Replace("t", cb_TargetThousandsSpace.Checked ? " " : string.Empty);
			ttlabel1.ForeColor = cb_TargetThousandsSpace.Checked ? Color.Green : Color.Blue;

			ttlabel2.Text = _exampleTNormalizedValue.Replace("t", cb_TargetThousandsNobreakSpace.Checked ? " " : string.Empty);
			ttlabel2.ForeColor = cb_TargetThousandsNobreakSpace.Checked ? Color.Green : Color.Blue;

			ttlabel3.Text = _exampleTNormalizedValue.Replace("t", cb_TargetThousandsThinSpace.Checked ? " " : string.Empty);
			ttlabel3.ForeColor = cb_TargetThousandsThinSpace.Checked ? Color.Green : Color.Blue;

			ttlabel4.Text = _exampleTNormalizedValue.Replace("t", cb_TargetThousandsNobreakThinSpace.Checked ? " " : string.Empty);
			ttlabel4.ForeColor = cb_TargetThousandsNobreakThinSpace.Checked ? Color.Green : Color.Blue;

			ttlabel5.Text = _exampleTNormalizedValue.Replace("t", cb_TargetThousandsComma.Checked ? "," : string.Empty);
			ttlabel5.ForeColor = cb_TargetThousandsComma.Checked ? Color.Green : Color.Blue;

			ttlabel6.Text = _exampleTNormalizedValue.Replace("t", cb_TargetThousandsPeriod.Checked ? "." : string.Empty);
			ttlabel6.ForeColor = cb_TargetThousandsPeriod.Checked ? Color.Green : Color.Blue;

			ttlabel7.ForeColor = cb_TargetNoSeparator.Checked ? Color.Green : Color.Blue;

			ttlabel8.Text = _exampleTNormalizedValue.Replace("t", targetTBox.Checked && !string.IsNullOrEmpty(customTBox.Text) ? customTBox.Text : string.Empty);
			ttlabel8.ForeColor = targetTBox.Checked && !string.IsNullOrEmpty(customTBox.Text) ? Color.Green : Color.Blue;


			dlabel1.Text = _exampleDNormalizedValue.Replace("d", cb_SourceDecimalComma.Checked ? "," : string.Empty);
			dlabel1.ForeColor = cb_SourceDecimalComma.Checked ? Color.Green : Color.Blue;

			dlabel2.Text = _exampleDNormalizedValue.Replace("d", cb_SourceDecimalPeriod.Checked ? "." : string.Empty);
			dlabel2.ForeColor = cb_SourceDecimalPeriod.Checked ? Color.Green : Color.Blue;

			dlabel3.Text = _exampleDNormalizedValue.Replace("d", customDSep.Checked && !string.IsNullOrEmpty(sourceDBox.Text) ? sourceDBox.Text : string.Empty);
			dlabel3.ForeColor = customDSep.Checked && !string.IsNullOrEmpty(sourceDBox.Text) ? Color.Green : Color.Blue;

			
			tdlabel1.Text = _exampleDNormalizedValue.Replace("d", cb_TargetDecimalComma.Checked ? "," : string.Empty);
			tdlabel1.ForeColor = cb_TargetDecimalComma.Checked ? Color.Green : Color.Blue;

			tdlabel2.Text = _exampleDNormalizedValue.Replace("d", cb_TargetDecimalPeriod.Checked ? "." : string.Empty);
			tdlabel2.ForeColor = cb_TargetDecimalPeriod.Checked ? Color.Green : Color.Blue;

			tdlabel3.Text = _exampleDNormalizedValue.Replace("d", customTargetDSep.Checked && !string.IsNullOrEmpty(targetDBox.Text)  ? targetDBox.Text : string.Empty);
			tdlabel3.ForeColor = customTargetDSep.Checked && !string.IsNullOrEmpty(targetDBox.Text) ? Color.Green : Color.Blue;

			sourceOmitLeadingZeroLabel.Text = sourceOmitZero.Checked ? _exampleOmitLeadingZeroValue.Replace("0", string.Empty) : _exampleOmitLeadingZeroValue;
			sourceOmitLeadingZeroLabel.ForeColor = sourceOmitZero.Checked ? Color.Green : Color.Blue;

			targetOmitLeadingZeroLabel.Text = targetOmitZero.Checked ? _exampleOmitLeadingZeroValue.Replace("0", string.Empty) : _exampleOmitLeadingZeroValue;
			targetOmitLeadingZeroLabel.ForeColor = targetOmitZero.Checked ? Color.Green : Color.Blue;
		}

		private void rb_PreventLocalizations_CheckedChanged(object sender, System.EventArgs e)
        {
	        if (TargetOmitLeadingZero)
	        {
				TargetOmitLeadingZero = false;
			}
			targetMisBox.Enabled = false;

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
                targetTBox.Checked = false;
            }
            targetTBox.Enabled = false;
            customTBox.Clear();
            customTBox.Enabled = false;

            if (TargetDecimalCustomSeparator)
            {
                customTargetDSep.Checked = false;
            }
            customTargetDSep.Enabled = false;
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
            targetTBox.Enabled = true;
            customTBox.Enabled = true;
            customTargetDSep.Enabled = true;
			cb_customSeparators.Enabled = true;
			tb_customsSeparators.Enabled = true;
			targetMisBox.Enabled = true;
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

	    private void CheckedChanged(object sender, System.EventArgs e)
	    {
		    if (sender is not CheckBox checkBox) return;


		    var isChecked = checkBox.Checked;
		    switch (checkBox.Name)
		    {
			    case "cb_SourceThousandsSpace":
				    tLabel1.Text = _exampleTNormalizedValue.Replace("t", isChecked ? " " : string.Empty);
				    tLabel1.ForeColor = isChecked ? Color.Green : Color.Blue;
				    break;
			    case "cb_SourceThousandsNobreakSpace":
				    tLabel2.Text = _exampleTNormalizedValue.Replace("t", isChecked ? " " : string.Empty);
				    tLabel2.ForeColor = isChecked ? Color.Green : Color.Blue;
					break;
			    case "cb_SourceThousandsThinSpace":
				    tLabel3.Text = _exampleTNormalizedValue.Replace("t", isChecked ? " " : string.Empty);
				    tLabel3.ForeColor = isChecked ? Color.Green : Color.Blue;
					break;
			    case "cb_SourceThousandsNobreakThinSpace":
				    tLabel4.Text = _exampleTNormalizedValue.Replace("t", isChecked ? " " : string.Empty);
				    tLabel4.ForeColor = isChecked ? Color.Green : Color.Blue;
					break;
			    case "cb_SourceThousandsComma":
				    tLabel5.Text = _exampleTNormalizedValue.Replace("t", isChecked ? "," : string.Empty);
				    tLabel5.ForeColor = isChecked ? Color.Green : Color.Blue;
					break;
			    case "cb_SourceThousandsPeriod":
				    tLabel6.Text = _exampleTNormalizedValue.Replace("t", isChecked ? "." : string.Empty);
				    tLabel6.ForeColor = isChecked ? Color.Green : Color.Blue;
					break;
				case "cb_SourceNoSeparator":
					tLabel7.ForeColor = isChecked ? Color.Green : Color.Blue;
					break;
			    case "customTSep":
				    tLabel8.Text = _exampleTNormalizedValue.Replace("t", isChecked && !string.IsNullOrEmpty(sourceTBox.Text) ? sourceTBox.Text : string.Empty);
				    tLabel8.ForeColor = isChecked && !string.IsNullOrEmpty(sourceTBox.Text) ? Color.Green : Color.Blue;
					break;


				case "cb_SourceDecimalComma":
					dlabel1.Text = _exampleDNormalizedValue.Replace("d", isChecked ? "," : string.Empty);
					dlabel1.ForeColor = isChecked ? Color.Green : Color.Blue;
					break;
			    case "cb_SourceDecimalPeriod":
					dlabel2.Text = _exampleDNormalizedValue.Replace("d", isChecked ? "." : string.Empty);
				    dlabel2.ForeColor = isChecked ? Color.Green : Color.Blue;
					break;
			    case "customDSep":
					dlabel3.Text = _exampleDNormalizedValue.Replace("d", isChecked && !string.IsNullOrEmpty(sourceDBox.Text) ? sourceDBox.Text : string.Empty);
				    dlabel3.ForeColor = isChecked && !string.IsNullOrEmpty(sourceDBox.Text) ? Color.Green : Color.Blue;
					break;
			    
			    case "cb_TargetDecimalComma":
					tdlabel1.Text = _exampleDNormalizedValue.Replace("d", isChecked ? "," : string.Empty);
				    tdlabel1.ForeColor = isChecked ? Color.Green : Color.Blue;
					break;
			    case "cb_TargetDecimalPeriod":
					tdlabel2.Text = _exampleDNormalizedValue.Replace("d", isChecked ? "." : string.Empty);
				    tdlabel2.ForeColor = isChecked ? Color.Green : Color.Blue;
					break;
			    case "customTargetDSep":
					tdlabel3.Text = _exampleDNormalizedValue.Replace("d", isChecked && !string.IsNullOrEmpty(targetDBox.Text) ? targetDBox.Text : string.Empty);
				    tdlabel3.ForeColor = isChecked && !string.IsNullOrEmpty(targetDBox.Text) ? Color.Green : Color.Blue;
					break;

				case "cb_TargetThousandsSpace":
					ttlabel1.Text = _exampleTNormalizedValue.Replace("t", isChecked ? " " : string.Empty);
					ttlabel1.ForeColor = isChecked ? Color.Green : Color.Blue;
					break;
			    case "cb_TargetThousandsNobreakSpace":
				    ttlabel2.Text = _exampleTNormalizedValue.Replace("t", isChecked ? " " : string.Empty);
				    ttlabel2.ForeColor = isChecked ? Color.Green : Color.Blue;
					break;
			    case "cb_TargetThousandsThinSpace":
				    ttlabel3.Text = _exampleTNormalizedValue.Replace("t", isChecked ? " " : string.Empty);
				    ttlabel3.ForeColor = isChecked ? Color.Green : Color.Blue;
					break;
			    case "cb_TargetThousandsNobreakThinSpace":
				    ttlabel4.Text = _exampleTNormalizedValue.Replace("t", isChecked ? " " : string.Empty);
				    ttlabel4.ForeColor = isChecked ? Color.Green : Color.Blue;
					break;
			    case "cb_TargetThousandsComma":
				    ttlabel5.Text = _exampleTNormalizedValue.Replace("t", isChecked ? "," : string.Empty);
				    ttlabel5.ForeColor = isChecked ? Color.Green : Color.Blue;
					break;
			    case "cb_TargetThousandsPeriod":
				    ttlabel6.Text = _exampleTNormalizedValue.Replace("t", isChecked ? "." : string.Empty);
				    ttlabel6.ForeColor = isChecked ? Color.Green : Color.Blue;
					break;
				case "cb_TargetNoSeparator":
					ttlabel7.ForeColor = isChecked ? Color.Green : Color.Blue;
					break;
				case "targetTBox":
				    ttlabel8.Text = _exampleTNormalizedValue.Replace("t", isChecked && !string.IsNullOrEmpty(customTBox.Text) ? customTBox.Text : string.Empty);
				    ttlabel8.ForeColor = isChecked && !string.IsNullOrEmpty(customTBox.Text) ? Color.Green : Color.Blue;
					break;

				case "sourceOmitZero":
					sourceOmitLeadingZeroLabel.Text = isChecked
						? _exampleOmitLeadingZeroValue.Replace("0", string.Empty)
						: _exampleOmitLeadingZeroValue;
					sourceOmitLeadingZeroLabel.ForeColor = isChecked ? Color.Green : Color.Blue;
					break;
			    case "targetOmitZero":
					targetOmitLeadingZeroLabel.Text = isChecked
						? _exampleOmitLeadingZeroValue.Replace("0", string.Empty)
						: _exampleOmitLeadingZeroValue;
				    targetOmitLeadingZeroLabel.ForeColor = isChecked ? Color.Green : Color.Blue;
					break;
			}

	    }

		private void TBox_TextChanged(object sender, System.EventArgs e)
		{
			if (sender is not TextBox textBox) return;
			switch (textBox.Name)
			{
				case "sourceTBox" when customTSep.Checked:
					tLabel8.Text = _exampleTNormalizedValue.Replace("t", sourceTBox.Text);
					tLabel8.ForeColor = string.IsNullOrEmpty(sourceTBox.Text) ? Color.Blue : Color.Green;
					break;
				case "customTBox" when targetTBox.Checked:
					ttlabel8.Text = _exampleTNormalizedValue.Replace("t", customTBox.Text);
					ttlabel8.ForeColor = string.IsNullOrEmpty(customTBox.Text) ? Color.Blue : Color.Green;
					break;

				case "sourceDBox" when customDSep.Checked:
					dlabel3.Text = _exampleDNormalizedValue.Replace("d", sourceDBox.Text);
					dlabel3.ForeColor = string.IsNullOrEmpty(sourceDBox.Text) ? Color.Blue : Color.Green;
					break;
				case "targetDBox" when customTargetDSep.Checked:
					tdlabel3.Text = _exampleDNormalizedValue.Replace("d", targetDBox.Text);
					tdlabel3.ForeColor = string.IsNullOrEmpty(targetDBox.Text) ? Color.Blue : Color.Green;
					break;
			}
		}

	    public List<RegexPattern> RegexExclusionList { get; set; }

	    private void button1_Click(object sender, System.EventArgs e)
	    {
		    var regexExclusionsGrid = new RegexExclusions(new RegexImporter());

		    regexExclusionsGrid.SetData(RegexExclusionList);
			regexExclusionsGrid.ShowDialog();

		    RegexExclusionList = regexExclusionsGrid.GetData();
	    }
	}
}
