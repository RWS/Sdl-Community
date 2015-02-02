using System.Windows.Forms;

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

        public string ModifiedAlphanumericsErrorType
        {
            get { return combo_ModifiedAlphanumericsErrorType.Text; }
            set { combo_ModifiedAlphanumericsErrorType.Text = value; }
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
        public NumberVerifierUI()
        {
            InitializeComponent();
            this.VScroll = true;
        }
    }
}
