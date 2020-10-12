using System.Collections.Generic;

namespace Sdl.Community.MTCloud.Provider.Service.Events
{
	public delegate void TranslationReceivedEventHandler(List<string> sourceSegment, List<string> targetSegment);
}