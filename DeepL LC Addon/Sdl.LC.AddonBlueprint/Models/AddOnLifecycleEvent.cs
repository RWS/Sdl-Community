using Sdl.Community.DeeplAddon.Enums;

namespace Sdl.Community.DeeplAddon.Models
{
	public class AddOnLifecycleEvent<T> : AddOnLifecycleEvent where T : class
	{
		/// <summary>
		/// The data object.
		/// </summary>
		public T Data { get; set; }
	}

	public class AddOnLifecycleEvent
	{
		/// <summary>
		/// The add-on lifecycle event id.
		/// </summary>
		public AddOnLifecycleEventEnum Id { get; set; }

		/// <summary>
		/// The timestamp.
		/// </summary>
		public string Timestamp { get; set; }
	}
}
