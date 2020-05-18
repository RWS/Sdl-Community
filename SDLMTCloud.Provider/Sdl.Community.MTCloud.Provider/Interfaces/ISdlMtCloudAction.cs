namespace Sdl.Community.MTCloud.Provider.Interfaces
{
	public interface ISDLMTCloudAction
	{
		string Id { get; set; }

		void LoadTooltip(string tooltipText);
	}
}
