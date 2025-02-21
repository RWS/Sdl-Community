using Sdl.Community.InvoiceAndQuotes.Integration.TellMe.Actions;
using Sdl.TellMe.ProviderApi;

namespace Sdl.Community.InvoiceAndQuotes.Integration.TellMe
{
	[TellMeProvider]
	public class TellMeProvider: ITellMeProvider
	{
		public string Name => string.Format(PluginResources.TellMe_Provider, PluginResources.Plugin_Name);

		public AbstractTellMeAction[] ProviderActions => new AbstractTellMeAction[]
		{
			new DocumentationAction
			{
				Keywords = new[] {"invoice", "quote", "inquote", "invoiceandquote", "community", "support", "wiki" }
			},
			new AppStoreForumAction
			{
				Keywords = new[] { "invoice", "quote", "inquote", "invoiceandquote", "support", "forum" }
			},
			new SourceCodeAction
			{
				Keywords = new[] { "invoice", "quote", "inquote", "invoiceandquote", "source code" }
			},
			new InQuoteAction
			{
				Keywords = new[] { "invoice", "quote", "inquote", "invoiceandquote" }
			}
		};
	}
}
