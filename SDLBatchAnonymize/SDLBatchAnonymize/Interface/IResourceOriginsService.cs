using Sdl.FileTypeSupport.Framework.BilingualApi;

namespace Sdl.Community.SDLBatchAnonymize.Interface
{
	public interface IResourceOriginsService
	{
		/// <summary>
		/// Anonymize MT/NMT markers for segment based on settings
		/// </summary>
		/// <param name="segmentPair">Segment Pair</param>
		/// <param name="anonymizerSettings">Anonymization settings</param>
		void RemoveMt(ISegmentPair segmentPair, IBatchAnonymizerSettings anonymizerSettings);

		void RemoveQe(ISegmentPair segmentPair);

		/// <summary>
		/// Anonymize TM markers for segment based on settings
		/// </summary>
		/// <param name="segmentPair">Segment Pair</param>
		/// <param name="anonymizerSettings">Anonymization settings</param>
		void RemoveTm(ISegmentPair segmentPair, IBatchAnonymizerSettings anonymizerSettings);
	}
}