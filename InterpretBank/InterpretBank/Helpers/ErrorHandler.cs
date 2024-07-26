using System;
using System.Runtime.CompilerServices;

namespace InterpretBank.Helpers
{
    public static class ErrorHandler
    {
        public static string GetFailureMessage(string failureReason = null, [CallerMemberName] string failingMethod = null) =>
                    $@"""{failingMethod}"" failed: {failureReason}";

        /// <summary>
        /// Provides a static utility for wrapping functions in a try-catch block
        /// and returning the result, success status, and failure message.
        /// </summary>
        public static ActionResult<T> WrapTryCatch<T>(Func<T> function, [CallerMemberName] string failingMethod = null)
        {
            try
            {
                return new(true, function(), null);
            }
            catch (Exception e)
            {
                return new(false, default, GetFailureMessage(e.Message, failingMethod));
            }
        }
    }
}