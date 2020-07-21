using System;
using System.Threading.Tasks;
using Sdl.Community.MTCloud.Provider.Interfaces;

namespace Sdl.Community.MTCloud.Provider.Helpers
{
	public static class TaskUtilities
	{
		// Asynchronous methods should return a Task instead of void
		public static async void FireAndForgetSafeAsync(this Task task, IErrorHandler handler = null)
		{
			try
			{
				await task;
			}
			catch (Exception ex)
			{
				handler?.HandleError(ex);
			}
		}
	}
}
