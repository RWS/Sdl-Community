namespace Sdl.Community.MTCloud.Provider.Model
{
	public class ImprovedTarget
	{
		public ImprovedTarget(string originalTarget)
		{
			OriginalTarget = originalTarget;
		}

		public string Improvement { get; set; }
		public string OriginalTarget { get; }
	}
}