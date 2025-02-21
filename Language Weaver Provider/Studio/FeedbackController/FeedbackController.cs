﻿using LanguageWeaverProvider.Studio.FeedbackController.View;
using LanguageWeaverProvider.Studio.FeedbackController.ViewModel;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.Desktop.IntegrationApi.Interfaces;
using Sdl.TranslationStudioAutomation.IntegrationApi;

namespace LanguageWeaverProvider.Studio.FeedbackController
{
	[ViewPart(Id = "Language Weaver - Feedback",
			  Name = "Language Weaver - Feedback",
			  Description = "Send feedback for the translated content using Language Weaver.",
			  Icon = "lwLogo_Main_Icon")]
	[ViewPartLayout(typeof(EditorController), Dock = DockType.Left)]
	public class FeedbackController : AbstractViewPartController
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
