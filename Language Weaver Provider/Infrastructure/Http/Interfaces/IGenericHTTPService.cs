using System.Threading.Tasks;

namespace LanguageWeaverProvider.Infrastructure.Http.Interfaces
{
    public interface IGenericHTTPService<TRequest, TResponse>
    {
        Task<IGenericHTTPResponse<TResponse>> SendRequest(TRequest request);
    }
}
