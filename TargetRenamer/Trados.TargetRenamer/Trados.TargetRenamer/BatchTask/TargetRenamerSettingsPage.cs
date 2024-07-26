using Sdl.Core.Settings;
using Sdl.Desktop.IntegrationApi;
using Trados.TargetRenamer.View;

namespace Trados.TargetRenamer.BatchTask
{
	public class TargetRenamerSettingsPage : DefaultSettingsPage<TargetRenamerSettingsView, TargetRenamerSettings>
    {
        private TargetRenamerSettingsView _control;
        private TargetRenamerSettings _settings;

        public override object GetControl()
        {
            _settings = ((ISettingsBundle)DataSource).GetSettingsGroup<TargetRenamerSettings>();
            _control = base.GetControl() as TargetRenamerSettingsView;
            if (!(_control is null))
            {
                _control.TargetRenamerSettingsViewModel.Settings = _settings;
            }

            return _control;
        }

        public override void Save()
        {
            base.Save();
            if (_settings is null) return;
            _settings.AppendAsPrefix = _control.TargetRenamerSettingsViewModel.AppendAsPrefix;
            _settings.AppendAsSuffix = _control.TargetRenamerSettingsViewModel.AppendAsSuffix;
            _settings.UseRegularExpression = _control.TargetRenamerSettingsViewModel.UseRegularExpression;
            _settings.UseCustomLocation = _control.TargetRenamerSettingsViewModel.UseCustomLocation;
            _settings.CustomLocation = _control.TargetRenamerSettingsViewModel.CustomLocation;
            _settings.RegularExpressionSearchFor = _control.TargetRenamerSettingsViewModel.RegularExpressionSearchFor;
            _settings.RegularExpressionReplaceWith =
                _control.TargetRenamerSettingsViewModel.RegularExpressionReplaceWith;
            _settings.Delimiter = _control.TargetRenamerSettingsViewModel.Delimiter;
            _settings.UseShortLocales = _control.TargetRenamerSettingsViewModel.UseShortLocales;
            _settings.AppendTargetLanguage = _control.TargetRenamerSettingsViewModel.AppendTargetLanguage;
            _settings.AppendCustomString = _control.TargetRenamerSettingsViewModel.AppendCustomString;
            _settings.CustomString = _control.TargetRenamerSettingsViewModel.CustomString;
        }
    }
}