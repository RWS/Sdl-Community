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
    Id = "Number Verifier Help Definition ID",
    Name = "Number Verifier Help",
    Description = "The help for the number verifier.",
    HelpTopic = "")]
    #endregion
    class NumberVerifierHelpPage : AbstractSettingsPage
    {
        #region "PrivateMembers"
        private NumberVerifierHelp _Control;
        #endregion

        #region "control"
        // Return the UI control.
        #region "GetControl"
        public override object GetControl()
        {
            if (_Control == null)
            {
                _Control = new NumberVerifierHelp();
            }

            return _Control;
        }
        #endregion
        

        // Load data from the settings into the UI control.
        #region "OnActivate"
        public override void OnActivate()
        {

        }
        #endregion



        // Reset the values on the UI control.
        #region "ResetToDefaults"
        public override void ResetToDefaults()
        {

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

        }
        #endregion


        // Call EndEdit after all changes have been saved in the Save() call.
        #region "AfterSave"
        public override void AfterSave()
        {

        }
        #endregion


        // Cancel any pending changes.
        #region "Cancel"
        public override void Cancel()
        {

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
