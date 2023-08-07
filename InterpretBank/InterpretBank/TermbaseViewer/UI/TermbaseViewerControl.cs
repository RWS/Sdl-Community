using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.Integration;

namespace InterpretBank.TermbaseViewer.UI
{
	public partial class TermbaseViewerControl : UserControl
	{
		public TermbaseViewerControl(TermbaseViewer termbaseViewer)
		{
			InitializeComponent();
			var elementHost = new ElementHost
			{
				Dock = DockStyle.Fill,
			};
			Controls.Add(elementHost);
			elementHost.Child = termbaseViewer;

			TermbaseViewer = termbaseViewer;
		}

		public TermbaseViewer TermbaseViewer { get; private set; }
	}
}