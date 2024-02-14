using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Runtime.Serialization;

namespace TMProvider
{
    [Serializable]
    public class ServerException : Exception
    {
        public ServerException() : base() { }
        public ServerException(string message) : base(message) { }
        public ServerException(string message, Exception innerException) : base(message, innerException) { }
        protected ServerException(SerializationInfo serInfo, StreamingContext context) : base(serInfo, context) { }
    }


    [Serializable]
    public class TokenTimedOutException : ServerException
    {
        public TokenTimedOutException() : base() { }
        public TokenTimedOutException(string message) : base(message) { }
        public TokenTimedOutException(string message, Exception innerException) : base(message, innerException) { }
        protected TokenTimedOutException(SerializationInfo serInfo, StreamingContext context) : base(serInfo, context) { }
    }

    [Serializable]
    public class AuthenticationException : ServerException
    {
        public AuthenticationException() : base() { }
        public AuthenticationException(string message) : base(message) { }
        public AuthenticationException(string message, Exception innerException) : base(message, innerException) { }
        protected AuthenticationException(SerializationInfo serInfo, StreamingContext context) : base(serInfo, context) { }
    }

    [Serializable]
    public class InvalidCertificateException : ServerException
    {
        public InvalidCertificateException() : base() { }
        public InvalidCertificateException(string message) : base(message) { }
        public InvalidCertificateException(string message, Exception innerException) : base(message, innerException) { }
        protected InvalidCertificateException(SerializationInfo serInfo, StreamingContext context) : base(serInfo, context) { }
    }

    [Serializable]
    public class ExpiredCertificateException : ServerException
    {
        public ExpiredCertificateException() : base() { }
        public ExpiredCertificateException(string message) : base(message) { }
        public ExpiredCertificateException(string message, Exception innerException) : base(message, innerException) { }
        protected ExpiredCertificateException(SerializationInfo serInfo, StreamingContext context) : base(serInfo, context) { }
    }

    [Serializable]
    public class NoCertificateException : ServerException
    {
        public NoCertificateException() : base() { }
        public NoCertificateException(string message) : base(message) { }
        public NoCertificateException(string message, Exception innerException) : base(message, innerException) { }
        protected NoCertificateException(SerializationInfo serInfo, StreamingContext context) : base(serInfo, context) { }
    }

    [Serializable]
    public class NameMismatchCertificateException : ServerException
    {
        public NameMismatchCertificateException() : base() { }
        public NameMismatchCertificateException(string message) : base(message) { }
        public NameMismatchCertificateException(string message, Exception innerException) : base(message, innerException) { }
        protected NameMismatchCertificateException(SerializationInfo serInfo, StreamingContext context) : base(serInfo, context) { }
    }

    [Serializable]
    public class TooFrequentLoginException : ServerException
    {
        public TooFrequentLoginException() : base() { }
        public TooFrequentLoginException(string message) : base(message) { }
        public TooFrequentLoginException(string message, Exception innerException) : base(message, innerException) { }
        protected TooFrequentLoginException(SerializationInfo serInfo, StreamingContext context) : base(serInfo, context) { }
    }

    [Serializable]
    public class NoLicenseException : ServerException
    {
        public NoLicenseException() : base() { }
        public NoLicenseException(string message) : base(message) { }
        public NoLicenseException(string message, Exception innerException) : base(message, innerException) { }
        protected NoLicenseException(SerializationInfo serInfo, StreamingContext context) : base(serInfo, context) { }
    }

    [Serializable]
    public class ReverseLookupException : ServerException
    {
        public ReverseLookupException() : base() { }
        public ReverseLookupException(string message) : base(message) { }
        public ReverseLookupException(string message, Exception innerException) : base(message, innerException) { }
        protected ReverseLookupException(SerializationInfo serInfo, StreamingContext context) : base(serInfo, context) { }
    }

    [Serializable]
    public class TMNotFoundException : ServerException
    {
        public TMNotFoundException() : base() { }
        public TMNotFoundException(string message) : base(message) { }
        public TMNotFoundException(string message, Exception innerException) : base(message, innerException) { }
        protected TMNotFoundException(SerializationInfo serInfo, StreamingContext context) : base(serInfo, context) { }
    }

    [Serializable]
    public class UnauthorizedTMReadException : ServerException
    {
        public UnauthorizedTMReadException() : base() { }
        public UnauthorizedTMReadException(string message) : base(message) { }
        public UnauthorizedTMReadException(string message, Exception innerException) : base(message, innerException) { }
        protected UnauthorizedTMReadException(SerializationInfo serInfo, StreamingContext context) : base(serInfo, context) { }
    }

    [Serializable]
    public class UnauthorizedTMWriteException : ServerException
    {
        public UnauthorizedTMWriteException() : base() { }
        public UnauthorizedTMWriteException(string message) : base(message) { }
        public UnauthorizedTMWriteException(string message, Exception innerException) : base(message, innerException) { }
        protected UnauthorizedTMWriteException(SerializationInfo serInfo, StreamingContext context) : base(serInfo, context) { }
    }

    [Serializable]
    public class GeneralServerException : ServerException
    {
        public GeneralServerException() : base() { }
        public GeneralServerException(string message) : base(message) { }
        public GeneralServerException(string message, Exception innerException) : base(message, innerException) { }
        protected GeneralServerException(SerializationInfo serInfo, StreamingContext context) : base(serInfo, context) { }
    }
}
