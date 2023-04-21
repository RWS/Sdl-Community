using Sdl.Community.MTCloud.Provider.UiHelpers.Watermark;

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