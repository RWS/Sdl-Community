using LanguageWeaverProvider.View;
using LanguageWeaverProvider.ViewModel;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.Desktop.IntegrationApi.Interfaces;
using Sdl.TranslationStudioAutomation.IntegrationApi;

namespace LanguageWeaverProvider.Studio.FeedbackController
{
	[ViewPart(
		Id = "Language Weaver - Feedback",
		Name = "Language Weaver - Feedback",
		Description = "Send feedback for the translated content using Language Weaver with QE enabled.",
		Icon = "lwLogoIco"
	)]

	[ViewPartLayout(typeof(EditorController), Dock = DockType.Left)]
	public class LanguageWeaverEditorController : AbstractViewPartController
	{
		private FeedbackView _feedbackView;

		protected override void Initialize()
		{
			var viewModel = new FeedbackViewModel();
			var view = new FeedbackView() { DataContext = viewModel };
			_feedbackView = view;
		}

		protected override IUIControl GetContentControl()
		{
			return _feedbackView;
		}
	}
}
