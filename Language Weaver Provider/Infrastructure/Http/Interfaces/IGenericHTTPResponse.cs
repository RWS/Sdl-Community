using System.Collections.Generic;

namespace LanguageWeaverProvider.Infrastructure.Http.Interfaces
{
    public interface IGenericHTTPResponse<T>
    {
        public bool Success { get; }

        T Response { get; }

        public IEnumerable<string> Errors { get; }
    }
}
