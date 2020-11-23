using System;
using Sdl.Community.MTCloud.Provider.Interfaces;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.Desktop.IntegrationApi.Interfaces;
using Sdl.TranslationStudioAutomation.IntegrationApi;

namespace Sdl.Community.MTCloud.Provider.Studio
{
	[ViewPart(
		Id = "SDL MT Cloud Rate Translations",
		Name = "SDL MT Cloud Rate Translations",
		Description = "SDL MT Cloud Rate Translations",
		Icon = "rating")]
	[ViewPartLayout(typeof(EditorController), Dock = DockType.Bottom)]
	public class RateItController : AbstractViewPartController
	{
		private Lazy<View.RateItControl> _control;

		public IRatingService RateIt => _control?.Value.RatingService;

		public void FocusFeedbackTextBox()
		{
			_control.Value.FocusFeedbackTextBox();
		}

		protected override IUIControl GetContentControl()
		{
			return _control.Value;
		}

		protected override void Initialize()
		{
			_control = new Lazy<View.RateItControl>(() => new View.RateItControl());
		}
	}
}