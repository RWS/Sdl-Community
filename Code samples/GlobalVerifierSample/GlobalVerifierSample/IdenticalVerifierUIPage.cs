using Sdl.Core.Settings;
using Sdl.Verification.Api;

namespace GlobalVerifierSample
{
	/// <summary>
	/// This is the extension class that displays and controls the plug-in user interface,
	/// in which the verification setting(s) can be specified. This class is responsible for
	/// e.g. saving the setting(s) configured in the UI, for resetting the values to their defaults,
	/// and for properly disposing of the UI control.
	/// </summary>
	[GlobalVerifierSettingsPage(
	Id = "Identical Settings Definition ID",
	Name = "Context to check",
	Description = "The display code of the context for which the length check should be performed.",
	HelpTopic = "")]
	internal class IdenticalVerifierUIPage : AbstractSettingsPage
	{
		private IdenticalVerifierUI _Control;
		private IdenticalVerifierSettings _ControlSettings;

		// Return the UI control.

		public override object GetControl()
		{
			_ControlSettings = ((ISettingsBundle)DataSource).GetSettingsGroup<IdenticalVerifierSettings>();
			_ControlSettings.BeginEdit();
			if (_Control == null)
			{
				_Control = new IdenticalVerifierUI();
			}

			return _Control;
		}

		// Load data from the settings into the UI control.

		public override void OnActivate()
		{
			_Control.ContextToCheck = _ControlSettings.CheckContext;
		}

		// Reset the values on the UI control.

		public override void ResetToDefaults()
		{
			_ControlSettings.CheckContext.Reset();
			_Control.ContextToCheck = _ControlSettings.CheckContext;
		}

		public override bool ValidateInput()
		{
			return _Control.ValidateChildren();
		}

		// Save the values from the UI into settings class.

		public override void Save()
		{
			_ControlSettings.CheckContext.Value = _Control.ContextToCheck;
		}

		// Call EndEdit after all changes have been saved in the Save() call.

		public override void AfterSave()
		{
			_ControlSettings.EndEdit();
		}

		// Cancel any pending changes.

		public override void Cancel()
		{
			_ControlSettings.CancelEdit();
		}

		// Properly dispose of the control.

		public override void Dispose()
		{
			_Control.Dispose();
		}
	}
}