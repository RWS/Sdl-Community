using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.TranslationStudioAutomation.IntegrationApi;

namespace IATETerminologyProvider.Ui
{
	[ViewPart(
		Id = "IATE Results Viewer",
		Name = "Search Results Viewer",
		Description = "IATE Search Results",
		Icon = "Iate_logo")]
	[ViewPartLayout(typeof(EditorController), Dock = DockType.Bottom)]
	public class SearchResultsViewerController : AbstractViewPartController
	{
		private readonly SearchResultsControl _control = new SearchResultsControl();
		protected override void Initialize()
		{
		}

		protected override Control GetContentControl()
		{
			return _control;
		}

		public WebBrowser Browser => _control.Browser;
	}
}
