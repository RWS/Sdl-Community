using System.Diagnostics.Contracts;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using SDLCommunityCleanUpTasks.Contracts;

namespace SDLCommunityCleanUpTasks
{
	[ContractClass(typeof(ISegmentHandlerContract))]
    public interface ISegmentHandler : IMarkupDataVisitor
    {
    }
}