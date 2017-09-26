using Sdl.Community.NumberVerifier.Interfaces;
using Sdl.Core.Settings;

using Sdl.Verification.Api;

namespace Sdl.Community.NumberVerifier
{
    /// <summary>
    /// This is the extension class that displays and controls the plug-in user interface,
    /// in which the verification setting(s) can be specified. This class is responsible for
    /// e.g. saving the setting(s) configured in the UI, for resetting the values to their defaults,
    /// and for properly disposing of the UI control.
    /// </summary>
    #region "Declaration"
    [GlobalVerifierSettingsPage(
    Id = "Number Settings Definition ID",
    Name = "Number Verifier Settings",
    Description = "The settings for the number verifier.",
    HelpTopic = "")]
    #endregion
    class NumberVerifierUIPage : AbstractSettingsPage
    {
        #region "PrivateMembers"
        private NumberVerifierUI _Control;
        private NumberVerifierSettings _ControlSettings;
        #endregion

        #region "control"
        // Return the UI control.
        #region "GetControl"
        public override object GetControl()
        {
            _ControlSettings = ((ISettingsBundle)DataSource).GetSettingsGroup<NumberVerifierSettings>();
            _ControlSettings.BeginEdit();
            if (_Control == null)
            {
                _Control = new NumberVerifierUI();
            }

            return _Control;
        }
        #endregion
        

        // Load data from the settings into the UI control.
        #region "OnActivate"

        public override void OnActivate()
        {
            _Control.AddedNumbersErrorType = _ControlSettings.AddedNumbersErrorType;
            _Control.RemovedNumbersErrorType = _ControlSettings.RemovedNumbersErrorType;
            _Control.ModifiedNumbersErrorType = _ControlSettings.ModifiedNumbersErrorType;
            _Control.ModifiedAlphanumericsErrorType = _ControlSettings.ModifiedAlphanumericsErrorType;

			_Control.ReportAddedNumbers = _ControlSettings.ReportAddedNumbers;
            _Control.ReportRemovedNumbers = _ControlSettings.ReportRemovedNumbers;
            _Control.ReportModifiedNumbers = _ControlSettings.ReportModifiedNumbers;
            _Control.ReportModifiedAlphanumerics = _ControlSettings.ReportModifiedAlphanumerics;
            _Control.ReportBriefMessages = _ControlSettings.ReportBriefMessages;
            _Control.ReportExtendedMessages = _ControlSettings.ReportExtendedMessages;
            _Control.ExcludeTagText = _ControlSettings.ExcludeTagText;
			_Control.CustomsSeparatorsAlphanumerics = _ControlSettings.CustomsSeparatorsAlphanumerics;
			_Control.HindiNumberVerification = _ControlSettings.HindiNumberVerification;
			_Control.AllowLocalizations = _ControlSettings.AllowLocalizations;
            _Control.PreventLocalizations = _ControlSettings.PreventLocalizations;
            _Control.RequireLocalizations = _ControlSettings.RequireLocalizations;

            _Control.SourceThousandsSpace = _ControlSettings.SourceThousandsSpace;
            _Control.SourceThousandsNobreakSpace = _ControlSettings.SourceThousandsNobreakSpace;
            _Control.SourceThousandsThinSpace = _ControlSettings.SourceThousandsThinSpace;
            _Control.SourceThousandsNobreakThinSpace = _ControlSettings.SourceThousandsNobreakThinSpace;
            _Control.SourceThousandsComma = _ControlSettings.SourceThousandsComma;
            _Control.SourceThousandsPeriod = _ControlSettings.SourceThousandsPeriod;
            _Control.SourceNoSeparator = _ControlSettings.SourceNoSeparator;
            _Control.TargetThousandsSpace = _ControlSettings.TargetThousandsSpace;
            _Control.TargetThousandsNobreakSpace = _ControlSettings.TargetThousandsNobreakSpace;
            _Control.TargetThousandsThinSpace = _ControlSettings.TargetThousandsThinSpace;
            _Control.TargetThousandsNobreakThinSpace = _ControlSettings.TargetThousandsNobreakThinSpace;
            _Control.TargetThousandsComma = _ControlSettings.TargetThousandsComma;
            _Control.TargetThousandsPeriod = _ControlSettings.TargetThousandsPeriod;
            _Control.TargetNoSeparator = _ControlSettings.TargetNoSeparator;
            _Control.SourceDecimalComma = _ControlSettings.SourceDecimalComma;
            _Control.SourceDecimalPeriod = _ControlSettings.SourceDecimalPeriod;
            _Control.TargetDecimalComma = _ControlSettings.TargetDecimalComma;
            _Control.TargetDecimalPeriod = _ControlSettings.TargetDecimalPeriod;
            _Control.ExcludeLockedSegments = _ControlSettings.ExcludeLockedSegments;
            _Control.Exclude100Percents = _ControlSettings.Exclude100Percents;
            _Control.ExcludeUntranslatedSegments = _ControlSettings.ExcludeUntranslatedSegments;
            _Control.ExcludeDraftSegments = _ControlSettings.ExcludeDraftSegments;
            _Control.SourceOmitLeadingZero = _ControlSettings.SourceOmitLeadingZero;
            _Control.TargetOmitLeadingZero = _ControlSettings.TargetOmitLeadingZero;
            _Control.SourceThousandsCustomSeparator = _ControlSettings.SourceThousandsCustomSeparator;
            _Control.TargetThousandsCustomSeparator= _ControlSettings.TargetThousandsCustomSeparator;
            _Control.SourceDecimalCustomSeparator= _ControlSettings.SourceDecimalCustomSeparator;
            _Control.TargetDecimalCustomSeparator= _ControlSettings.TargetDecimalCustomSeparator;
            _Control.GetSourceThousandsCustomSeparator= _ControlSettings.GetSourceThousandsCustomSeparator;
            _Control.GetTargetThousandsCustomSeparator= _ControlSettings.GetTargetThousandsCustomSeparator;
            _Control.GetSourceDecimalCustomSeparator= _ControlSettings.GetSourceDecimalCustomSeparator;
            _Control.GetTargetDecimalCustomSeparator= _ControlSettings.GetTargetDecimalCustomSeparator;
			_Control.GetAlphanumericsCustomSeparator = _ControlSettings.GetAlphanumericsCustomSeparator;
		}

        #endregion


        // Reset the values on the UI control.
        #region "ResetToDefaults"
        public override void ResetToDefaults()
        {
            _Control.AddedNumbersErrorType = _ControlSettings.AddedNumbersErrorType;
            _Control.RemovedNumbersErrorType = _ControlSettings.RemovedNumbersErrorType;
            _Control.ModifiedNumbersErrorType = _ControlSettings.ModifiedNumbersErrorType;
            _Control.ModifiedAlphanumericsErrorType = _ControlSettings.ModifiedAlphanumericsErrorType;
			_ControlSettings.Reset("ReportAddedNumbers");
            _Control.ReportAddedNumbers = _ControlSettings.ReportAddedNumbers;
            _ControlSettings.Reset("ReportRemovedNumbers");
            _Control.ReportRemovedNumbers = _ControlSettings.ReportRemovedNumbers;
            _ControlSettings.Reset("ReportModifiedNumbers");
            _Control.ReportModifiedNumbers = _ControlSettings.ReportModifiedNumbers;
            _ControlSettings.Reset("ReportModifiedAlphanumerics");
            _Control.ReportModifiedAlphanumerics = _ControlSettings.ReportModifiedAlphanumerics;
			_ControlSettings.Reset("CustomsSeparatorsAlphanumerics");
			_Control.CustomsSeparatorsAlphanumerics = _ControlSettings.CustomsSeparatorsAlphanumerics;
			_ControlSettings.Reset("HindiNumberVerification");
			_Control.HindiNumberVerification = _ControlSettings.HindiNumberVerification;
			_ControlSettings.Reset("ReportBriefMessages");
            _Control.ReportBriefMessages = _ControlSettings.ReportBriefMessages;
            _ControlSettings.Reset("ReportExtendedMessages");
            _Control.ReportExtendedMessages = _ControlSettings.ReportExtendedMessages;
            _ControlSettings.Reset("ExcludeTagText");
            _Control.ExcludeTagText = _ControlSettings.ExcludeTagText;
            _ControlSettings.Reset("AllowLocalizations");
            _Control.AllowLocalizations = _ControlSettings.AllowLocalizations;
            _ControlSettings.Reset("PreventLocalizations");
            _Control.PreventLocalizations = _ControlSettings.PreventLocalizations;
            _ControlSettings.Reset("RequireLocalizations");
            _Control.RequireLocalizations = _ControlSettings.RequireLocalizations;
            _ControlSettings.Reset("SourceThousandsSpace");
            _Control.SourceThousandsSpace = _ControlSettings.SourceThousandsSpace;
            _ControlSettings.Reset("SourceThousandsNobreakSpace");
            _Control.SourceThousandsNobreakSpace = _ControlSettings.SourceThousandsNobreakSpace;
            _ControlSettings.Reset("SourceThousandsThinSpace");
            _Control.SourceThousandsThinSpace = _ControlSettings.SourceThousandsThinSpace;
            _ControlSettings.Reset("SourceThousandsNobreakThinSpace");
            _Control.SourceThousandsNobreakThinSpace = _ControlSettings.SourceThousandsNobreakThinSpace;
            _ControlSettings.Reset("SourceThousandsComma");
            _Control.SourceThousandsComma = _ControlSettings.SourceThousandsComma;
            _ControlSettings.Reset("SourceThousandsPeriod");
            _Control.SourceThousandsPeriod = _ControlSettings.SourceThousandsPeriod;
            _ControlSettings.Reset("SourceNoSeparator");
            _Control.SourceNoSeparator = _ControlSettings.SourceNoSeparator;
            _ControlSettings.Reset("TargetThousandsSpace");
            _Control.TargetThousandsSpace = _ControlSettings.TargetThousandsSpace;
            _ControlSettings.Reset("TargetThousandsNobreakSpace");
            _Control.TargetThousandsNobreakSpace = _ControlSettings.TargetThousandsNobreakSpace;
            _ControlSettings.Reset("TargetThousandsThinSpace");
            _Control.TargetThousandsThinSpace = _ControlSettings.TargetThousandsThinSpace;
            _ControlSettings.Reset("TargetThousandsNobreakThinSpace");
            _Control.TargetThousandsNobreakThinSpace = _ControlSettings.TargetThousandsNobreakThinSpace;
            _ControlSettings.Reset("TargetThousandsComma");
            _Control.TargetThousandsComma = _ControlSettings.TargetThousandsComma;
            _ControlSettings.Reset("TargetThousandsPeriod");
            _Control.TargetThousandsPeriod = _ControlSettings.TargetThousandsPeriod;
            _ControlSettings.Reset("TargetNoSeparator");
            _Control.TargetNoSeparator = _ControlSettings.TargetNoSeparator;
            _ControlSettings.Reset("SourceDecimalComma");
            _Control.SourceDecimalComma = _ControlSettings.SourceDecimalComma;
            _ControlSettings.Reset("SourceDecimalPeriod");
            _Control.SourceDecimalPeriod = _ControlSettings.SourceDecimalPeriod;
            _ControlSettings.Reset("TargetDecimalComma");
            _Control.TargetDecimalComma = _ControlSettings.TargetDecimalComma;
            _ControlSettings.Reset("TargetDecimalPeriod");
            _Control.TargetDecimalPeriod = _ControlSettings.TargetDecimalPeriod;
            _ControlSettings.Reset("ExcludeLockedSegments");
            _Control.ExcludeLockedSegments = _ControlSettings.ExcludeLockedSegments;
            _ControlSettings.Reset("Exclude100Percents");
            _Control.Exclude100Percents = _ControlSettings.Exclude100Percents;
            _ControlSettings.Reset("ExcludeUntranslatedSegments");
            _Control.ExcludeUntranslatedSegments = _ControlSettings.ExcludeUntranslatedSegments;
            _ControlSettings.Reset("ExcludeUntranslatedSegments");
            _Control.ExcludeDraftSegments = _ControlSettings.ExcludeDraftSegments;
            _ControlSettings.Reset("ExcludeDraftSegments");
            _Control.SourceOmitLeadingZero = _ControlSettings.SourceOmitLeadingZero;
            _ControlSettings.Reset("SourceOmitLeadingZero");
            _Control.TargetOmitLeadingZero = _ControlSettings.TargetOmitLeadingZero;
            _ControlSettings.Reset("TargetOmitLeadingZero");
            _Control.SourceThousandsCustomSeparator = _ControlSettings.SourceThousandsCustomSeparator;
            _ControlSettings.Reset("SourceThousandsCustomSeparator");
            _Control.TargetThousandsCustomSeparator = _ControlSettings.TargetThousandsCustomSeparator;
            _ControlSettings.Reset("TargetThousandsCustomSeparator");
            _Control.SourceDecimalCustomSeparator = _ControlSettings.SourceDecimalCustomSeparator;
            _ControlSettings.Reset("SourceDecimalCustomSeparator");
            _Control.TargetDecimalCustomSeparator = _ControlSettings.TargetDecimalCustomSeparator;
            _ControlSettings.Reset("TargetDecimalCustomSeparator");
            _Control.GetSourceThousandsCustomSeparator =
            _ControlSettings.GetSourceThousandsCustomSeparator;
            _ControlSettings.Reset("GetSourceThousandsCustomSeparator");
            _Control.GetTargetThousandsCustomSeparator = _ControlSettings.GetTargetThousandsCustomSeparator;
            _ControlSettings.Reset("GetTargetThousandsCustomSeparator");
            _Control.GetSourceDecimalCustomSeparator = _ControlSettings.GetSourceDecimalCustomSeparator;
            _ControlSettings.Reset("GetSourceDecimalCustomSeparator");
            _Control.GetTargetDecimalCustomSeparator = _ControlSettings.GetTargetDecimalCustomSeparator;
            _ControlSettings.Reset("GetTargetDecimalCustomSeparator");
			_Control.GetAlphanumericsCustomSeparator = _ControlSettings.GetAlphanumericsCustomSeparator;
			_ControlSettings.Reset("GetAlphanumericsCustomSeparator");

		}
		#endregion


		public override bool ValidateInput()
        {
            return _Control.ValidateChildren();
        }


        // Save the values from the UI into settings class.
        #region "Save"

        public override void Save()
        {
            _ControlSettings.AddedNumbersErrorType= _Control.AddedNumbersErrorType;
            _ControlSettings.RemovedNumbersErrorType = _Control.RemovedNumbersErrorType;
            _ControlSettings.ModifiedNumbersErrorType = _Control.ModifiedNumbersErrorType;
            _ControlSettings.ModifiedAlphanumericsErrorType = _Control.ModifiedAlphanumericsErrorType;

			_ControlSettings.ReportAddedNumbers = _Control.ReportAddedNumbers;
            _ControlSettings.ReportRemovedNumbers = _Control.ReportRemovedNumbers;
            _ControlSettings.ReportModifiedNumbers = _Control.ReportModifiedNumbers;
            _ControlSettings.ReportModifiedAlphanumerics = _Control.ReportModifiedAlphanumerics;
            _ControlSettings.ReportBriefMessages = _Control.ReportBriefMessages;
            _ControlSettings.ReportExtendedMessages = _Control.ReportExtendedMessages;
            _ControlSettings.ExcludeTagText = _Control.ExcludeTagText;
			_ControlSettings.CustomsSeparatorsAlphanumerics = _Control.CustomsSeparatorsAlphanumerics;
			_ControlSettings.HindiNumberVerification = _Control.HindiNumberVerification;

			_ControlSettings.AllowLocalizations = _Control.AllowLocalizations;
            _ControlSettings.PreventLocalizations = _Control.PreventLocalizations;
            _ControlSettings.RequireLocalizations = _Control.RequireLocalizations;

            _ControlSettings.SourceThousandsSpace = _Control.SourceThousandsSpace;
            _ControlSettings.SourceThousandsNobreakSpace = _Control.SourceThousandsNobreakSpace;
            _ControlSettings.SourceThousandsThinSpace = _Control.SourceThousandsThinSpace;
            _ControlSettings.SourceThousandsNobreakThinSpace = _Control.SourceThousandsNobreakThinSpace;
		    _ControlSettings.SourceThousandsComma = _Control.SourceThousandsComma;
            _ControlSettings.SourceThousandsPeriod = _Control.SourceThousandsPeriod;
            _ControlSettings.SourceNoSeparator = _Control.SourceNoSeparator;
            _ControlSettings.TargetThousandsSpace = _Control.TargetThousandsSpace;
            _ControlSettings.TargetThousandsNobreakSpace = _Control.TargetThousandsNobreakSpace;
            _ControlSettings.TargetThousandsThinSpace = _Control.TargetThousandsThinSpace;
            _ControlSettings.TargetThousandsNobreakThinSpace = _Control.TargetThousandsNobreakThinSpace;
            _ControlSettings.TargetThousandsComma = _Control.TargetThousandsComma;
            _ControlSettings.TargetThousandsPeriod = _Control.TargetThousandsPeriod;
            _ControlSettings.SourceDecimalComma = _Control.SourceDecimalComma;
            _ControlSettings.SourceDecimalPeriod = _Control.SourceDecimalPeriod;
            _ControlSettings.TargetDecimalComma = _Control.TargetDecimalComma;
            _ControlSettings.TargetDecimalPeriod = _Control.TargetDecimalPeriod;
            _ControlSettings.TargetNoSeparator = _Control.TargetNoSeparator;
            _ControlSettings.ExcludeLockedSegments = _Control.ExcludeLockedSegments;
            _ControlSettings.Exclude100Percents = _Control.Exclude100Percents;
            _ControlSettings.ExcludeUntranslatedSegments = _Control.ExcludeUntranslatedSegments;
            _ControlSettings.ExcludeDraftSegments = _Control.ExcludeDraftSegments;
            _ControlSettings.SourceOmitLeadingZero = _Control.SourceOmitLeadingZero;
            _ControlSettings.TargetOmitLeadingZero = _Control.TargetOmitLeadingZero;
            _ControlSettings.SourceThousandsCustomSeparator = _Control.SourceThousandsCustomSeparator;
            _ControlSettings.TargetThousandsCustomSeparator = _Control.TargetThousandsCustomSeparator;
            _ControlSettings.SourceDecimalCustomSeparator = _Control.SourceDecimalCustomSeparator;
            _ControlSettings.TargetDecimalCustomSeparator = _Control.TargetDecimalCustomSeparator;
            _ControlSettings.GetSourceThousandsCustomSeparator = _Control.GetSourceThousandsCustomSeparator;
            _ControlSettings.GetTargetThousandsCustomSeparator = _Control.GetTargetThousandsCustomSeparator;
            _ControlSettings.GetSourceDecimalCustomSeparator = _Control.GetSourceDecimalCustomSeparator;
            _ControlSettings.GetTargetDecimalCustomSeparator = _Control.GetTargetDecimalCustomSeparator;
			_ControlSettings.GetAlphanumericsCustomSeparator = _Control.GetAlphanumericsCustomSeparator;

		}

		#endregion

		public override void OnDeactivate()
        {
			//         _ControlSettings.AddedNumbersErrorType = _Control.AddedNumbersErrorType;
			//         _ControlSettings.RemovedNumbersErrorType = _Control.RemovedNumbersErrorType;
			//         _ControlSettings.ModifiedNumbersErrorType = _Control.ModifiedNumbersErrorType;
			//         _ControlSettings.ModifiedAlphanumericsErrorType = _Control.ModifiedAlphanumericsErrorType;
			//		  _ControlSettings.CustomsSeparatorsAlphanumerics = _Control.CustomsSeparatorsAlphanumerics;

			//         _ControlSettings.ReportAddedNumbers = _Control.ReportAddedNumbers;
			//         _ControlSettings.ReportRemovedNumbers = _Control.ReportRemovedNumbers;
			//         _ControlSettings.ReportModifiedNumbers = _Control.ReportModifiedNumbers;
			//         _ControlSettings.ReportModifiedAlphanumerics = _Control.ReportModifiedAlphanumerics;
			//         _ControlSettings.ReportBriefMessages = _Control.ReportBriefMessages;
			//         _ControlSettings.ReportExtendedMessages = _Control.ReportExtendedMessages;
			//         _ControlSettings.ExcludeTagText = _Control.ExcludeTagText;
			//         _ControlSettings.AllowLocalizations = _Control.AllowLocalizations;
			//         _ControlSettings.PreventLocalizations = _Control.PreventLocalizations;
			//         _ControlSettings.RequireLocalizations = _Control.RequireLocalizations;
			//         _ControlSettings.SourceThousandsSpace = _Control.SourceThousandsSpace;
			//         _ControlSettings.SourceThousandsNobreakSpace = _Control.SourceThousandsNobreakSpace;
			//         _ControlSettings.SourceThousandsThinSpace = _Control.SourceThousandsThinSpace;
			//         _ControlSettings.SourceThousandsNobreakThinSpace = _Control.SourceThousandsNobreakThinSpace;
			//         _ControlSettings.SourceThousandsComma = _Control.SourceThousandsComma;
			//         _ControlSettings.SourceThousandsPeriod = _Control.SourceThousandsPeriod;
			//         _ControlSettings.SourceNoSeparator = _Control.SourceNoSeparator;
			//         _ControlSettings.TargetThousandsSpace = _Control.TargetThousandsSpace;
			//         _ControlSettings.TargetThousandsNobreakSpace = _Control.TargetThousandsNobreakSpace;
			//         _ControlSettings.TargetThousandsThinSpace = _Control.TargetThousandsThinSpace;
			//         _ControlSettings.TargetThousandsNobreakThinSpace = _Control.TargetThousandsNobreakThinSpace;
			//         _ControlSettings.TargetThousandsComma = _Control.TargetThousandsComma;
			//         _ControlSettings.TargetThousandsPeriod = _Control.TargetThousandsPeriod;
			//         _ControlSettings.TargetNoSeparator = _Control.TargetNoSeparator;
			//         _ControlSettings.SourceDecimalComma = _Control.SourceDecimalComma;
			//         _ControlSettings.SourceDecimalPeriod = _Control.SourceDecimalPeriod;
			//         _ControlSettings.TargetDecimalComma = _Control.TargetDecimalComma;
			//         _ControlSettings.TargetDecimalPeriod = _Control.TargetDecimalPeriod;
			//         _ControlSettings.ExcludeLockedSegments = _Control.ExcludeLockedSegments;
			//         _ControlSettings.Exclude100Percents = _Control.Exclude100Percents;
			//         _ControlSettings.ExcludeUntranslatedSegments = _Control.ExcludeUntranslatedSegments;
			//         _ControlSettings.ExcludeDraftSegments = _Control.ExcludeDraftSegments;
			//         _ControlSettings.SourceOmitLeadingZero = _Control.SourceOmitLeadingZero;
			//         _ControlSettings.TargetOmitLeadingZero = _Control.TargetOmitLeadingZero;
			//         _ControlSettings.SourceThousandsCustomSeparator = _Control.SourceThousandsCustomSeparator;
			//         _ControlSettings.TargetThousandsCustomSeparator = _Control.TargetThousandsCustomSeparator;
			//         _ControlSettings.SourceDecimalCustomSeparator = _Control.SourceDecimalCustomSeparator;
			//         _ControlSettings.TargetDecimalCustomSeparator = _Control.TargetDecimalCustomSeparator;
			//         _ControlSettings.GetSourceThousandsCustomSeparator = _Control.GetSourceThousandsCustomSeparator;
			//         _ControlSettings.GetTargetThousandsCustomSeparator = _Control.GetTargetThousandsCustomSeparator;
			//         _ControlSettings.GetSourceDecimalCustomSeparator = _Control.GetSourceDecimalCustomSeparator;
			//         _ControlSettings.GetTargetDecimalCustomSeparator = _Control.GetTargetDecimalCustomSeparator;
			_ControlSettings.GetAlphanumericsCustomSeparator = _Control.GetAlphanumericsCustomSeparator;
			_ControlSettings.HindiNumberVerification = _Control.HindiNumberVerification;
		}

		// Call EndEdit after all changes have been saved in the Save() call.
		#region "AfterSave"
		public override void AfterSave()
        {
            _ControlSettings.EndEdit();
        }
        #endregion


        // Cancel any pending changes.
        #region "Cancel"
        public override void Cancel()
        {
            _ControlSettings.CancelEdit();
        }
        #endregion

        // Properly dispose of the control.
        #region "Dispose"
        public override void Dispose()
        {
            _Control.Dispose();
        }
        #endregion

        #endregion
    }
}
