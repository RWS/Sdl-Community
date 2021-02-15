using System.Collections.Generic;
using Sdl.Community.MTCloud.Provider.Model;

namespace Sdl.Community.MTCloud.Provider.Service.Events
{
	public delegate void TranslationReceivedEventHandler(List<string> sourceSegment, TranslationData targetSegmentData);
}