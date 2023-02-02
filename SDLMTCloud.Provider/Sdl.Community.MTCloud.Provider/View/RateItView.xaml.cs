using System.Windows.Controls;
using Sdl.Community.MTCloud.Provider.UiHelpers.Watermark;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;

namespace Sdl.Community.MTCloud.Provider.View
{
	/// <summary>
	/// Interaction logic for RateItView.xaml
	/// </summary>
	public partial class RateItView
	{
		public RateItView()
		{
			InitializeComponent();
			WatermarkHandler.Handle(FeedbackTextBox);
		}

		public void FocusFeedbackTextBox()
		{
			FeedbackTextBox.Focus();
		}
	}
}