using Sdl.FileTypeSupport.Framework.NativeApi;

namespace Multilingual.XML.FileType.EmbeddedContent
{
	internal interface IPublishSubContent : ISubContentPublisher
	{
		void PublishContent(ProcessSubContentEventArgs eventArgs);
	}
}
