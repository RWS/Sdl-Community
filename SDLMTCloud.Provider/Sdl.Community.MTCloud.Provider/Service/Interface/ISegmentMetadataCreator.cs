using Sdl.Community.MTCloud.Provider.Model;
using Sdl.TranslationStudioAutomation.IntegrationApi;

namespace Sdl.Community.MTCloud.Provider.Service.Interface
{
	public interface ISegmentMetadataCreator
	{
		void AddTargetSegmentMetaData(TranslationData translationData);
		void AddToCurrentSegmentContextData(Document activeDocument, TranslationOriginInformation translationOriginInformation);
		void AddToSegmentContextData();
	}
}