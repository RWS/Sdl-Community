using LanguageWeaverProvider.Infrastructure.Http.Interfaces;
using System.Collections.Generic;

namespace LanguageWeaverProvider.Infrastructure.Http.Model
{
    public class GenericHTTPRespose<T> : IGenericHTTPResponse<T>
    {
        public bool Success { get; set; }

        public T Response { get; set; } = default(T);

        public IEnumerable<string> Errors { get; set; } = new List<string>();
    }
}
