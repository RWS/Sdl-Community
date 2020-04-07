using Sdl.FileTypeSupport.Framework.BilingualApi;

namespace Sdl.Community.SDLBatchAnonymize.Interface
{
	public interface IResourceOriginsService
	{
		void RemoveMt(ISegmentPair segmentPair, IBatchAnonymizerSettings anonymizerSettings);
		void RemoveTm(ISegmentPair segmentPair, IBatchAnonymizerSettings anonymizerSettings);
	}
}
