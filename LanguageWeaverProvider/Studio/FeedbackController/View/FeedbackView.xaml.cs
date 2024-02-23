using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using LanguageWeaverProvider.Studio.FeedbackController.ViewModel;
using Sdl.Desktop.IntegrationApi.Interfaces;

namespace LanguageWeaverProvider.Studio.FeedbackController.View
{
	/// <summary>
	/// Interaction logic for FeedbackView.xaml
	/// </summary>
	public partial class FeedbackView : UserControl, IUIControl
	{
		readonly FeedbackViewModel _dataContext;

        public FeedbackView()
        {
            InitializeComponent();
			_dataContext = DataContext as FeedbackViewModel;
		}

		public void Dispose() { }

	}
}