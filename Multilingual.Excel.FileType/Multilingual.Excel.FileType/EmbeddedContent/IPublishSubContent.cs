using Sdl.FileTypeSupport.Framework.NativeApi;

namespace Multilingual.Excel.FileType.EmbeddedContent
{
	internal interface IPublishSubContent : ISubContentPublisher
	{
		void PublishContent(ProcessSubContentEventArgs eventArgs);
	}
}
