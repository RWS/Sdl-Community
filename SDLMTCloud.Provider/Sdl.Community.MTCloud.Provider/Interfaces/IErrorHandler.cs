using System;

namespace Sdl.Community.MTCloud.Provider.Interfaces
{
	public interface IErrorHandler
	{
		void HandleError(Exception ex);
	}
}