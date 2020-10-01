using System.Text.Json.Serialization;

namespace Sdl.LC.AddonBlueprint.Enums
{
	public enum AddOnLifecycleEventEnum
	{
		/// <summary>
		/// Add-On was registered in LanguageCloud.
		/// </summary>
		REGISTERED,

		/// <summary>
		/// Add-On was unregistered in LanguageCloud.
		/// </summary>
		UNREGISTERED,

		/// <summary>
		/// Add-On was activated on a tenant account.
		/// </summary>
		ACTIVATED
	}
}
