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
            _Control.ExcludeUntranslatedSegments = _ControlSettings.ExcludeUntranslatedSegments.Value;
            _Control.SourceThousandsCustomSeparator = _ControlSettings.SourceThousandsCustomSeparator.Value;
            _Control.TargetThousandsCustomSeparator= _ControlSettings.TargetThousandsCustomSeparator.Value;
            _Control.SourceDecimalCustomSeparator= _ControlSettings.SourceDecimalCustomSeparator.Value;
            _Control.TargetDecimalCustomSeparator= _ControlSettings.TargetDecimalCustomSeparator.Value;
            _Control.GetSourceThousandsCustomSeparator= _ControlSettings.GetSourceThousandsCustomSeparator.Value;
            _Control.GetTargetThousandsCustomSeparator= _ControlSettings.GetTargetThousandsCustomSeparator.Value;
            _Control.GetSourceDecimalCustomSeparator= _ControlSettings.GetSourceDecimalCustomSeparator.Value;
            _Control.GetTargetDecimalCustomSeparator= _ControlSettings.GetTargetDecimalCustomSeparator.Value;
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
            _ControlSettings.ReportAddedNumbers.Reset();
            _Control.ReportAddedNumbers = _ControlSettings.ReportAddedNumbers;
            _ControlSettings.ReportRemovedNumbers.Reset();
            _Control.ReportRemovedNumbers = _ControlSettings.ReportRemovedNumbers;
            _ControlSettings.ReportModifiedNumbers.Reset();
            _Control.ReportModifiedNumbers = _ControlSettings.ReportModifiedNumbers;
            _ControlSettings.ReportModifiedAlphanumerics.Reset();
            _Control.ReportModifiedAlphanumerics = _ControlSettings.ReportModifiedAlphanumerics;
            _ControlSettings.ReportBriefMessages.Reset();
            _Control.ReportBriefMessages = _ControlSettings.ReportBriefMessages;
            _ControlSettings.ReportExtendedMessages.Reset();
            _Control.ReportExtendedMessages = _ControlSettings.ReportExtendedMessages;
            _ControlSettings.ExcludeTagText.Reset();
            _Control.ExcludeTagText = _ControlSettings.ExcludeTagText;
            _ControlSettings.AllowLocalizations.Reset();
            _Control.AllowLocalizations = _ControlSettings.AllowLocalizations;
            _ControlSettings.PreventLocalizations.Reset();
            _Control.PreventLocalizations = _ControlSettings.PreventLocalizations;
            _ControlSettings.RequireLocalizations.Reset();
            _Control.RequireLocalizations = _ControlSettings.RequireLocalizations;
            _ControlSettings.SourceThousandsSpace.Reset();
            _Control.SourceThousandsSpace = _ControlSettings.SourceThousandsSpace;
            _ControlSettings.SourceThousandsNobreakSpace.Reset();
            _Control.SourceThousandsNobreakSpace = _ControlSettings.SourceThousandsNobreakSpace;
            _ControlSettings.SourceThousandsThinSpace.Reset();
            _Control.SourceThousandsThinSpace = _ControlSettings.SourceThousandsThinSpace;
            _ControlSettings.SourceThousandsNobreakThinSpace.Reset();
            _Control.SourceThousandsNobreakThinSpace = _ControlSettings.SourceThousandsNobreakThinSpace;
            _ControlSettings.SourceThousandsComma.Reset();
            _Control.SourceThousandsComma = _ControlSettings.SourceThousandsComma;
            _ControlSettings.SourceThousandsPeriod.Reset();
            _Control.SourceThousandsPeriod = _ControlSettings.SourceThousandsPeriod;
            _ControlSettings.SourceNoSeparator.Reset();
            _Control.SourceNoSeparator = _ControlSettings.SourceNoSeparator;
            _ControlSettings.TargetThousandsSpace.Reset();
            _Control.TargetThousandsSpace = _ControlSettings.TargetThousandsSpace;
            _ControlSettings.TargetThousandsNobreakSpace.Reset();
            _Control.TargetThousandsNobreakSpace = _ControlSettings.TargetThousandsNobreakSpace;
            _ControlSettings.TargetThousandsThinSpace.Reset();
            _Control.TargetThousandsThinSpace = _ControlSettings.TargetThousandsThinSpace;
            _ControlSettings.TargetThousandsNobreakThinSpace.Reset();
            _Control.TargetThousandsNobreakThinSpace = _ControlSettings.TargetThousandsNobreakThinSpace;
            _ControlSettings.TargetThousandsComma.Reset();
            _Control.TargetThousandsComma = _ControlSettings.TargetThousandsComma;
            _ControlSettings.TargetThousandsPeriod.Reset();
            _Control.TargetThousandsPeriod = _ControlSettings.TargetThousandsPeriod;
            _ControlSettings.TargetNoSeparator.Reset();
            _Control.TargetNoSeparator = _ControlSettings.TargetNoSeparator;
            _ControlSettings.SourceDecimalComma.Reset();
            _Control.SourceDecimalComma = _ControlSettings.SourceDecimalComma;
            _ControlSettings.SourceDecimalPeriod.Reset();
            _Control.SourceDecimalPeriod = _ControlSettings.SourceDecimalPeriod;
            _ControlSettings.TargetDecimalComma.Reset();
            _Control.TargetDecimalComma = _ControlSettings.TargetDecimalComma;
            _ControlSettings.TargetDecimalPeriod.Reset();
            _Control.TargetDecimalPeriod = _ControlSettings.TargetDecimalPeriod;
            _ControlSettings.ExcludeLockedSegments.Reset();
            _Control.ExcludeLockedSegments = _ControlSettings.ExcludeLockedSegments;
            _ControlSettings.Exclude100Percents.Reset();
            _Control.Exclude100Percents = _ControlSettings.Exclude100Percents;
            _ControlSettings.ExcludeUntranslatedSegments.Reset();
            _Control.ExcludeUntranslatedSegments = _ControlSettings.ExcludeUntranslatedSegments;
            _ControlSettings.ExcludeUntranslatedSegments.Reset();
            _Control.SourceThousandsCustomSeparator = _ControlSettings.SourceThousandsCustomSeparator;
            _ControlSettings.SourceThousandsCustomSeparator.Reset();
            _Control.TargetThousandsCustomSeparator = _ControlSettings.TargetThousandsCustomSeparator;
            _ControlSettings.TargetThousandsCustomSeparator.Reset();
            _Control.SourceDecimalCustomSeparator = _ControlSettings.SourceDecimalCustomSeparator;
            _ControlSettings.SourceDecimalCustomSeparator.Reset();
            _Control.TargetDecimalCustomSeparator = _ControlSettings.TargetDecimalCustomSeparator;
            _ControlSettings.TargetDecimalCustomSeparator.Reset();
            _Control.GetSourceThousandsCustomSeparator =
               _ControlSettings.GetSourceThousandsCustomSeparator;
            _ControlSettings.GetSourceThousandsCustomSeparator.Reset();
            _Control.GetTargetThousandsCustomSeparator = _ControlSettings.GetTargetThousandsCustomSeparator;
            _ControlSettings.GetTargetThousandsCustomSeparator.Reset();
            _Control.GetSourceDecimalCustomSeparator = _ControlSettings.GetSourceDecimalCustomSeparator;
            _ControlSettings.GetSourceDecimalCustomSeparator.Reset();
            _Control.GetTargetDecimalCustomSeparator = _ControlSettings.GetTargetDecimalCustomSeparator;
            _ControlSettings.GetTargetDecimalCustomSeparator.Reset();


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
            _ControlSettings.AddedNumbersErrorType.Value = _Control.AddedNumbersErrorType;
            _ControlSettings.RemovedNumbersErrorType.Value = _Control.RemovedNumbersErrorType;
            _ControlSettings.ModifiedNumbersErrorType.Value = _Control.ModifiedNumbersErrorType;
            _ControlSettings.ModifiedAlphanumericsErrorType.Value = _Control.ModifiedAlphanumericsErrorType;
            _ControlSettings.ReportAddedNumbers.Value = _Control.ReportAddedNumbers;
            _ControlSettings.ReportRemovedNumbers.Value = _Control.ReportRemovedNumbers;
            _ControlSettings.ReportModifiedNumbers.Value = _Control.ReportModifiedNumbers;
            _ControlSettings.ReportModifiedAlphanumerics.Value = _Control.ReportModifiedAlphanumerics;
            _ControlSettings.ReportBriefMessages.Value = _Control.ReportBriefMessages;
            _ControlSettings.ReportExtendedMessages.Value = _Control.ReportExtendedMessages;
            _ControlSettings.ExcludeTagText.Value = _Control.ExcludeTagText;

            _ControlSettings.AllowLocalizations.Value = _Control.AllowLocalizations;
            _ControlSettings.PreventLocalizations.Value = _Control.PreventLocalizations;
            _ControlSettings.RequireLocalizations.Value = _Control.RequireLocalizations;

            _ControlSettings.SourceThousandsSpace.Value = _Control.SourceThousandsSpace;
            _ControlSettings.SourceThousandsNobreakSpace.Value = _Control.SourceThousandsNobreakSpace;
            _ControlSettings.SourceThousandsThinSpace.Value = _Control.SourceThousandsThinSpace;
            _ControlSettings.SourceThousandsNobreakThinSpace.Value = _Control.SourceThousandsNobreakThinSpace;
         _ControlSettings.SourceThousandsComma.Value = _Control.SourceThousandsComma;
            _ControlSettings.SourceThousandsPeriod.Value = _Control.SourceThousandsPeriod;
            _ControlSettings.SourceNoSeparator.Value = _Control.SourceNoSeparator;
            _ControlSettings.TargetThousandsSpace.Value = _Control.TargetThousandsSpace;
            _ControlSettings.TargetThousandsNobreakSpace.Value = _Control.TargetThousandsNobreakSpace;
            _ControlSettings.TargetThousandsThinSpace.Value = _Control.TargetThousandsThinSpace;
            _ControlSettings.TargetThousandsNobreakThinSpace.Value = _Control.TargetThousandsNobreakThinSpace;
            _ControlSettings.TargetThousandsComma.Value = _Control.TargetThousandsComma;
            _ControlSettings.TargetThousandsPeriod.Value = _Control.TargetThousandsPeriod;
            _ControlSettings.SourceDecimalComma.Value = _Control.SourceDecimalComma;
            _ControlSettings.SourceDecimalPeriod.Value = _Control.SourceDecimalPeriod;
            _ControlSettings.TargetDecimalComma.Value = _Control.TargetDecimalComma;
            _ControlSettings.TargetDecimalPeriod.Value = _Control.TargetDecimalPeriod;
            _ControlSettings.TargetNoSeparator.Value = _Control.TargetNoSeparator;
            _ControlSettings.ExcludeLockedSegments.Value = _Control.ExcludeLockedSegments;
            _ControlSettings.Exclude100Percents.Value = _Control.Exclude100Percents;
            _ControlSettings.ExcludeUntranslatedSegments.Value = _Control.ExcludeUntranslatedSegments;
            _ControlSettings.SourceThousandsCustomSeparator.Value = _Control.SourceThousandsCustomSeparator;
            _ControlSettings.TargetThousandsCustomSeparator.Value = _Control.TargetThousandsCustomSeparator;
            _ControlSettings.SourceDecimalCustomSeparator.Value = _Control.SourceDecimalCustomSeparator;
            _ControlSettings.TargetDecimalCustomSeparator.Value = _Control.TargetDecimalCustomSeparator;
            _ControlSettings.GetSourceThousandsCustomSeparator.Value = _Control.GetSourceThousandsCustomSeparator;
            _ControlSettings.GetTargetThousandsCustomSeparator.Value = _Control.GetTargetThousandsCustomSeparator;
            _ControlSettings.GetSourceDecimalCustomSeparator.Value = _Control.GetSourceDecimalCustomSeparator;
            _ControlSettings.GetTargetDecimalCustomSeparator.Value = _Control.GetTargetDecimalCustomSeparator;

        }

        #endregion

        public override void OnDeactivate()
        {
            _ControlSettings.AddedNumbersErrorType.Value = _Control.AddedNumbersErrorType;
            _ControlSettings.RemovedNumbersErrorType.Value = _Control.RemovedNumbersErrorType;
            _ControlSettings.ModifiedNumbersErrorType.Value = _Control.ModifiedNumbersErrorType;
            _ControlSettings.ModifiedAlphanumericsErrorType.Value = _Control.ModifiedAlphanumericsErrorType;
            _ControlSettings.ReportAddedNumbers.Value = _Control.ReportAddedNumbers;
            _ControlSettings.ReportRemovedNumbers.Value = _Control.ReportRemovedNumbers;
            _ControlSettings.ReportModifiedNumbers.Value = _Control.ReportModifiedNumbers;
            _ControlSettings.ReportModifiedAlphanumerics.Value = _Control.ReportModifiedAlphanumerics;
            _ControlSettings.ReportBriefMessages.Value = _Control.ReportBriefMessages;
            _ControlSettings.ReportExtendedMessages.Value = _Control.ReportExtendedMessages;
            _ControlSettings.ExcludeTagText.Value = _Control.ExcludeTagText;
            _ControlSettings.AllowLocalizations.Value = _Control.AllowLocalizations;
            _ControlSettings.PreventLocalizations.Value = _Control.PreventLocalizations;
            _ControlSettings.RequireLocalizations.Value = _Control.RequireLocalizations;
            _ControlSettings.SourceThousandsSpace.Value = _Control.SourceThousandsSpace;
            _ControlSettings.SourceThousandsNobreakSpace.Value = _Control.SourceThousandsNobreakSpace;
            _ControlSettings.SourceThousandsThinSpace.Value = _Control.SourceThousandsThinSpace;
            _ControlSettings.SourceThousandsNobreakThinSpace.Value = _Control.SourceThousandsNobreakThinSpace;
            _ControlSettings.SourceThousandsComma.Value = _Control.SourceThousandsComma;
            _ControlSettings.SourceThousandsPeriod.Value = _Control.SourceThousandsPeriod;
            _ControlSettings.SourceNoSeparator.Value = _Control.SourceNoSeparator;
            _ControlSettings.TargetThousandsSpace.Value = _Control.TargetThousandsSpace;
            _ControlSettings.TargetThousandsNobreakSpace.Value = _Control.TargetThousandsNobreakSpace;
            _ControlSettings.TargetThousandsThinSpace.Value = _Control.TargetThousandsThinSpace;
            _ControlSettings.TargetThousandsNobreakThinSpace.Value = _Control.TargetThousandsNobreakThinSpace;
            _ControlSettings.TargetThousandsComma.Value = _Control.TargetThousandsComma;
            _ControlSettings.TargetThousandsPeriod.Value = _Control.TargetThousandsPeriod;
            _ControlSettings.TargetNoSeparator.Value = _Control.TargetNoSeparator;
            _ControlSettings.SourceDecimalComma.Value = _Control.SourceDecimalComma;
            _ControlSettings.SourceDecimalPeriod.Value = _Control.SourceDecimalPeriod;
            _ControlSettings.TargetDecimalComma.Value = _Control.TargetDecimalComma;
            _ControlSettings.TargetDecimalPeriod.Value = _Control.TargetDecimalPeriod;
            _ControlSettings.ExcludeLockedSegments.Value = _Control.ExcludeLockedSegments;
            _ControlSettings.Exclude100Percents.Value = _Control.Exclude100Percents;
            _ControlSettings.ExcludeUntranslatedSegments.Value = _Control.ExcludeUntranslatedSegments;
            _ControlSettings.SourceThousandsCustomSeparator.Value = _Control.SourceThousandsCustomSeparator;
            _ControlSettings.TargetThousandsCustomSeparator.Value = _Control.TargetThousandsCustomSeparator;
            _ControlSettings.SourceDecimalCustomSeparator.Value = _Control.SourceDecimalCustomSeparator;
            _ControlSettings.TargetDecimalCustomSeparator.Value = _Control.TargetDecimalCustomSeparator;
            _ControlSettings.GetSourceThousandsCustomSeparator.Value = _Control.GetSourceThousandsCustomSeparator;
            _ControlSettings.GetTargetThousandsCustomSeparator.Value = _Control.GetTargetThousandsCustomSeparator;
            _ControlSettings.GetSourceDecimalCustomSeparator.Value = _Control.GetSourceDecimalCustomSeparator;
            _ControlSettings.GetTargetDecimalCustomSeparator.Value = _Control.GetTargetDecimalCustomSeparator;
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
