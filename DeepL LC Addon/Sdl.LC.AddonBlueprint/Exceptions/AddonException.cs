using System;
using System.Net;

namespace Sdl.LC.AddonBlueprint.Exceptions
{
    public class AddonException : Exception
    {
        public string ErrorCode { get; set; }
        public HttpStatusCode StatusCode { get; set; }
        public Details[] ExceptionDetails { get; set; }

        public AddonException()
        {
            ErrorCode = ErrorCodes.InternalServerError;
            StatusCode = HttpStatusCode.InternalServerError;
        }

        public AddonException(string message)
            : base(message)
        {
            ErrorCode = ErrorCodes.InternalServerError;
            StatusCode = HttpStatusCode.InternalServerError;
        }

        public AddonException(string message, Exception inner)
            : base(message, inner)
        {
            ErrorCode = ErrorCodes.InternalServerError;
            StatusCode = HttpStatusCode.InternalServerError;
        }

        public AddonException(HttpStatusCode statusCode, string errorCode, string message)
            : base(message)
        {
            ErrorCode = errorCode;
            StatusCode = statusCode;
        }

        public AddonException(HttpStatusCode statusCode, string errorCode, string message, Details[] details)
            : base(message)
        {
            ErrorCode = errorCode;
            StatusCode = statusCode;
            ExceptionDetails = details;
        }

        public AddonException(HttpStatusCode statusCode, string errorCode, string message, Exception inner)
            : base(message, inner)
        {
            ErrorCode = errorCode;
            StatusCode = statusCode;
        }

        public AddonException(HttpStatusCode statusCode, string errorCode, string message, Details[] details, Exception inner)
            : base(message, inner)
        {
            ErrorCode = errorCode;
            StatusCode = statusCode;
            ExceptionDetails = details;
        }
    }
}
