using System.Windows;
using System.Windows.Interop;

namespace Sdl.Community.XLIFF.Manager.Wizard.View
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
