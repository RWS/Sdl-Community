using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.TranslationStudioAutomation.IntegrationApi.Presentation.DefaultLocations;

namespace MtComparisonTool.Studio
{
	[Action("MT Comparison",
		Name = "MT Comparison Tool",
		Description = "MT Comparison Tool")]
	[ActionLayout(typeof(TranslationStudioDefaultContextMenus.ProjectsContextMenuLocation), 2, DisplayType.Default, "",true)]
	public class MtComparisonAction : AbstractAction
	{
		protected override void Execute()
		{
			
		}
	}
}
