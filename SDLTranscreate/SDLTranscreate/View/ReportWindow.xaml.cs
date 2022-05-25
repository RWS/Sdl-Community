using System.Windows;
using System.Windows.Interop;
using Trados.Transcreate.ViewModel;

namespace Trados.Transcreate.View
{	
	public partial class ReportWindow : Window
	{
		private readonly ReportViewModel _model;

		public ReportWindow(ReportViewModel model)
		{
			_model = model;

			InitializeComponent();

			var windowInteropHelper = new WindowInteropHelper(this);
			windowInteropHelper.Owner = ApplicationInstance.GetActiveForm().Handle;

			Loaded += ReportWindow_Loaded;
		}

		private void ReportWindow_Loaded(object sender, RoutedEventArgs e)
		{
			Loaded -= ReportWindow_Loaded;
			DataContext = _model;
		}
	}
}
