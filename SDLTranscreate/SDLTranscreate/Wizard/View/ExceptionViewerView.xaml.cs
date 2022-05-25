using System.Windows;
using System.Windows.Interop;

namespace Trados.Transcreate.Wizard.View
{	
	public partial class ExceptionViewerView : Window
	{		
		public ExceptionViewerView()
		{
			InitializeComponent();

			var windowInteropHelper = new WindowInteropHelper(this);
			windowInteropHelper.Owner = ApplicationInstance.GetActiveForm().Handle;
		}
	}
}
