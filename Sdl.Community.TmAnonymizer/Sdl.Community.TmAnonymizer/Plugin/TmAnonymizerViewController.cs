using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Sdl.Community.TmAnonymizer.Ui;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.TranslationStudioAutomation.IntegrationApi;
using Sdl.TranslationStudioAutomation.IntegrationApi.Presentation.DefaultLocations;

namespace Sdl.Community.TmAnonymizer.Plugin
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
