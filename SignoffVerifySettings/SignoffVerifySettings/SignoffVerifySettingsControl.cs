using System.Windows.Forms;
using Sdl.Desktop.IntegrationApi;

namespace Sdl.Community.SignoffVerifySettings
{
	class SignoffVerifySettingsControl : UserControl, ISettingsAware<SignoffVerifySettings>
	{
		public SignoffVerifySettings Settings { get; set; }		
	}
}