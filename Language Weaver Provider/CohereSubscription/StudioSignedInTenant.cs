using LanguageWeaverProvider.Model.Interface;
using LanguageWeaverProvider.Services;

namespace LanguageWeaverProvider.CohereSubscription
{
    public class StudioSignedInTenant : ISignedInTenant
    {
        public string GetActiveTenantId() => StudioIdentityService.GetActiveTenantId();
    }
}
