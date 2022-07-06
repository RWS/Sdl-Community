namespace Sdl.Community.MTCloud.Provider.Interfaces
{
	public delegate void ShortcutChangedEventRaiser();

	public interface IShortcutService
	{
		event ShortcutChangedEventRaiser StudioShortcutChanged;

		string GetShortcutDetails(string actionId);
	}
}