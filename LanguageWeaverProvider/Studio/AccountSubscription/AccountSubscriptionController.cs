using LanguageWeaverProvider.View;
using LanguageWeaverProvider.ViewModel;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.Desktop.IntegrationApi.Interfaces;
using Sdl.TranslationStudioAutomation.IntegrationApi;

namespace LanguageWeaverProvider.Studio.AccountSubscription
{
	[ViewPart(Id = "Language Weaver - Subscription",
			  Name = "Language Weaver - Subscription",
			  Description = "See informations about your active subscription.",
			  Icon = "lwLogoIco")]
	[ViewPartLayout(typeof(EditorController), Dock = DockType.Left)]
	public class AccountSubscriptionController : AbstractViewPartController
	{
		private AccountSubscriptionView _feedbackView;

		protected override void Initialize()
		{
			var viewModel = new AccountSubscriptionViewModel();
			var view = new AccountSubscriptionView() { DataContext = viewModel };
			_feedbackView = view;
		}

		protected override IUIControl GetContentControl()
		{
			 return _feedbackView;
		}
	}
}
