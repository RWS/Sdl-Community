using System;
using System.Net;

namespace Sdl.Community.DeeplAddon.Exceptions
{
    public class AddonValidationException : AddonException
    {
        public AddonValidationException(string message, Exception inner, params Details[] details) : base(message, inner)
        {
            ErrorCode = ErrorCodes.InvalidValues;
            StatusCode = HttpStatusCode.BadRequest;
            ExceptionDetails = details;
        }

        public AddonValidationException(string message, params Details[] details) : base(message)
        {
            ErrorCode = ErrorCodes.InvalidValues;
            StatusCode = HttpStatusCode.BadRequest;
            ExceptionDetails = details;
        }
    }
}
