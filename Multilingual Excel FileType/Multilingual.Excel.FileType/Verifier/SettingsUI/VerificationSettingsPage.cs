using Multilingual.Excel.FileType.Constants;
using Multilingual.Excel.FileType.FileType.Settings;
using Multilingual.Excel.FileType.Verifier.Settings;
using Sdl.Core.Settings;
using Sdl.Verification.Api;

namespace Multilingual.Excel.FileType.Verifier.SettingsUI
{
	[GlobalVerifierSettingsPage(
		Id = SettingsConstants.MultilingualExcelVerificationSettingsId,
		Name = "VerificationSettings_Name",
		Description = "VerificationSettings_Description",
		HelpTopic = "")]
	public class VerificationSettingsPage : AbstractSettingsPage
	{
		private VerifcationSettings _control;
		private MultilingualExcelVerificationSettings _controlSettings;
		private LanguageMappingSettings _languageMappingSettings;

		public override object GetControl()
		{
			_controlSettings = ((ISettingsBundle)DataSource).GetSettingsGroup<MultilingualExcelVerificationSettings>();
			_controlSettings.BeginEdit();

			return _control ?? (_control = new VerifcationSettings());
		}

		public override void OnActivate()
		{
			ApplySettings();
		}

		public override void ResetToDefaults()
		{
			_controlSettings.Reset();

			ApplySettings();
		}

		internal LanguageMappingSettings LanguageMappingSettings
		{
			get
			{
				var settingsBundle = DataSource as ISettingsBundle;

				if (_languageMappingSettings != null || settingsBundle == null)
				{
					return _languageMappingSettings;
				}


				_languageMappingSettings = new LanguageMappingSettings();
				_languageMappingSettings.PopulateFromSettingsBundle(settingsBundle, FiletypeConstants.FileTypeDefinitionId);

				return _languageMappingSettings;
			}
		}

		private void ApplySettings()
		{
			//var projectsController = SdlTradosStudio.Application.GetController<ProjectsController>();
			//var project = projectsController.CurrentProject ?? projectsController.SelectedProjects.FirstOrDefault();
			//if (project != null)
			//{
			//	var charLimitationColumnDefined = _languageMappingSettings.LanguageMappingLanguages.Any(a=>!string.IsNullOrEmpty(a.CharacterLimitationColumn));
			//	var pixelLimitationColumnDefined = _languageMappingSettings.LanguageMappingLanguages.Any(a => !string.IsNullOrEmpty(a.PixelLimitationColumn));
			//}
			
			_control.MaxCharacterLengthEnabled = _controlSettings.MaxCharacterLengthEnabled.Value;
			_control.MaxCharacterLengthSeverity = _controlSettings.MaxCharacterLengthSeverity.Value;

			_control.MaxPixelLengthEnabled = _controlSettings.MaxPixelLengthEnabled.Value;
			_control.MaxPixelLengthSeverity = _controlSettings.MaxPixelLengthSeverity.Value;

			_control.MaxLinesPerParagraphEnabled = _controlSettings.MaxLinesPerParagraphEnabled.Value;
			_control.MaxLinesPerParagraphSeverity = _controlSettings.MaxLinesPerParagraphSeverity.Value;

			_control.VerifySourceParagraphsEnabled = _controlSettings.VerifySourceParagraphsEnabled.Value;
			_control.VerifyTargetParagraphsEnabled = _controlSettings.VerifyTargetParagraphsEnabled.Value;
		}

		public override bool ValidateInput()
		{
			return _control.ValidateChildren();
		}

		public override void Save()
		{
			_controlSettings.MaxCharacterLengthEnabled.Value = _control.MaxCharacterLengthEnabled;
			_controlSettings.MaxCharacterLengthSeverity.Value = _control.MaxCharacterLengthSeverity;

			_controlSettings.MaxPixelLengthEnabled.Value = _control.MaxPixelLengthEnabled;
			_controlSettings.MaxPixelLengthSeverity.Value = _control.MaxPixelLengthSeverity;

			_controlSettings.MaxLinesPerParagraphEnabled.Value = _control.MaxLinesPerParagraphEnabled;
			_controlSettings.MaxLinesPerParagraphSeverity.Value = _control.MaxLinesPerParagraphSeverity;

			_controlSettings.VerifySourceParagraphsEnabled.Value  = _control.VerifySourceParagraphsEnabled;
			_controlSettings.VerifyTargetParagraphsEnabled.Value = _control.VerifyTargetParagraphsEnabled;

			MultilingualExcelVerifierSettings.Instance.OnSavedSettings();
		}

		public override void AfterSave()
		{
			_controlSettings.EndEdit();
		}

		public override void Cancel()
		{
			_controlSettings.CancelEdit();
		}

		public override void Dispose()
		{
			_control.Dispose();
		}
	}
}
