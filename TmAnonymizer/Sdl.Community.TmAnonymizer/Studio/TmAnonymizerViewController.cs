using System;
using System.Windows.Forms;
using Sdl.Community.TmAnonymizer.Ui;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.TranslationStudioAutomation.IntegrationApi.Presentation.DefaultLocations;

namespace Sdl.Community.TmAnonymizer.Studio
{
	[View(
		Id = "TmAnonymizer",
		Name = "Tm Anonymizer",
		Description = "Anonymize personal information from tm",
		LocationByType = typeof(TranslationStudioDefaultViews.TradosStudioViewsLocation))]
	public class TmAnonymizerViewController: AbstractViewController
	{
		private static readonly Lazy<TmAnonymizerUserControl> Control = new Lazy<TmAnonymizerUserControl>(() => new TmAnonymizerUserControl());
		
		protected override void Initialize(IViewContext context)
		{
		}
		protected override Control GetContentControl()
		{
			return Control.Value;
		}
	}
}
