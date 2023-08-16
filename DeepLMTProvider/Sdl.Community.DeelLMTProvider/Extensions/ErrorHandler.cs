using System;
using System.Runtime.CompilerServices;

namespace Sdl.Community.DeepLMTProvider.Extensions
{
	/// <summary>
	/// Class that doesn't use external resources (dbs, files, the internet) so it is static and its methods invoked anywhere since it won't impact unit testing and it is thread safe since it is stateless
	/// </summary>
	public static class ErrorHandler
	{
		/// <summary>
		/// Provides a static utility for wrapping functions in a try-catch block
		/// and returning the result, success status, and failure message.
		/// </summary>
		public static (bool Success, T Result, string FailureMessage) WrapTryCatch<T>(Func<T> function, [CallerMemberName] string failingMethod = null)
		{
			try
			{
				return (true, function(), null);
			}
			catch (Exception e)
			{
				return (false, default, GetFailureMessage(e.Message, failingMethod));
			}
		}

		public static string GetFailureMessage(string failureReason = null, [CallerMemberName] string failingMethod = null) =>
			$@"""{failingMethod}"" failed: {failureReason}";
	}
}