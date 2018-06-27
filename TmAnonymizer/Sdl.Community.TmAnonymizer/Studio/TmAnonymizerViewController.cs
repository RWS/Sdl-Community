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

		[RibbonGroup("TmRibbonGroup", "Tm Anonymizer user guide")]
		[RibbonGroupLayout(LocationByType = typeof(TranslationStudioDefaultRibbonTabs.HomeRibbonTabLocation))]
		public class TmAnonymizerRibbonGroup : AbstractRibbonGroup
		{

		}

		[Action("Sdl.Community.TmAnonymizer", typeof(TmAnonymizerViewController), Name = "Help", Icon = "wiki", Description = "An wiki page will be opened in browser with user documentation")]
		[ActionLayout(typeof(TmAnonymizerRibbonGroup), 250, DisplayType.Large)]
		public class TmAnonymizerHelpAction : AbstractAction
		{
			protected override void Execute()
			{
				System.Diagnostics.Process.Start("https://community.sdl.com/product-groups/translationproductivity/w/customer-experience/3272.sdl-tm-anonymizer");

			}
		}
	}
	
	
}
