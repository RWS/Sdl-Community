using Sdl.TellMe.ProviderApi;
using Sdl.TranslationStudioAutomation.IntegrationApi;
using System.Drawing;

namespace Sdl.Community.InvoiceAndQuotes.Integration.TellMe.Actions
{
    public class InQuoteAction : AbstractTellMeAction
    {
        public InQuoteAction() => Name = $"{PluginResources.Plugin_Name}";

        public override string Category => string.Format(PluginResources.TellMe_Provider_Results, PluginResources.Plugin_Name);
        public override Icon Icon => PluginResources.Invoice;
        public override bool IsAvailable => true;

        public override void Execute() => SdlTradosStudio.Application.ExecuteAction<InvoiceAndQuotesViewPart>();
    }
}