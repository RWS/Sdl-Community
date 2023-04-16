using Sdl.Community.MTCloud.Provider.Model.RateIt;
using Sdl.TranslationStudioAutomation.IntegrationApi;

namespace Sdl.Community.MTCloud.Provider.Service.Interface
{
	public interface ISegmentMetadataCreator
	{
		void StoreMetadata(TranslationData translationData);

		void AddToCurrentSegmentContextData(IStudioDocument activeDocument, TranslationOriginDatum translationOriginDatum);

		void AddStoredMetadataToProjectFile();
	}
}