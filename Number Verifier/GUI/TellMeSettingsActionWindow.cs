using System.Drawing;
using System;
using System.Windows.Forms;
using Sdl.Core.Settings;
using Sdl.ProjectAutomation.FileBased;
using Sdl.TranslationStudioAutomation.IntegrationApi;

namespace Sdl.Community.NumberVerifier.GUI
{
	public partial class TellMeSettingsActionWindow : Form
	{
		public TellMeSettingsActionWindow()
		{
			InitializeComponent();
			KeyDown += TellMeSettingsActionWindow_KeyDown;

			var projectController = SdlTradosStudio.Application.GetController<ProjectsController>();

			CurrentProject = projectController.CurrentProject;
			CurrentProjectSettings = CurrentProject.GetSettings();

			NumberVerifierSettings = CurrentProjectSettings.GetSettingsGroup<NumberVerifierSettings>();
			Setup();

			AdjustWindowSize();
		}

		private void AdjustWindowSize()
		{
			var screen = Screen.FromControl(this);
			var workingArea = screen.WorkingArea;
			var maxWidth = workingArea.Width;
			var maxHeight = workingArea.Height;

			// Adjust window size
			var newWidth = Math.Min(this.Width, maxWidth);
			var newHeight = Math.Min(this.Height, maxHeight);
			this.Size = new Size(newWidth, newHeight);

			// Adjust window position
			var newX = Math.Max(workingArea.Left, Math.Min(this.Left, workingArea.Right - newWidth));
			var newY = Math.Max(workingArea.Top, Math.Min(this.Top, workingArea.Bottom - newHeight));
			this.Location = new Point(newX, newY);
		}

		private FileBasedProject CurrentProject { get; set; }

		private ISettingsBundle CurrentProjectSettings { get; set; }

		private NumberVerifierSettings NumberVerifierSettings { get; }

		private void OKButton_Click(object sender, System.EventArgs e)
		{
			NumberVerifierSettings.AddedNumbersErrorType = Ui.AddedNumbersErrorType;
			NumberVerifierSettings.RemovedNumbersErrorType = Ui.RemovedNumbersErrorType;
			NumberVerifierSettings.ModifiedNumbersErrorType = Ui.ModifiedNumbersErrorType;
			NumberVerifierSettings.ModifiedAlphanumericsErrorType = Ui.ModifiedAlphanumericsErrorType;
			NumberVerifierSettings.NumberFormatErrorType = Ui.NumberFormatErrorType;
			NumberVerifierSettings.CheckInOrder = Ui.CheckInOrder;
			NumberVerifierSettings.RegexExclusionList = Ui.RegexExclusionList;

			NumberVerifierSettings.ReportAddedNumbers = Ui.ReportAddedNumbers;
			NumberVerifierSettings.ReportRemovedNumbers = Ui.ReportRemovedNumbers;
			NumberVerifierSettings.ReportModifiedNumbers = Ui.ReportModifiedNumbers;
			NumberVerifierSettings.ReportModifiedAlphanumerics = Ui.ReportModifiedAlphanumerics;
			NumberVerifierSettings.ReportNumberFormatErrors = Ui.ReportNumberFormatErrors;
			NumberVerifierSettings.ReportBriefMessages = Ui.ReportBriefMessages;
			NumberVerifierSettings.ReportExtendedMessages = Ui.ReportExtendedMessages;
			NumberVerifierSettings.ExcludeTagText = Ui.ExcludeTagText;
			NumberVerifierSettings.CustomsSeparatorsAlphanumerics = Ui.CustomsSeparatorsAlphanumerics;

			NumberVerifierSettings.AllowLocalizations = Ui.AllowLocalizations;
			NumberVerifierSettings.PreventLocalizations = Ui.PreventLocalizations;
			NumberVerifierSettings.RequireLocalizations = Ui.RequireLocalizations;

			NumberVerifierSettings.SourceThousandsSpace = Ui.SourceThousandsSpace;
			NumberVerifierSettings.SourceThousandsNobreakSpace = Ui.SourceThousandsNobreakSpace;
			NumberVerifierSettings.SourceThousandsThinSpace = Ui.SourceThousandsThinSpace;
			NumberVerifierSettings.SourceThousandsNobreakThinSpace = Ui.SourceThousandsNobreakThinSpace;
			NumberVerifierSettings.SourceThousandsComma = Ui.SourceThousandsComma;
			NumberVerifierSettings.SourceThousandsPeriod = Ui.SourceThousandsPeriod;
			NumberVerifierSettings.SourceNoSeparator = Ui.SourceNoSeparator;
			NumberVerifierSettings.TargetThousandsSpace = Ui.TargetThousandsSpace;
			NumberVerifierSettings.TargetThousandsNobreakSpace = Ui.TargetThousandsNobreakSpace;
			NumberVerifierSettings.TargetThousandsThinSpace = Ui.TargetThousandsThinSpace;
			NumberVerifierSettings.TargetThousandsNobreakThinSpace = Ui.TargetThousandsNobreakThinSpace;
			NumberVerifierSettings.TargetThousandsComma = Ui.TargetThousandsComma;
			NumberVerifierSettings.TargetThousandsPeriod = Ui.TargetThousandsPeriod;
			NumberVerifierSettings.SourceDecimalComma = Ui.SourceDecimalComma;
			NumberVerifierSettings.SourceDecimalPeriod = Ui.SourceDecimalPeriod;
			NumberVerifierSettings.TargetDecimalComma = Ui.TargetDecimalComma;
			NumberVerifierSettings.TargetDecimalPeriod = Ui.TargetDecimalPeriod;
			NumberVerifierSettings.TargetNoSeparator = Ui.TargetNoSeparator;
			NumberVerifierSettings.ExcludeLockedSegments = Ui.ExcludeLockedSegments;
			NumberVerifierSettings.Exclude100Percents = Ui.Exclude100Percents;
			NumberVerifierSettings.ExcludeUntranslatedSegments = Ui.ExcludeUntranslatedSegments;
			NumberVerifierSettings.ExcludeDraftSegments = Ui.ExcludeDraftSegments;
			NumberVerifierSettings.SourceOmitLeadingZero = Ui.SourceOmitLeadingZero;
			NumberVerifierSettings.TargetOmitLeadingZero = Ui.TargetOmitLeadingZero;
			NumberVerifierSettings.SourceThousandsCustom = Ui.SourceThousandsCustomSeparator;
			NumberVerifierSettings.TargetThousandsCustom = Ui.TargetThousandsCustomSeparator;
			NumberVerifierSettings.SourceDecimalCustom = Ui.SourceDecimalCustomSeparator;
			NumberVerifierSettings.TargetDecimalCustom = Ui.TargetDecimalCustomSeparator;
			NumberVerifierSettings.SourceThousandsCustomSeparator = Ui.GetSourceThousandsCustomSeparator;
			NumberVerifierSettings.TargetThousandsCustomSeparator = Ui.GetTargetThousandsCustomSeparator;
			NumberVerifierSettings.SourceDecimalCustomSeparator = Ui.GetSourceDecimalCustomSeparator;
			NumberVerifierSettings.TargetDecimalCustomSeparator = Ui.GetTargetDecimalCustomSeparator;
			NumberVerifierSettings.AlphanumericsCustomSeparator = Ui.GetAlphanumericsCustomSeparator;

			CurrentProject.UpdateSettings(CurrentProjectSettings);
		}

		private void Setup()
		{
			Ui.AddedNumbersErrorType = NumberVerifierSettings.AddedNumbersErrorType;
			Ui.RemovedNumbersErrorType = NumberVerifierSettings.RemovedNumbersErrorType;
			Ui.ModifiedNumbersErrorType = NumberVerifierSettings.ModifiedNumbersErrorType;
			Ui.ModifiedAlphanumericsErrorType = NumberVerifierSettings.ModifiedAlphanumericsErrorType;
			Ui.NumberFormatErrorType = NumberVerifierSettings.NumberFormatErrorType;
			Ui.CheckInOrder = NumberVerifierSettings.CheckInOrder;
			Ui.RegexExclusionList = NumberVerifierSettings.RegexExclusionList ?? new System.Collections.Generic.List<Model.RegexPattern>();

			Ui.ReportAddedNumbers = NumberVerifierSettings.ReportAddedNumbers;
			Ui.ReportRemovedNumbers = NumberVerifierSettings.ReportRemovedNumbers;
			Ui.ReportModifiedNumbers = NumberVerifierSettings.ReportModifiedNumbers;
			Ui.ReportModifiedAlphanumerics = NumberVerifierSettings.ReportModifiedAlphanumerics;
			Ui.ReportNumberFormatErrors = NumberVerifierSettings.ReportNumberFormatErrors;
			Ui.ReportBriefMessages = NumberVerifierSettings.ReportBriefMessages;
			Ui.ReportExtendedMessages = NumberVerifierSettings.ReportExtendedMessages;
			Ui.ExcludeTagText = NumberVerifierSettings.ExcludeTagText;
			Ui.CustomsSeparatorsAlphanumerics = NumberVerifierSettings.CustomsSeparatorsAlphanumerics;
			Ui.AllowLocalizations = NumberVerifierSettings.AllowLocalizations;
			Ui.PreventLocalizations = NumberVerifierSettings.PreventLocalizations;
			Ui.RequireLocalizations = NumberVerifierSettings.RequireLocalizations;

			Ui.SourceThousandsSpace = NumberVerifierSettings.SourceThousandsSpace;
			Ui.SourceThousandsNobreakSpace = NumberVerifierSettings.SourceThousandsNobreakSpace;
			Ui.SourceThousandsThinSpace = NumberVerifierSettings.SourceThousandsThinSpace;
			Ui.SourceThousandsNobreakThinSpace = NumberVerifierSettings.SourceThousandsNobreakThinSpace;
			Ui.SourceThousandsComma = NumberVerifierSettings.SourceThousandsComma;
			Ui.SourceThousandsPeriod = NumberVerifierSettings.SourceThousandsPeriod;
			Ui.SourceNoSeparator = NumberVerifierSettings.SourceNoSeparator;
			Ui.TargetThousandsSpace = NumberVerifierSettings.TargetThousandsSpace;
			Ui.TargetThousandsNobreakSpace = NumberVerifierSettings.TargetThousandsNobreakSpace;
			Ui.TargetThousandsThinSpace = NumberVerifierSettings.TargetThousandsThinSpace;
			Ui.TargetThousandsNobreakThinSpace = NumberVerifierSettings.TargetThousandsNobreakThinSpace;
			Ui.TargetThousandsComma = NumberVerifierSettings.TargetThousandsComma;
			Ui.TargetThousandsPeriod = NumberVerifierSettings.TargetThousandsPeriod;
			Ui.TargetNoSeparator = NumberVerifierSettings.TargetNoSeparator;
			Ui.SourceDecimalComma = NumberVerifierSettings.SourceDecimalComma;
			Ui.SourceDecimalPeriod = NumberVerifierSettings.SourceDecimalPeriod;
			Ui.TargetDecimalComma = NumberVerifierSettings.TargetDecimalComma;
			Ui.TargetDecimalPeriod = NumberVerifierSettings.TargetDecimalPeriod;
			Ui.ExcludeLockedSegments = NumberVerifierSettings.ExcludeLockedSegments;
			Ui.Exclude100Percents = NumberVerifierSettings.Exclude100Percents;
			Ui.ExcludeUntranslatedSegments = NumberVerifierSettings.ExcludeUntranslatedSegments;
			Ui.ExcludeDraftSegments = NumberVerifierSettings.ExcludeDraftSegments;
			Ui.SourceOmitLeadingZero = NumberVerifierSettings.SourceOmitLeadingZero;
			Ui.TargetOmitLeadingZero = NumberVerifierSettings.TargetOmitLeadingZero;
			Ui.SourceThousandsCustomSeparator = NumberVerifierSettings.SourceThousandsCustom;
			Ui.TargetThousandsCustomSeparator = NumberVerifierSettings.TargetThousandsCustom;
			Ui.SourceDecimalCustomSeparator = NumberVerifierSettings.SourceDecimalCustom;
			Ui.TargetDecimalCustomSeparator = NumberVerifierSettings.TargetDecimalCustom;
			Ui.GetSourceThousandsCustomSeparator = NumberVerifierSettings.SourceThousandsCustomSeparator;
			Ui.GetTargetThousandsCustomSeparator = NumberVerifierSettings.TargetThousandsCustomSeparator;
			Ui.GetSourceDecimalCustomSeparator = NumberVerifierSettings.SourceDecimalCustomSeparator;
			Ui.GetTargetDecimalCustomSeparator = NumberVerifierSettings.TargetDecimalCustomSeparator;
			Ui.GetAlphanumericsCustomSeparator = NumberVerifierSettings.AlphanumericsCustomSeparator;
		}

		private void TellMeSettingsActionWindow_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode != Keys.Escape)
				return;

			DialogResult = DialogResult.Cancel;
			Close();
		}
	}
}