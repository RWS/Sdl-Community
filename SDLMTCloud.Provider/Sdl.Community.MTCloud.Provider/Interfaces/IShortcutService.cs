namespace Sdl.Community.MTCloud.Provider.Interfaces
{
	public delegate void ShortcutChangedEventRaiser();
	public interface IShortcutService
	{
		string GetShotcutDetails(string actionId);
		event ShortcutChangedEventRaiser StudioShortcutChanged;
	}
}
