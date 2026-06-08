using LanguageWeaverProvider.CohereSubscription.Workflow.Model;
using System.Threading.Tasks;

namespace LanguageWeaverProvider.CohereSubscription.Workflow.Interfaces
{
    public interface ICohereSubscriptionWorkflowService
    {
        Task<CohereSubscriptionData> ExecuteAsync();
    }
}
