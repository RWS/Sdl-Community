using Sdl.Community.MTCloud.Provider.Model.RateIt;
using Sdl.TranslationStudioAutomation.IntegrationApi;

namespace Sdl.Community.MTCloud.Provider.Service.Interface
{
	public interface ISegmentMetadataCreator
	{
		void AddStoredMetadataToProjectFile();

		void AddToCurrentSegmentContextData(IStudioDocument activeDocument, TranslationOriginDatum translationOriginDatum);

		void StoreMetadata(TranslationData translationData);
	}
}