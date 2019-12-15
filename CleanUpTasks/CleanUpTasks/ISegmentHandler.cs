using System.Diagnostics.Contracts;
using Sdl.Community.CleanUpTasks.Contracts;
using Sdl.FileTypeSupport.Framework.BilingualApi;

namespace Sdl.Community.CleanUpTasks
{
	[ContractClass(typeof(ISegmentHandlerContract))]
	public interface ISegmentHandler : IMarkupDataVisitor
	{
	}
}