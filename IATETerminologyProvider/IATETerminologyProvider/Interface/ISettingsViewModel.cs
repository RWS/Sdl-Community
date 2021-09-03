using Sdl.Community.IATETerminologyProvider.Model;

namespace Sdl.Community.IATETerminologyProvider.Interface
{
	public interface ISettingsViewModel
	{
		SettingsModel Settings{ get; set; }
		void Setup();
		void Reset();
	}
}