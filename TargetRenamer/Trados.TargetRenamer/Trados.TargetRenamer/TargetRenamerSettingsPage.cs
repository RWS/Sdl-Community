using Sdl.Desktop.IntegrationApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sdl.Core.Settings;
using Trados.TargetRenamer.Control;
using Trados.TargetRenamer.ViewModel;

namespace Trados.TargetRenamer
{
    public class TargetRenamerSettingsPage : DefaultSettingsPage<TargetRenamerSettingsControl, TargetRenamerSettings>
    {
	    private TargetRenamerSettingsControl _control;
	    private TargetRenamerSettings _settings;

	    public override object GetControl()
	    {
		    _settings = ((ISettingsBundle) DataSource).GetSettingsGroup<TargetRenamerSettings>();
			_control = base.GetControl() as TargetRenamerSettingsControl;
			if (!(_control is null))
			{
				_control.TargetRenamerSettingsViewModel.Settings = _settings;
			}

			return _control;
	    }

	    public override void Save()
		{
			base.Save();
			if(_settings is null) return;
			
		}
    }
}
