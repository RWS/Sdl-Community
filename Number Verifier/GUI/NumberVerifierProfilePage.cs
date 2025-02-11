using Sdl.Core.Settings;
using Sdl.Verification.Api;

namespace Sdl.Community.NumberVerifier.GUI
{
    #region "Declaration"
    [GlobalVerifierSettingsPage(
    Id = "Number Verifier Profiles ID",
    Name = "Number Verifier Profiles",
    Description = "The profiles for the number verifier.",
    HelpTopic = "")]
    #endregion
    internal class NumberVerifierProfilePage : AbstractSettingsPage
    {
        #region "PrivateMembers"
        private NumberVerifierProfile _Control;
        private NumberVerifierSettings _ControlSettings;

        #endregion

        public override object GetControl()
        {
            _ControlSettings = ((ISettingsBundle)DataSource).GetSettingsGroup<NumberVerifierSettings>();
            _ControlSettings.BeginEdit();

            _Control ??= new NumberVerifierProfile(new Services.FilePathDialogService(), _ControlSettings);

            return _Control;
        }

        public override void OnActivate()
        {
            ApplicationInitializer.IsNumberVerifierView = false;
        }

        public override void ResetToDefaults()
        {
        }

        public override bool ValidateInput()
        {
            return _Control.ValidateChildren();
        }

        public override void Save()
        {
        }

        public override void AfterSave()
        {
            _ControlSettings.EndEdit();
        }

        public override void Cancel()
        {
            _ControlSettings.CancelEdit();
        }

        public override void Dispose()
        {
            _Control.Dispose();
        }
    }
}
