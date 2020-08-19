using Sdl.FileTypeSupport.Framework.NativeApi;

namespace Sdl.Community.MTCloud.Provider.Model
{
	public class ImprovedTarget
	{
		public ImprovedTarget(string originalTarget)
		{
			if (string.IsNullOrWhiteSpace(originalTarget)) return;
			OriginalTarget = originalTarget;
		}

		public string Improvement { get; set; }
		public string OriginalTarget { get; }
		public ITranslationOrigin OriginalTargetOrigin { get; set; }
	}
}